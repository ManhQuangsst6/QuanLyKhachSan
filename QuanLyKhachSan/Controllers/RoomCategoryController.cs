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
    public class RoomCategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public RoomCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("QLKS");
        }

        [HttpPost("AddRoomCategory")]
        public async Task<IActionResult> AddRoomCategoryAsync(RoomCategory roomCategory)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("AddRoomCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_CategoryID", OracleDbType.Varchar2).Value = Guid.NewGuid().ToString();
                        command.Parameters.Add("p_CategoryName", OracleDbType.Varchar2).Value = roomCategory.CategoryName;
                        command.Parameters.Add("p_Description", OracleDbType.Varchar2).Value = roomCategory.Description;
                        command.Parameters.Add("p_Price", OracleDbType.Double).Value = roomCategory.Price;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("RoomCategory added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateRoomCategory")]
        public async Task<IActionResult> UpdateRoomCategoryAsync(RoomCategory roomCategory, string p_CategoryID)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("UpdateRoomCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_CategoryID", OracleDbType.Varchar2).Value = p_CategoryID;
                        command.Parameters.Add("p_CategoryName", OracleDbType.Varchar2).Value = roomCategory.CategoryName;
                        command.Parameters.Add("p_Description", OracleDbType.Varchar2).Value = roomCategory.Description;
                        command.Parameters.Add("p_Price", OracleDbType.Double).Value = roomCategory.Price;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("RoomCategory updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("DeleteRoomCategory")]
        public async Task<IActionResult> DeleteRoomCategoryAsync(string categoryID)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("DeleteRoomCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_CategoryID", OracleDbType.Varchar2).Value = categoryID;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("RoomCategory deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
