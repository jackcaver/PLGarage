using AutoMapper.QueryableExtensions;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Implementation.Player
{
    public class MailMessagesImpl
    {
        public static string GetMessages(Database database, Guid SessionID, int page, int per_page, string[] mail_message_types)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var messagesQuery = database.MailMessages
                // TODO: !!! SEMI IMPORTANT !!! I dont know if the below contains will work, as the underlying database wont have the context of enums, TO INVESTIGATE
                .Where(match => match.Recipient.UserId == user.UserId && mail_message_types.Contains(match.Type.ToString()))
                .OrderByDescending(match => match.CreatedAt);

            // TODO: Remove below once above is answered
            //if (!mail_message_types.Contains(MailMessageType.ALERT.ToString()))
            //    messages.RemoveAll(match => match.Type == MailMessageType.ALERT);
            //if (!mail_message_types.Contains(MailMessageType.WEBSITE.ToString()))
            //    messages.RemoveAll(match => match.Type == MailMessageType.WEBSITE);
            //if (!mail_message_types.Contains(MailMessageType.GAME.ToString()))
            //    messages.RemoveAll(match => match.Type == MailMessageType.GAME);

            //calculating pages
            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = messagesQuery.Count();
            var totalUnread = messagesQuery.Count(match => !match.HasRead);
            var totalPages = PageCalculator.GetTotalPages(total, per_page);
            var messages = messagesQuery
                .Skip(pageStart)
                .Take(per_page)
                .ProjectTo<mailMessage>(database.MapperConfig)
                .ToList();

            var resp = new Response<List<MailMessages>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response =
                [
                    new MailMessages
                    {
                        Page = page,
                        RowEnd = pageEnd,
                        RowStart = pageStart,
                        Total = messages.Count(),
                        TotalPages = totalPages,
                        PlayerId = user.UserId,
                        UnreadCount = totalUnread,
                        MailMessagesList = messages
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string GetMessage(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            
            var message = database.MailMessages
                .FirstOrDefault(match => match.Recipient.UserId == user.UserId && match.Id == id);

            if (user == null || message == null)    // TODO: User may be null
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<Models.Response.MailMessage>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [database.MapperConfig.CreateMapper().Map<Models.Response.MailMessage>(message)]
            };

            message.HasRead = true;
            message.UpdatedAt = DateTime.UtcNow;
            database.SaveChanges();

            return resp.Serialize();
        }

        public static string CreateMessage(Database database, Guid SessionID, int? reply_to_mail_message_id, Models.Request.MailMessage mail_message)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (reply_to_mail_message_id != null)
            {
                var message = database.MailMessages
                    .FirstOrDefault(match => match.Recipient.UserId == user.UserId && match.Id == reply_to_mail_message_id);
                if (message != null) 
                {
                    message.HasReplied = true;
                    message.UpdatedAt = DateTime.UtcNow;
                    database.SaveChanges();
                }
            }

            var recipientUsernames = mail_message.recipient_list.Split(", ");
            var recipients = database.Users
                .Where(match => recipientUsernames.Contains(match.Username))    // TODO: !!! IMPORTANT !!! Do we need to fail if any one username is not found? (Likely yes, though maybe not given how the below works)
                .ToList();

            foreach (var recipient in recipients)
            {
                database.MailMessages.Add(new MailMessageData   // TODO: Can we optimise by using one mail message object with multiple recipients?
                {
                    AttachmentReference = mail_message.attachment_reference,
                    Body = mail_message.body,
                    Subject = mail_message.subject,
                    RecipientList = recipients,
                    Type = mail_message.mail_message_type,
                    Recipient = recipient,
                    Sender = user,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RemoveMessage(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var message = database.MailMessages
                .FirstOrDefault(match => match.Recipient.UserId == user.UserId && match.Id == id);

            if (user == null || message == null)    // TODO: User may be null
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.MailMessages.Remove(message);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
    }
}
