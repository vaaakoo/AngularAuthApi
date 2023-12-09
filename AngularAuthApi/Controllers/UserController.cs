using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using AngularAuthYtAPI.Context;
using Microsoft.EntityFrameworkCore;


namespace AngularAuthYtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext context)
        {
            _authContext = context;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x => x.Email == userObj.Email && x.Password == userObj.Password);

            if (user == null)
                return NotFound(new { Message = "User not found!" });


            return Ok(new
            {
                Message = "Login Succes!"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            // check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist" });

            //check username
            if (await CheckIdNumberExistAsync(userObj.IdNumber))
                return BadRequest(new { Message = "Idnumber Already Exist" });

            var passMessage = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(passMessage))
                return BadRequest(new { Message = passMessage.ToString() });

            await _authContext.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "User Added!"
            });
        }

        private Task<bool> CheckEmailExistAsync(string? email)
            => _authContext.Users.AnyAsync(x => x.Email == email);

        private Task<bool> CheckIdNumberExistAsync(int? idnumber)
            => _authContext.Users.AnyAsync(x => x.IdNumber == idnumber);

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter" + Environment.NewLine);
            return sb.ToString();
        }

       
      
    }

}
