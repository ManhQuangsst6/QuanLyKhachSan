using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public async Task<IActionResult> GetListCustomers(string customerName, string identityCard)
        {
            try
            {
                List<Customer> customers = new List<Customer>();

                using (OracleConnection con = new OracleConnection(_configuration.GetConnectionString("QLKS")))
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetCustomerPayments", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_CustomerName", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(customerName) ? "" : customerName;
                        cmd.Parameters.Add("p_IdentityCard", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(identityCard) ? "" : identityCard;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
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
