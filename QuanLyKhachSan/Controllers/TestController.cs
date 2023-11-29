using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace QuanLyKhachSan.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		public TestController(IConfiguration configuration)
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
			command.CommandText = "select * from bookings";
			var res = await command.ExecuteReaderAsync();
			if (res != null)
			{
				while (res.Read())
				{
				}
			}

			return Ok();
		}
	}
}
