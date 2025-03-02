using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;
[Authorize]
public class MessagesController(IUnitOfWork unitOfWork, IMapper mapper) : BaseAPIController
{
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessageDTO(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername();
        if(username == createMessageDTO.RecipientUsername.ToLower()) return BadRequest("Cannot message yourself");
        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);
        if(recipient == null || sender == null || sender.UserName == null || recipient.UserName == null) return BadRequest("Cannot message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        unitOfWork.MessageRepository.AddMessage(message);
        if(await unitOfWork.Complete()) return Ok(mapper.Map<MessageDTO>(message));
        return BadRequest("Failed to save message");
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(messageParams.PageNumber, messages);
        return messages;
    } 

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();
        return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await unitOfWork.MessageRepository.GetMessage(id);
        if(message == null) return BadRequest("Cannot delete this messgae");
        if(message.SenderUsername != username && message.RecipientUsername != username) return Forbid();
        if(message.SenderUsername == username) message.SenderDeleted = true;
        if(message.RecipientUsername == username) message.RecipientDeleted = true;
        if(message is {SenderDeleted: true, RecipientDeleted : true}) 
        {
            unitOfWork.MessageRepository.DeleteMessage(message);
        }
        if(await unitOfWork.Complete()) return Ok();
        return BadRequest("Failed to delete message");
    }
}
