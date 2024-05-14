using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace Weather_Example_API_Dev.Controllers
{
    public class ShutdownParameter
    {
        public string UserID { get; set; }
    }
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IApplicationLifetime ApplicationLifetime { get; set;}
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            ApplicationLifetime = applicationLifetime;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        
        [HttpPost(Name = "Shutdown")]
        public string Shutdown([FromBody] ShutdownParameter userID)
        {
            // Example Password for vulnerability finding
            var password = "TestPassword";

            Console.WriteLine("User ID is : " + userID.UserID);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            if(userID.UserID == "Shutdown")
            {
                ApplicationLifetime.StopApplication();
                return "Shutting Down";
            }
            // Vulnerability Scanning Test
            if(userID.UserID == password)
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    String sql = "SELECT * FROM sys.databases WHERE id = " + userID;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                            }
                        }
                    }
                }
            }
           

            return password;
        }
    }
}
