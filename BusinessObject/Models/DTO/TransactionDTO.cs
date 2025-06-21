namespace BusinessObject.Models.DTO
{
    public class TransactionDTO
    {
        public int Transactionid { get; set; }

        public int Bookingid { get; set; }

        public DateTime Transactiondate { get; set; }

        public int? Paymentmethod { get; set; }

        public double? Amountpaid { get; set; }
    }
}
