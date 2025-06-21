namespace BusinessObject.Models.DTO
{
    public class ProductpriceDTO
    {
        public int Productpriceid { get; set; }

        public int Productid { get; set; }

        public DateTime Startdate { get; set; }

        public DateTime? Enddate { get; set; }

        public double? Numberprice { get; set; }
    }
}
