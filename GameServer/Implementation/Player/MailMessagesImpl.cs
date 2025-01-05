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

            var messages = database.MailMessages.Where(match => match.RecipientId == user.UserId).ToList();
            messages.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (!mail_message_types.Contains(MailMessageType.ALERT.ToString()))
                messages.RemoveAll(match => match.Type == MailMessageType.ALERT);
            if (!mail_message_types.Contains(MailMessageType.WEBSITE.ToString()))
                messages.RemoveAll(match => match.Type == MailMessageType.WEBSITE);
            if (!mail_message_types.Contains(MailMessageType.GAME.ToString()))
                messages.RemoveAll(match => match.Type == MailMessageType.GAME);

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, messages.Count);

            if (pageEnd > messages.Count)
                pageEnd = messages.Count;

            var mailMessagesList = new List<mailMessage>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var message = messages[i];
                if (message != null)
                {
                    mailMessagesList.Add(new mailMessage
                    {
                        attachment_reference = message.AttachmentReference,
                        created_at = message.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        has_deleted = false,
                        has_forwarded = message.HasForwarded,
                        has_read = message.HasRead,
                        has_replied = message.HasReplied,
                        id = message.Id,
                        mail_message_type = message.Type.ToString(),
                        recipient_id = message.RecipientId,
                        recipient_list = message.RecipientList,
                        sender_id = message.SenderId,
                        sender_name = message.SenderName,
                        subject = message.Subject,
                        updated_at = message.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            var resp = new Response<List<mail_messages>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response =
                [
                    new mail_messages
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = messages.Count(),
                        total_pages = totalPages,
                        player_id = user.UserId,
                        unread_count = messages.Count(match => !match.HasRead),
                        mailMessagesList = mailMessagesList
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string GetMessage(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var message = database.MailMessages.FirstOrDefault(match => match.Id == id);

            if (user == null || message.RecipientId != user.UserId || message == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<mail_message>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response =
                [
                    new mail_message
                    {
                        attachment_reference = message.AttachmentReference,
                        body = message.Body,
                        created_at = message.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        has_deleted = false,
                        has_forwarded = message.HasForwarded,
                        has_read = message.HasRead,
                        has_replied = message.HasReplied,
                        id = message.Id,
                        sender_id = message.SenderId,
                        sender_name = message.SenderName,
                        subject = message.Subject,
                        updated_at = message.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    }
                ]
            };

            message.HasRead = true;
            message.UpdatedAt = DateTime.UtcNow;
            database.SaveChanges();

            return resp.Serialize();
        }

        public static string CreateMessage(Database database, Guid SessionID, int? reply_to_mail_message_id, MailMessage mail_message)
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
                var message = database.MailMessages.FirstOrDefault(match => match.RecipientId == user.UserId && match.Id == reply_to_mail_message_id);
                if (message != null) 
                {
                    message.HasReplied = true;
                    message.UpdatedAt = DateTime.UtcNow;
                    database.SaveChanges();
                }
            }

            var RecipientList = new List<int>();

            foreach (var recipientName in mail_message.recipient_list.Split(", ")) 
            {
                var recipient = database.Users.FirstOrDefault(match => match.Username == recipientName);
                if (recipient == null)
                {
                    var errorResp = new Response<EmptyResponse>
                    {
                        status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                        response = new EmptyResponse { }
                    };
                    return errorResp.Serialize();
                }
                RecipientList.Add(recipient.UserId);
            }

            foreach (var id in RecipientList)
            {
                database.MailMessages.Add(new MailMessageData
                {
                    AttachmentReference = mail_message.attachment_reference,
                    Body = mail_message.body,
                    Subject = mail_message.subject,
                    RecipientList = mail_message.recipient_list,
                    Type = mail_message.mail_message_type,
                    RecipientId = id,
                    SenderId = user.UserId,
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
            var message = database.MailMessages.FirstOrDefault(match => match.Id == id);

            if (user == null || message.RecipientId != user.UserId || message == null)
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
