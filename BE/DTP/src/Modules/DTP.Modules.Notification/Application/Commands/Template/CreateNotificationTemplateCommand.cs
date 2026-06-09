using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Commands.Template
{
    public class CreateNotificationTemplateCommand : IRequest<Guid>
    {
        public CreateTemplateRequest Request { get; set; } = default!;
    }

    public class CreateNotificationTemplateCommandHandler : IRequestHandler<CreateNotificationTemplateCommand, Guid>
    {
        private readonly INotificationService _notificationService;

        public CreateNotificationTemplateCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Guid> Handle(
            CreateNotificationTemplateCommand request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.CreateTemplateAsync(
                request.Request,
                cancellationToken);
        }
    }
}
