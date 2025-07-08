using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Data;

namespace MiniTwitter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HealthCheckController> _logger;
        private readonly IConfiguration _configuration;


        public HealthCheckController(ApplicationDbContext context, ILogger<HealthCheckController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("check-db-connection")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                // Attempt to open a connection to the database by running a simple query
                await _context.Database.ExecuteSqlRawAsync("SELECT 1"); 

                return Ok("Database connection is healthy. yes very");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error connecting to the database: {ex.Message}");
                return StatusCode(500, $"Failed to connect to the database.{ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is running");
        }

        [HttpGet("check-db-connection2")]
        public async Task<IActionResult> CheckDatabaseConnection2()
        {
            try
            {
                // Simulate async work (e.g., fake DB check)
                await Task.Delay(10); // Simulates a delay as if querying the DB

                // Simulate a successful connection
                return Ok("Database connection is healthy yes.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error simulating database connection: {ex.Message}");
                return StatusCode(500, $"Failed to simulate DB connection. {ex.Message}");
            }
        }

        [HttpGet("connection-string")]
        public IActionResult GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return Ok(new { ConnectionString = connectionString });
        }

        [HttpGet("test-sql-connection")]
        public IActionResult TestSqlConnection()
        {
            try
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    return Ok("Connection opened successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                var connStr = _configuration.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return Ok("Connected successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }
    }
   
}
