namespace API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRespository
{
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }
    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = context.Messages
        .OrderByDescending(m => m.MessageSent)
        .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null) // unread messages
        };

        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);
        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var query = context.Messages
        .Where(x => x.RecipientUsername == currentUsername && x.RecipientDeleted == false && x.SenderUsername == recipientUsername 
        || x.SenderUsername == currentUsername && x.SenderDeleted == false  && x.RecipientUsername == recipientUsername)
        .OrderBy(x => x.MessageSent)
        .AsQueryable();

        var unreadMessages = query.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();
        if(unreadMessages.Count() != 0)
        {
            foreach(var unreadMessage in unreadMessages)
            {
                unreadMessage.DateRead = System.DateTime.UtcNow;  
            }
        }

        return await query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups.Include(x => x.Connections).Where(x => x.Connections.Any(x => x.ConnectionId == connectionId)).FirstOrDefaultAsync();
    }
}
