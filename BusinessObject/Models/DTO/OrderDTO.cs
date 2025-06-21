namespace BusinessObject.Models.DTO
{
    public class OrderDTO
    {
        public long OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? UserId { get; set; }

        public long? Total { get; set; }

        public short? Status { get; set; }
        public short? TransactionType { get; set; }

        public string? AddressShip { get; set; }
        public int? StoreId { get; set; }
        public virtual User? User { get; set; }
    }
}
