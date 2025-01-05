using GameServer.Implementation.Player;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player
{
    public class MailMessagesController : Controller
    {
        private readonly Database database;

        public MailMessagesController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("mail_messages.xml")]
        public IActionResult GetMessages(int page, int per_page, string mail_message_types)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(MailMessagesImpl.GetMessages(database, SessionID, page, per_page, mail_message_types.Split(",")), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("mail_messages.xml")]
        public IActionResult CreateMessage(int? reply_to_mail_message_id, MailMessage mail_message)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(MailMessagesImpl.CreateMessage(database, SessionID, reply_to_mail_message_id, mail_message), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mail_messages/{id}.xml")]
        public IActionResult GetMessage(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(MailMessagesImpl.GetMessage(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("mail_messages/{id}.xml")]
        public IActionResult RemoveMessage(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(MailMessagesImpl.RemoveMessage(database, SessionID, id), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
