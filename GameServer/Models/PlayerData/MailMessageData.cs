﻿using GameServer.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class MailMessageData
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RecipientList { get; set; }
        public string AttachmentReference { get; set; }
        public MailMessageType Type { get; set; }
        public bool HasRead { get; set; }
        public bool HasReplied { get; set; }
        public bool HasForwarded { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User Sender { get; set; }
        public User Recipient { get; set; }
    }
}
