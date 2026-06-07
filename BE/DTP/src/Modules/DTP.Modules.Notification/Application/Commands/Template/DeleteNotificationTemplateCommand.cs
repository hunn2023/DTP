using DTP.Modules.Notification.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Commands.Template
{
    public class DeleteNotificationTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteNotificationTemplateCommandHandler : IRequestHandler<DeleteNotificationTemplateCommand, bool>
    {
        private readonly INotificationService _notificationService;

        public DeleteNotificationTemplateCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(
            DeleteNotificationTemplateCommand request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.DeleteTemplateAsync(
                request.Id,
                cancellationToken);
        }
    }
}
