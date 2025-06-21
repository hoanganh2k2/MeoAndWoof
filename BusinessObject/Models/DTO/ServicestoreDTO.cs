namespace BusinessObject.Models.DTO
{
    public class ServicestoreDTO
    {
        public int Productid { get; set; }

        public int Serviceid { get; set; }

        public string Productname { get; set; } = null!;

        public string Productimage { get; set; } = null!;

        public string? Description { get; set; }

        public double? Productdiscount { get; set; }

        public int Categoryid { get; set; }

        public int? Statusid { get; set; }
        public double? currentPrice { get; set; }
    }
}
