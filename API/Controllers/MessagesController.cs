using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMessageRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(DataContext context, IMessageRepository repo,
        IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("you cann't send a message to yourself");

            var sender = await _context.Users.Include(i => i.Photos).FirstOrDefaultAsync(f => f.UserName == username);
            var recipient = await _context.Users.Include(i => i.Photos).FirstOrDefaultAsync(f => f.UserName.ToLower() == createMessageDto.RecipientUsername.ToLower());

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _repo.AddMessage(message);

            if (await _repo.SaveChangesAsync())
                return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Have failed to send the message");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await _repo.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messageParams.PageNumber, messageParams.PageSize,
            messages.TotalCount, messages.TotalPages);

            return messages;
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUserName();

            return Ok(await _repo.GetMessageThread(currentUsername, username));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var message = await _repo.GetMessage(id);
            var currentUsername = User.GetUserName();

            if (message == null) return NotFound();

            if (message.SenderUsername != currentUsername && message.RecipientUsername != currentUsername)
                return Unauthorized();

            if (message.SenderUsername == currentUsername) message.SenderDeleted = true;
            if (message.RecipientUsername == currentUsername) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) _repo.DeleteMessage(message);

            if (await _repo.SaveChangesAsync()) return Ok();

            return BadRequest("Prblem with deleting a message");
        }
    }
}