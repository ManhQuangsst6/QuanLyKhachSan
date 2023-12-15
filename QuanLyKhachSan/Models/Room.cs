namespace QuanLyKhachSan.Models
{
    public class Room
    {
        public string RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomCategory { get; set; }
        public int Rate { get; set; }
        public Double Price { get; set; }
        public int RoomStatus { get; set; }
        public int NumberOfFeedbacks { get; set; }
    }
}
