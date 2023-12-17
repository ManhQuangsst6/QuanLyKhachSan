using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using QuanLyKhachSan.Models;
using System.Data;

namespace QuanLyKhachSan.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RoomController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public RoomController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> GetRoomList(string? categoryID)
		{
			try
			{
				List<Room> roomList = new List<Room>();

				using (OracleConnection con = new OracleConnection(_configuration.GetConnectionString("QLKS")))
				{
					con.Open();

					using (OracleCommand cmd = new OracleCommand("GetRoomListByCategory", con))
					{
						cmd.CommandType = System.Data.CommandType.StoredProcedure;

						cmd.Parameters.Add("p_CategoryID", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(categoryID) ? (object)DBNull.Value : categoryID;
						cmd.Parameters.Add("p_RoomList", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

						using (OracleDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								Room room = new Room
								{
									RoomId = reader["RoomId"].ToString(),
									//RoomId = reader["RoomNumber"].ToString(),
									RoomNumber = reader["RoomNumber"].ToString(),
									Rate = Convert.ToInt32(reader["Rate"]),
									RoomCategory = reader["CategoryName"].ToString(),
									Price = Convert.ToDouble(reader["Price"].ToString()),
									RoomStatus = Convert.ToInt32(reader["StatusRoom"]),
									NumberOfFeedbacks = Convert.ToInt32(reader["NumberOfFeedbacks"])
								};

								roomList.Add(room);
							}
						}
					}
				}

				return Ok(roomList);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}