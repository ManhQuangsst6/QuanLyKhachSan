namespace QuanLyKhachSan.Models
{
    public class Booking
    {
        public string BookingID { get; set; }
        public string CustomerID { get; set; }
        public string RoomID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int StatusRoom { get; set; }
    }
}
