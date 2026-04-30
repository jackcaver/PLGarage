using GameServer.Implementation.Common;
using GameServer.Implementation.Player;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class MailMessagesController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("mail_messages.xml")]
        public IActionResult GetMessages(int page, int per_page, string mail_message_types)
        {
            var user = Session.GetUser(database, User);
            return Content(MailMessages.GetMessages(database, user, page, per_page, string.IsNullOrEmpty(mail_message_types) ? [] : mail_message_types.Split(",")), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("mail_messages.xml")]
        public IActionResult CreateMessage(int? reply_to_mail_message_id, MailMessage mail_message)
        {
            var user = Session.GetUser(database, User);
            return Content(MailMessages.CreateMessage(database, user, reply_to_mail_message_id, mail_message), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [Route("mail_messages/{id}.xml")]
        public IActionResult GetMessage(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(MailMessages.GetMessage(database, user, id), "application/xml;charset=utf-8");
        }

        [Authorize]
        [HttpDelete]
        [Route("mail_messages/{id}.xml")]
        public IActionResult RemoveMessage(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(MailMessages.RemoveMessage(database, user, id), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
