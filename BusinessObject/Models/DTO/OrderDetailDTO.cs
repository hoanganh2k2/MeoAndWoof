namespace BusinessObject.Models.DTO
{
    public class OrderDetailDTO
    {
        public long OrderId { get; set; }

        public int ProductId { get; set; }

        public long? UnitPrice { get; set; }

        public int? Quantity { get; set; }

        public int? Discount { get; set; }
        public virtual Servicestore? Product { get; set; } = null!;
    }
}
