namespace QuanLyKhachSan.Models
{
    public class BookingRequestModel
    {
        public string CustomerID { get; set; }
        public string RoomID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
