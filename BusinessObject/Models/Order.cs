namespace BusinessObject.Models;

public partial class Order
{
	public long OrderId { get; set; }

	public DateTime OrderDate { get; set; }

	public DateTime? ShippedDate { get; set; }

	public int? UserId { get; set; }

	public long? Total { get; set; }

	/// <summary>
	/// 1: Thanh toán khi nhận hàng 2: VNPay 3: PayOS
	/// </summary>
	public short? TransactionType { get; set; }

	public string? AddressShip { get; set; }

	public short? Status { get; set; }

	public int? StoreId { get; set; }

	public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

	public virtual Service? Store { get; set; }

	public virtual User? User { get; set; }
}
