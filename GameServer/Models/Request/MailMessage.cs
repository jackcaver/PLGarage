using GameServer.Models.PlayerData;

namespace GameServer.Models.Request
{
    public class MailMessage
    {
        public string recipient_list {  get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string attachment_reference { get; set; }
        public MailMessageType mail_message_type { get; set; }
    }
}
