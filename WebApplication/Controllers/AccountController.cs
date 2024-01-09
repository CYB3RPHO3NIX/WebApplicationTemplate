using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        public AccountController(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("Database");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShowSignUpPage()
        {
            return View(@"Views\Account\SignUp.cshtml");
        }
        public IActionResult ShowSignInPage()
        {
            return View(@"Views\Account\SignIn.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(string username, string password1, string password2)
        {
            if(password1.Equals(password2))
            {
                RegisterNewUser(username, password1);
            }
            return View();
        }
        public async Task<IActionResult> SignIn()
        {
            return View();
        }
        public bool RegisterNewUser(string username, string password)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Username", username, DbType.String, ParameterDirection.Input);
                parameters.Add("Password", password, DbType.String, ParameterDirection.Input);

                string responseMessage = string.Empty;
                bool IsSuccess = false;
                parameters.Add("IsSuccess", IsSuccess, DbType.Boolean, ParameterDirection.Output);
                parameters.Add("Message", responseMessage, DbType.String, ParameterDirection.Output);
                dbConnection.Query("dbo.InsertUser", parameters, commandType: CommandType.StoredProcedure);

                IsSuccess = parameters.Get<bool>("IsSuccess");
                responseMessage = parameters.Get<string>("Message");
                if(IsSuccess)
                {
                    Log.Information($"Added new User with Username: {username}");
                }else
                {
                    Log.Error($"Error while adding new User. Message: {responseMessage}");
                }
                return IsSuccess;
            }
        }
    }
}
