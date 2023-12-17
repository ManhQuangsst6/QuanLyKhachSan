using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TypeRoomController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		public TypeRoomController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{

			OracleConnection con = new OracleConnection(_configuration.GetConnectionString("QLKS"));
			con.Open();
			OracleCommand command = con.CreateCommand();
			command.CommandType = System.Data.CommandType.Text;
			command.CommandText = "select * from RoomCategories";
			var res = await command.ExecuteReaderAsync();
			List<TypeRoom> a = new List<TypeRoom>();
			if (res != null)
			{
				while (res.Read())
				{
					TypeRoom room = new TypeRoom();
					room.Id = res["CategoryId"].ToString();
					room.Name = res["CategoryName"].ToString();
					a.Add(room);
				}
			}

			return Ok(a);
		}
	}
}
