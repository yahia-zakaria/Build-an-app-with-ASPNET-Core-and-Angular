using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly PresenceTracker _tracker;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly ILogger<MessageHub> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<PresenceHub> presenceHub,
        PresenceTracker tracker, ILogger<MessageHub> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _presenceHub = presenceHub;
            _mapper = mapper;
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await AddToGroup(Context, groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var messages = await _unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<bool> AddToGroup(HubCallerContext context, string groupName)
        {
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(context.ConnectionId, context.User.GetUserName());

            if (group == null)
            {
                group = new Group(groupName);
                _unitOfWork.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            return await _unitOfWork.Complete();
        }

        private async Task RemoveFromMessageGroup(string connectionId)
        {
            var connection = await _unitOfWork.MessageRepository.GetConnection(connectionId);
            _unitOfWork.MessageRepository.RemoveConnection(connection);

            await _unitOfWork.Complete();
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("you cann't send a message to yourself");

            var sender = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername.ToLower());

            if (recipient == null) throw new HubException("Recipient not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
            };
            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(a => a.Username == createMessageDto.RecipientUsername))
            {
                message.DateRead = DateTime.UtcNow;

            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new
                    {
                        Username = sender.UserName,
                        KnownAs = sender.KnownAs
                    });
                }
            }

            message.DateRead = null;
            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }

        }


        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}