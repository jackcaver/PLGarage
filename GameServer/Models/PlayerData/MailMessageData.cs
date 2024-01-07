using GameServer.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class MailMessageData
    {
        private Database _database;
        private Database database
        {
            get
            {
                if (_database != null) return _database;
                return _database = new Database();
            }
            set => _database = value;
        }

        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }

        public string SenderName => this.database.Users.FirstOrDefault(match => match.UserId == this.SenderId).Username;

        public int RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public User Recipient { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string RecipientList { get; set; }
        public string AttachmentReference { get; set; }
        public MailMessageType Type { get; set; }
        public bool HasRead { get; set; }
        public bool HasReplied { get; set; }
        public bool HasForwarded { get; set; }
    }
}
