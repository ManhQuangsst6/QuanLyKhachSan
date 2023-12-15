using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using QuanLyKhachSan.Models;
using System;
using System.Data;
using System.Threading.Tasks;

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
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("AddCustomer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_CustomerID", OracleDbType.Varchar2).Value = Guid.NewGuid().ToString();
                        command.Parameters.Add("p_FullName", OracleDbType.Varchar2).Value = customer.FullName;
                        command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = customer.PhoneNumber;
                        command.Parameters.Add("p_IdentityCard", OracleDbType.Varchar2).Value = customer.IdentityCard;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Customer added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateStatusRoom")]
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
    }
}
