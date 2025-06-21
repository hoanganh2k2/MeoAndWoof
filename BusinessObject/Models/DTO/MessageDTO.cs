namespace BusinessObject.Models.DTO
{
    public class MessageDTO
    {
        public int Messageid { get; set; }

        public int Senderid { get; set; }

        public int Receiverid { get; set; }

        public string? Messagetext { get; set; }

        public DateTime Sendate { get; set; }
    }
}
