namespace GameServer.Models.Request
{
    public class PlayerProfile
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string addr_line1 { get; set; }
        public string addr_line2 { get; set; }
        public string addr_line3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string birthdate { get; set; }
        public string email { get; set; }
        public string email_confirmation { get; set; }
        public string cell_phone { get; set; }
        public string quote { get; set; }
        public string im_yahoo { get; set; }
        public string im_aol { get; set; }
        public string im_msn { get; set; }
        public string im_icq { get; set; }
        public int team_id { get; set; }
    }
}
