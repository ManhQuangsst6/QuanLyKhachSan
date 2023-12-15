using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace QuanLyKhachSan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] BookingRequestModel request)
        {
            //try
            //{
            List<Booking> bookings = new List<Booking>();

            using (OracleConnection con = new OracleConnection(_configuration.GetConnectionString("QLKS")))
            {
                con.Open();

                using (OracleCommand cmd = new OracleCommand("AddBooking", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    List<string> bookingIDList = new List<string>();
                    for (int i = 0; i < request.RoomIDs.Count; i++)
                    {
                        string bookingID = Guid.NewGuid().ToString();
                        bookingIDList.Add(bookingID);
                    }
                    // Định nghĩa và thêm tham số vào cmd
                    cmd.Parameters.Add("p_BookingIDs", OracleDbType.Array, ParameterDirection.Input).Value = bookingIDList.ToArray();
                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Varchar2, ParameterDirection.Input).Value = Guid.NewGuid().ToString();
                    cmd.Parameters.Add("p_FullName", OracleDbType.Varchar2, ParameterDirection.Input).Value = request.FullName;
                    cmd.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2, ParameterDirection.Input).Value = request.PhoneNumber;
                    cmd.Parameters.Add("p_IdentityCard", OracleDbType.Varchar2, ParameterDirection.Input).Value = request.IdentityCard;
                    cmd.Parameters.Add("p_RoomIDs", OracleDbType.Array, ParameterDirection.Input).Value = request.RoomIDs.ToArray();
                    cmd.Parameters.Add("p_CheckInDate", OracleDbType.Date, ParameterDirection.Input).Value = request.CheckInDate;
                    cmd.Parameters.Add("p_CheckOutDate", OracleDbType.Date, ParameterDirection.Input).Value = request.CheckOutDate;

                    // Thêm tham số OUT để nhận dữ liệu trả về
                    cmd.Parameters.Add("p_BookingInfo", OracleDbType.RefCursor, ParameterDirection.Output);

                    // Thực hiện thủ tục
                    cmd.ExecuteNonQuery();

                    // Đọc dữ liệu từ cursor
                    using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_BookingInfo"].Value).GetDataReader())
                    {
                        while (reader.Read())
                        {
                            Booking booking = new Booking
                            {
                                BookingID = reader["BookingID"].ToString(),
                                CustomerID = reader["CustomerID"].ToString(),
                                RoomID = reader["RoomID"].ToString(),
                                CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                                CheckOutDate = Convert.ToDateTime(reader["CheckOutDate"]),
                                StatusRoom = Convert.ToInt32(reader["StatusRoom"])
                            };

                            bookings.Add(booking);
                        }
                    }
                }
            }

            return Ok(bookings);
        }
        //catch (Exception ex)
        //{
        //    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //}
        //}
    }
}
