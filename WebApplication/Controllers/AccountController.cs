using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;
using System.Security.Claims;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("Database");
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied()
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
            return View(@"Views\Home\Index.cshtml");
        }
        
        public async Task<IActionResult> SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View(@"Views\Home\Index.cshtml");
        }
        public async Task<IActionResult> SignIn(SignInModel userData)
        {
            var user = Validate(userData);
            if(user != null)
            {
                await LoginAsync(user);
                return View(@"Views\Home\Home.cshtml"); //this is not working
            }
            return View(@"Views\Home\Index.cshtml");
        }
        public async Task LoginAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.SerialNumber, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.EmailId),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(principal);
        }
        public User Validate(SignInModel userData)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Password", userData.Password, DbType.String, ParameterDirection.Input);

                int userId = 0;
                bool IsValid = false;
                string Message = string.Empty;

                parameters.Add("IsValid", IsValid, DbType.Boolean, ParameterDirection.Output);
                parameters.Add("UserId", userId, DbType.Int32, ParameterDirection.Output);
                parameters.Add("Message", Message, DbType.String, ParameterDirection.Output);

                if(!string.IsNullOrEmpty(userData.Username))
                {
                    parameters.Add("Username", userData.Username, DbType.String, ParameterDirection.Input);
                    dbConnection.Query("dbo.ValidateUserByUsername", parameters, commandType: CommandType.StoredProcedure);
                }
                else if(!string.IsNullOrEmpty(userData.Email))
                {
                    parameters.Add("EmailId", userData.Email, DbType.String, ParameterDirection.Input);
                    dbConnection.Query("dbo.ValidateUserByEmailId", parameters, commandType: CommandType.StoredProcedure);
                }
                IsValid = parameters.Get<bool>("IsValid");
                
                if (IsValid) 
                {
                    userId = parameters.Get<int>("UserId");
                    return GetUserDetails(userId);
                }
                return null;
            }
        }
        public User GetUserDetails(int userId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId, DbType.Int32, ParameterDirection.Input);
                var result = dbConnection.Query<User>("dbo.GetUser", parameters, commandType: CommandType.StoredProcedure);
                if (result.Count() > 0) return result.FirstOrDefault();
                else return null;
            }
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
