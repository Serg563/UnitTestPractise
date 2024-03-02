using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.SignalR;
using System.Security.Claims;
using OrderApi.SignalR.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TaskManagerApi.Entities;

namespace OrderApi.Controllers
{
    public class NotificationController : ControllerBase
    {
        private readonly INotificationSink _notificationSink;
        private readonly SignalRContext _context;
        private ApiResponse _apiResponse;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public NotificationController(INotificationSink notificationSink,
            SignalRContext context,
            ApiResponse response,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _notificationSink = notificationSink;
            _context = context;
            _apiResponse = response;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
        } 

        //[Authorize]
        [HttpGet("/notify")]
        public async Task<IActionResult> Notify(string user, string message)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _notificationSink.PushAsync(new(user, message));
            return Ok();
        }

        [HttpGet("testdb")]
        public async Task<IEnumerable<ApplicationUser>> testdb()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO requestDTO)
        {
            ApplicationUser user = _context.ApplicationUsers
                .FirstOrDefault(x => x.UserName.ToLower() == requestDTO.UserName.ToLower());
            if (user != null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("User already exist");
                return BadRequest(_apiResponse);
            }
            try
            {
                var mapped = new ApplicationUser()
                {
                    UserName = requestDTO.UserName,
                    Email = requestDTO.UserName,
                    Name = requestDTO.Name,
                };
                var res = await _userManager.CreateAsync(mapped, requestDTO.Password);
                if (res.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("developer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("developer"));

                    }
                    if (!await _roleManager.RoleExistsAsync("admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("developer"));
                    }
                    if (requestDTO.Role == "admin")
                    {
                        await _userManager.AddToRoleAsync(mapped, "admin");
                    }
                    else if (requestDTO.Role == "developer")
                    {
                        await _userManager.AddToRoleAsync(mapped, "developer");
                    }
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    return Ok(_apiResponse);
                }
            }
            catch (Exception)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Register Error");
                return BadRequest(_apiResponse);
            }
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);

        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDTO requestDTO)
        {
            ApplicationUser user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.UserName.ToLower() == requestDTO.UserName.ToLower());

            if (user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("User not found");
                return BadRequest(_apiResponse);
            }
            bool isValid = await _userManager.CheckPasswordAsync(user, requestDTO.Password);

            if (!isValid)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("password is wrong");
                return BadRequest(_apiResponse);
            }

            try
            {
                string secretKey = "this is the most secret key ever";
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.ASCII.GetBytes(secretKey);
                var role = await _userManager.GetRolesAsync(user);
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("fullname",user.Name),
                        new Claim("id", user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email.ToString()),
                        new Claim(ClaimTypes.Role, role.FirstOrDefault()),
                    }),
                    Expires = DateTime.UtcNow.AddDays(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                var signedInUser = await _signInManager.PasswordSignInAsync(user,requestDTO.Password,true,false);
                if (!signedInUser.Succeeded)
                {   
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("SignIn has failed");
                    return BadRequest(_apiResponse);
                }


                LoginResponseDTO loginResponse = new LoginResponseDTO()
                {
                    Email = user.Email,
                    Token = tokenHandler.WriteToken(token)
                };
                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("UserName or Password is incorrect");
                    return BadRequest(_apiResponse);
                }
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = loginResponse;
                return Ok(_apiResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
           var users = await _context.ApplicationUsers.Include(x => x.UserDetails).ToListAsync();
           return users;
        }
             
    }

    public class ApiResponse
    {
        public ApiResponse()
        {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object? Result { get; set; }
    }
    public class LoginRequestDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponseDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public UserDetails UserDetails { get; set; }
    }
    public class RegisterRequestDTO
    {
        public string? UserName { get; set; }

        public string? Name { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}
