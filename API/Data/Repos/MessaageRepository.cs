using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repos
{
    public class MessaageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessaageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
            .Include(i=>i.Sender)
            .Include(i=>i.Recipient)
            .SingleOrDefaultAsync(s=>s.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(i=>i.Connections)
            .FirstOrDefaultAsync(f=>f.Name==groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(o => o.MessageSent)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(w => w.RecipientUsername == messageParams.Username && w.RecipientDeleted == false),
                "Outbox" => query.Where(w => w.SenderUsername == messageParams.Username && w.SenderDeleted == false),
                _ => query.Where(w => w.RecipientUsername == messageParams.Username && w.DateRead == null && w.RecipientDeleted == false)
            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
            .Include(i => i.Sender).ThenInclude(t => t.Photos)
            .Include(i => i.Recipient).ThenInclude(t => t.Photos)
            .Where(w => w.Recipient.UserName == currentUsername && w.RecipientDeleted == false && w.Sender.UserName == recipientUsername
            || w.Recipient.UserName == recipientUsername && w.Sender.UserName == currentUsername && w.RecipientDeleted == false)
            .OrderByDescending(o => o.MessageSent).ToListAsync();

            var unreadMessage = messages.Where(w => w.RecipientUsername == currentUsername && w.DateRead == null).ToList();

            if (unreadMessage.Any())
            {
                foreach (var item in unreadMessage)
                {
                    item.DateRead = System.DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
    }
}