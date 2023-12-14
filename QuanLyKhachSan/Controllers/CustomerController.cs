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
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetListCustomers(string? customerName, string? identityCard)
        {
            try
            {
                List<Customer> customers = new List<Customer>();

                // Mở kết nối đến database Oracle
                using (OracleConnection con = new OracleConnection(_configuration.GetConnectionString("QLKS")))
                {
                    con.Open();

                    // Tạo đối tượng OracleCommand để thực hiện thủ tục
                    using (OracleCommand cmd = new OracleCommand("GetCustomerPayments", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Input parameters
                        cmd.Parameters.Add("p_CustomerName", OracleDbType.Varchar2).Value = customerName != null ? customerName : DBNull.Value;
                        cmd.Parameters.Add("p_IdentityCard", OracleDbType.Varchar2).Value = identityCard != null ? identityCard : DBNull.Value;


                        // Output parameter (ref cursor)
                        cmd.Parameters.Add("p_CustomerPayments", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                // Create a Customer object and populate its properties from the reader
                                Customer customer = new Customer
                                {
                                    CustomerID = reader["CustomerID"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    IdentityCard = reader["IdentityCard"].ToString(),
                                    NumberOfPayments = Convert.ToInt32(reader["NumberOfPayments"])
                                };

                                customers.Add(customer);
                            }
                        }
                    }
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}