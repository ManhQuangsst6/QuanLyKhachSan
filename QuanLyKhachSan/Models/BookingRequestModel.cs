namespace QuanLyKhachSan.Models
{
    public class BookingRequestModel
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public List<string> RoomIDs { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

}
