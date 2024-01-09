using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;
using WebApplication.Models;

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
        public async Task<IActionResult> SignUp(SignUpModel data)
        {
            if(data.Password.Equals(data.PasswordConfirm))
            {
                RegisterNewUser(data);
            }
            return View();
        }
        public async Task<IActionResult> SignIn()
        {
            return View();
        }
        public bool IsUsernameTaken(string username)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Username", username, DbType.String, ParameterDirection.Input);
                bool IsTaken = false;
                parameters.Add("IsTaken", IsTaken, DbType.Boolean, ParameterDirection.Output);

                dbConnection.Query("dbo.IsUsernameTaken", parameters, commandType: CommandType.StoredProcedure);

                IsTaken = parameters.Get<bool>("IsTaken");

                return IsTaken;
            }
        }
        public bool RegisterNewUser(SignUpModel userData)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Username", userData.Username, DbType.String, ParameterDirection.Input);
                parameters.Add("Password", userData.Password, DbType.String, ParameterDirection.Input);
                parameters.Add("EmailId", userData.Email, DbType.String, ParameterDirection.Input);

                string responseMessage = string.Empty;
                bool IsSuccess = false;
                parameters.Add("IsSuccess", IsSuccess, DbType.Boolean, ParameterDirection.Output);
                parameters.Add("Message", responseMessage, DbType.String, ParameterDirection.Output);
                dbConnection.Query("dbo.InsertUser", parameters, commandType: CommandType.StoredProcedure);

                IsSuccess = parameters.Get<bool>("IsSuccess");
                responseMessage = parameters.Get<string>("Message");
                if(IsSuccess)
                {
                    Log.ForContext("Username", userData.Username).Information($"Successfully added new user.");
                }
                else
                {
                    Log.ForContext("Username", userData.Username).Error($"Error while adding new User. Issue: {responseMessage}");
                }
                return IsSuccess;
            }
        }
    }
}
