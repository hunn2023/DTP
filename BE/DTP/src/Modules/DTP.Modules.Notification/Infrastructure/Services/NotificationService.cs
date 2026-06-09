using DTP.Modules.Notification.Application.Abstractions.Repositories;
using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.DTOs;
using DTP.Modules.Notification.Domain.Entities;
using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationMessageRepository _messageRepository;
        private readonly INotificationTemplateRepository _templateRepository;
        private readonly INotificationDeliveryLogRepository _deliveryLogRepository;
        private readonly IEmailNotificationSender _emailSender;
        private readonly INotificationUnitOfWork _unitOfWork;

        public NotificationService(
            INotificationMessageRepository messageRepository,
            INotificationTemplateRepository templateRepository,
            INotificationDeliveryLogRepository deliveryLogRepository,
            IEmailNotificationSender emailSender,
            INotificationUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _templateRepository = templateRepository;
            _deliveryLogRepository = deliveryLogRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> SendAsync(
            CreateNotificationRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateSendRequest(request);

            var title = request.Title;
            var content = request.Content;

            if (!string.IsNullOrWhiteSpace(request.TemplateCode))
            {
                var template = await _templateRepository.GetActiveAsync(
                    request.TemplateCode,
                    request.Channel,
                    cancellationToken);

                if (template == null)
                    throw new Exception("Notification template not found or inactive.");

                title = ApplyTemplate(template.TitleTemplate, request.Data);
                content = ApplyTemplate(template.ContentTemplate, request.Data);
            }

            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Notification title is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Notification content is required.");

            var message = new NotificationMessage(
                request.UserId,
                request.Email,
                request.Type,
                request.Channel,
                title,
                content,
                request.ReferenceType,
                request.ReferenceId);

            await _messageRepository.AddAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.Channel == NotificationChannel.InApp)
            {
                message.MarkSent();

                await _deliveryLogRepository.AddAsync(
                    new NotificationDeliveryLog(
                        message.Id,
                        request.Channel,
                        NotificationStatus.Sent,
                        "InApp",
                        null,
                        "Stored in database",
                        null),
                    cancellationToken);

                _messageRepository.Update(message);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return message.Id;
            }

            if (request.Channel == NotificationChannel.Email)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.Email))
                        throw new Exception("Email is required for email notification.");

                    await _emailSender.SendAsync(
                        request.Email,
                        title,
                        content,
                        cancellationToken);

                    message.MarkSent();

                    await _deliveryLogRepository.AddAsync(
                        new NotificationDeliveryLog(
                            message.Id,
                            request.Channel,
                            NotificationStatus.Sent,
                            "SMTP",
                            null,
                            "Email sent",
                            null),
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    message.MarkFailed(ex.Message);

                    await _deliveryLogRepository.AddAsync(
                        new NotificationDeliveryLog(
                            message.Id,
                            request.Channel,
                            NotificationStatus.Failed,
                            "SMTP",
                            null,
                            null,
                            ex.Message),
                        cancellationToken);
                }

                _messageRepository.Update(message);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return message.Id;
            }

            message.MarkFailed("Unsupported notification channel.");

            await _deliveryLogRepository.AddAsync(
                new NotificationDeliveryLog(
                    message.Id,
                    request.Channel,
                    NotificationStatus.Failed,
                    "System",
                    null,
                    null,
                    "Unsupported notification channel."),
                cancellationToken);

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return message.Id;
        }

        public async Task<bool> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var message = await _messageRepository.GetByIdAsync(notificationId, cancellationToken);

            if (message == null)
                throw new Exception("Notification not found.");

            if (message.UserId != userId)
                throw new Exception("You do not have permission to read this notification.");

            message.MarkRead();

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<NotificationMessageDto>> GetMyNotificationsAsync(
            Guid userId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            var items = await _messageRepository.GetByUserIdAsync(
                userId,
                pageIndex,
                pageSize,
                cancellationToken);

            return items.Select(MapMessage).ToList();
        }

        public async Task<List<NotificationMessageDto>> GetAdminNotificationsAsync(
            Guid? userId,
            NotificationType? type,
            NotificationChannel? channel,
            NotificationStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            var items = await _messageRepository.GetPagedAsync(
                userId,
                type,
                channel,
                status,
                pageIndex,
                pageSize,
                cancellationToken);

            return items.Select(MapMessage).ToList();
        }

        public async Task<Guid> CreateTemplateAsync(
            CreateTemplateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Template code is required.");

            if (string.IsNullOrWhiteSpace(request.TitleTemplate))
                throw new Exception("Title template is required.");

            if (string.IsNullOrWhiteSpace(request.ContentTemplate))
                throw new Exception("Content template is required.");

            var exists = await _templateRepository.ExistsCodeAsync(
                request.Code,
                null,
                cancellationToken);

            if (exists)
                throw new Exception("Template code already exists.");

            var template = new NotificationTemplate(
                request.Code.Trim(),
                request.Type,
                request.Channel,
                request.TitleTemplate.Trim(),
                request.ContentTemplate.Trim(),
                request.IsActive);

            await _templateRepository.AddAsync(template, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return template.Id;
        }

        public async Task<bool> UpdateTemplateAsync(
            Guid id,
            UpdateTemplateRequest request,
            CancellationToken cancellationToken = default)
        {
            var template = await _templateRepository.GetByIdAsync(id, cancellationToken);

            if (template == null)
                throw new Exception("Notification template not found.");

            if (string.IsNullOrWhiteSpace(request.TitleTemplate))
                throw new Exception("Title template is required.");

            if (string.IsNullOrWhiteSpace(request.ContentTemplate))
                throw new Exception("Content template is required.");

            template.Update(
                request.TitleTemplate.Trim(),
                request.ContentTemplate.Trim(),
                request.IsActive);

            _templateRepository.Update(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteTemplateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var template = await _templateRepository.GetByIdAsync(id, cancellationToken);

            if (template == null)
                throw new Exception("Notification template not found.");

            _templateRepository.Remove(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<NotificationTemplateDto>> GetTemplatesAsync(
            CancellationToken cancellationToken = default)
        {
            var items = await _templateRepository.GetAllAsync(cancellationToken);

            return items.Select(x => new NotificationTemplateDto
            {
                Id = x.Id,
                Code = x.Code,
                Type = x.Type,
                Channel = x.Channel,
                TitleTemplate = x.TitleTemplate,
                ContentTemplate = x.ContentTemplate,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        private static void ValidateSendRequest(CreateNotificationRequest request)
        {
            if (request.UserId == null && string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("UserId or Email is required.");

            if (request.Channel == NotificationChannel.Email &&
                string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email is required for email notification.");

            if (string.IsNullOrWhiteSpace(request.TemplateCode) &&
                string.IsNullOrWhiteSpace(request.Title))
                throw new Exception("Title or TemplateCode is required.");

            if (string.IsNullOrWhiteSpace(request.TemplateCode) &&
                string.IsNullOrWhiteSpace(request.Content))
                throw new Exception("Content or TemplateCode is required.");
        }

        private static string ApplyTemplate(
            string template,
            Dictionary<string, string>? data)
        {
            if (data == null || data.Count == 0)
                return template;

            foreach (var item in data)
            {
                template = template.Replace(
                    "{{" + item.Key + "}}",
                    item.Value ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase);
            }

            return template;
        }

        private static void NormalizePaging(ref int pageIndex, ref int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;
        }

        private static NotificationMessageDto MapMessage(NotificationMessage x)
        {
            return new NotificationMessageDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Email = x.Email,
                Type = x.Type,
                Channel = x.Channel,
                Status = x.Status,
                Title = x.Title,
                Content = x.Content,
                ReferenceType = x.ReferenceType,
                ReferenceId = x.ReferenceId,
                IsRead = x.IsRead,
                ReadAt = x.ReadAt,
                SentAt = x.SentAt,
                ErrorMessage = x.ErrorMessage,
                CreatedAt = x.CreatedAt
            };
        }
    }
}
