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
    public class ServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceList()
        {
            try
            {
                List<Service> services = new List<Service>();

                using (OracleConnection con = new OracleConnection(_configuration.GetConnectionString("QLKS")))
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetServiceList", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Thêm tham số OUT
                        cmd.Parameters.Add("p_ServiceList", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Service service = new Service
                                {
                                    ServiceName = reader["ServiceName"].ToString(),
                                    Price = Convert.ToDouble(reader["Price"])
                                };

                                services.Add(service);
                            }
                        }
                    }
                }

                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}