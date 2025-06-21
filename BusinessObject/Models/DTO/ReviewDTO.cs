namespace BusinessObject.Models.DTO
{
    public class ReviewDTO
    {
        public int Reviewid { get; set; }

        public int Serviceid { get; set; }

        public int Userid { get; set; }

        public int Rating { get; set; }

        public string? Reviewtext { get; set; }

        public DateTime? Reviewdate { get; set; }
    }
}
