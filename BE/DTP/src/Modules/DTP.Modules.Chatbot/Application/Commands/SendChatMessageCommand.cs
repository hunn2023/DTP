using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.Commands
{
    public sealed class SendChatMessageCommand : IRequest<ChatResponseDto>
    {
        public ChatRequestDto Request { get; }

        public SendChatMessageCommand(ChatRequestDto request)
        {
            Request = request;
        }
    }

    public sealed class SendChatMessageCommandHandler
        : IRequestHandler<SendChatMessageCommand, ChatResponseDto>
    {
        private readonly IChatbotService _chatbotService;

        public SendChatMessageCommandHandler(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        public async Task<ChatResponseDto> Handle(
            SendChatMessageCommand request,
            CancellationToken cancellationToken)
        {
            return await _chatbotService.SendMessageAsync(
                request.Request,
                cancellationToken);
        }
    }
}
