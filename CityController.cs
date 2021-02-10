using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FixitBEApi.Models;

namespace FixitBEApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        // GET: api/City/countryCode
        [HttpGet("{countryCode}")]
        public string Get(string countryCode)
        {
			// Make sure the returned dataset is in JSON format so that JSON tools can be used to parse it when returned to the caller.
            string qs = "SELECT [City] FROM [FixIt].[dbo].[City] with (nolock) where CountryCode = '" + countryCode.ToString() + "'  order by City FOR JSON AUTO";
            string connectionString = Globals.Fixit_BE_ConnectionString;
            string SQLResult = "";

            using (SqlConnection connection = new SqlConnection(
                        connectionString))
            {
                SqlCommand command = new SqlCommand(qs, connection);
                command.CommandTimeout = 60;
                command.Connection.Open();
                SqlDataReader dr;
                dr = command.ExecuteReader();
                while (dr.Read())
                {	// Compose the City List as a concatenated string for later parsing
                    SQLResult = SQLResult + dr.GetValue(0);
                }
                dr.Close();
                connection.Close();
                return SQLResult;
            }
        }

        // POST: api/City
        [HttpPost]
        public void Post([FromBody] CityItem value)
        {	
            DateTime msgDate = DateTime.Now;
            string msgDateStr = msgDate.ToString("yyyy/MM/dd HH:mm:ss");
            string qs = "INSERT INTO [FixIt].[dbo].[City] (CountryCode, City) Values ('" + value.CountryCode + "', '" + value.City + "')";
            FixitBEController obj = new FixitBEController();
			// Insert the City Record into the Database
            if (obj.SQLFixItDataSubmit(qs))
            {	// If insert is successfull, log the event along with the user whose privilege was used to perform the transaction
                qs = "INSERT INTO[dbo].[Message]([MsgDate],[UserHandle],[StatusCode],[StatusMessage]) VALUES('";
                qs = qs + msgDateStr+ "','" + value.UserHandle + "','OK','City item inserted.')";
                obj.SQLFixItLogSubmit(qs);
            }
        }
    }
}
