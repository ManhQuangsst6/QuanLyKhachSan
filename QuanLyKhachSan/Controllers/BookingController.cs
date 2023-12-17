using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using QuanLyKhachSan.Models;
using System.Data;

namespace QuanLyKhachSan.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly string _connectionString;

		public BookingController(IConfiguration configuration)
		{
			_configuration = configuration;
			_connectionString = _configuration.GetConnectionString("QLKS");
		}

		[HttpPost("AddCustomer")]
		public async Task<IActionResult> AddCustomerAsync(CustomerRequestModel customer)
		{
			try
			{
				string id = Guid.NewGuid().ToString();
				using (var connection = new OracleConnection(_connectionString))
				{
					connection.Open();

					using (var command = new OracleCommand("AddCustomer", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_CustomerID", OracleDbType.Varchar2).Value = id;
						command.Parameters.Add("p_FullName", OracleDbType.Varchar2).Value = customer.FullName;
						command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = customer.PhoneNumber;
						command.Parameters.Add("p_IdentityCard", OracleDbType.Varchar2).Value = customer.IdentityCard;

						await command.ExecuteNonQueryAsync();
					}
				}

				return Ok(id);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
			}
		}

		[HttpGet("UpdateStatusRoom")]
		public async Task<IActionResult> UpdateStatusRoomAsync(string roomID)
		{
			try
			{
				using (var connection = new OracleConnection(_connectionString))
				{
					connection.Open();

					using (var command = new OracleCommand("UpdateStatusRoom", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_RoomID", OracleDbType.Varchar2).Value = roomID;

						await command.ExecuteNonQueryAsync();
					}
				}

				return Ok("Room status updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
			}
		}

		[HttpPost("AddBooking")]
		public async Task<IActionResult> AddBookingAsync(BookingRequestModel booking)
		{
			try
			{
				using (var connection = new OracleConnection(_connectionString))
				{
					connection.Open();

					using (var command = new OracleCommand("AddBooking", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_BookingID", OracleDbType.Varchar2).Value = Guid.NewGuid().ToString();
						command.Parameters.Add("p_CustomerID", OracleDbType.Varchar2).Value = booking.CustomerID;
						command.Parameters.Add("p_RoomID", OracleDbType.Varchar2).Value = booking.RoomID;
						command.Parameters.Add("p_CheckInDate", OracleDbType.Date).Value = booking.CheckInDate;
						command.Parameters.Add("p_CheckOutDate", OracleDbType.Date).Value = booking.CheckOutDate;

						await command.ExecuteNonQueryAsync();
					}
				}

                return Ok("Booking added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpPost("GetCustomerBookingInfo")]
        public async Task<IActionResult> GetCustomerBookingInfoAsync(string bookingID)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("GetCustomerBookingInfo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Đặt tham số cho stored procedure
                        command.Parameters.Add("p_BookingID", OracleDbType.Varchar2).Value = bookingID;

                        // Thêm tham số output cho kết quả truy vấn
                        command.Parameters.Add("p_CustomerInfo", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Thực hiện truy vấn
                        await command.ExecuteNonQueryAsync();

                        // Lấy kết quả từ tham số output
                        OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_CustomerInfo"].Value).GetDataReader();

                        // Kiểm tra xem có dữ liệu hay không
                        if (reader.HasRows)
                        {
                            // Đọc dữ liệu từ SqlDataReader
                            while (reader.Read())
                            {
                                string fullName = reader["FullName"].ToString();
                                DateTime checkInDate = Convert.ToDateTime(reader["CheckInDate"]);
                                string phoneNumber = reader["PhoneNumber"].ToString();
                                string identityCard = reader["IdentityCard"].ToString();

                                // Thực hiện các thao tác mong muốn với dữ liệu
                                // Ví dụ: Trả về dữ liệu dưới dạng JSON
                                var result = new
                                {
                                    FullName = fullName,
                                    CheckInDate = checkInDate,
                                    PhoneNumber = phoneNumber,
                                    IdentityCard = identityCard
                                };

                                return Ok(result);
                            }
                        }
                    }
                }

                return NotFound("Booking not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("GetRoomsByCustomerID")]
        public async Task<IActionResult> GetRoomsByCustomerIDAsync(string customerID)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("GetRoomsByCustomerID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Đặt tham số cho stored procedure
                        command.Parameters.Add("p_CustomerID", OracleDbType.Varchar2).Value = customerID;

                        // Thêm tham số output cho kết quả truy vấn
                        command.Parameters.Add("p_RoomsInfo", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Thực hiện truy vấn
                        await command.ExecuteNonQueryAsync();

                        // Lấy kết quả từ tham số output
                        OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_RoomsInfo"].Value).GetDataReader();

                        // Kiểm tra xem có dữ liệu hay không
                        if (reader.HasRows)
                        {
                            // Tạo một danh sách để lưu trữ thông tin về các phòng đã đặt
                            List<object> roomsList = new List<object>();

                            // Đọc dữ liệu từ SqlDataReader
                            while (reader.Read())
                            {
                                string roomName = reader["RoomName"].ToString();
                                string categoryName = reader["CategoryName"].ToString();
                                decimal rate = Convert.ToDecimal(reader["Rate"]);
                                decimal price = Convert.ToDecimal(reader["Price"]);
                                int roomStatus = Convert.ToInt32(reader["RoomStatus"]);

                                // Thêm thông tin của mỗi phòng vào danh sách
                                roomsList.Add(new
                                {
                                    RoomName = roomName,
                                    CategoryName = categoryName,
                                    Rate = rate,
                                    Price = price,
                                    RoomStatus = roomStatus
                                });
                            }

                            // Trả về danh sách các phòng đã đặt dưới dạng JSON
                            return Ok(roomsList);
                        }
                    }
                }

                return NotFound("No rooms found for the given customer.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("AddPaymentTransaction")]
        public async Task<IActionResult> AddPaymentTransactionAsync(PaymentTransactionRequestModel paymentTransaction)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("AddPaymentTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Đặt tham số cho stored procedure
                        command.Parameters.Add("p_TransactionID", OracleDbType.Varchar2).Value = Guid.NewGuid().ToString();
                        command.Parameters.Add("p_BookingID", OracleDbType.Varchar2).Value = paymentTransaction.BookingID;
                        command.Parameters.Add("p_PaymentDate", OracleDbType.Date).Value = DateTime.Now;
                        command.Parameters.Add("p_Amount", OracleDbType.Decimal).Value = paymentTransaction.Amount;
                        command.Parameters.Add("p_PaymentMethod", OracleDbType.Decimal).Value = paymentTransaction.PaymentMethod;
                        command.Parameters.Add("p_StatusPayment", OracleDbType.Decimal).Value = 0;

                        // Thực hiện truy vấn
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Payment Transaction added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetPaymentTransactions")]
        public async Task<IActionResult> GetPaymentTransactionsAsync()
        {
            try
            {
                List<object> paymentTransactions = new List<object>();

                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("SELECT * FROM PaymentTransactions", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                // Đọc dữ liệu từ SqlDataReader và thêm vào danh sách
                                var paymentTransaction = new
                                {
                                    TransactionID = reader["TransactionID"].ToString(),
                                    BookingID = reader["BookingID"].ToString(),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    PaymentMethod = Convert.ToInt32(reader["PaymentMethod"]),
                                    StatusPayment = Convert.ToInt32(reader["StatusPayment"])
                                };

                                paymentTransactions.Add(paymentTransaction);
                            }
                        }
                    }
                }

                return Ok(paymentTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
