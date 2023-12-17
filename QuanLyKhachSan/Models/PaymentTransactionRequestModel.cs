namespace QuanLyKhachSan.Models
{
    public class PaymentTransactionRequestModel
    {
        public string BookingID { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
    }
}
