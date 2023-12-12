using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using AngularAuthYtAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

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

            // Check if the provided credentials match the default admin credentials
            if (userObj.Email == "admin" && userObj.Password == "admin")
            {

                var adminUser = new User { Email = "admin", IsAdmin = true, Role = "admin" };
                return Ok(new
                {
                    Message = "true",
                    User = adminUser
                });
            }

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

            //check username
            if (userObj.Email == "admin" && userObj.Password == "admin")
            {
                return BadRequest(new { Message = "This is not valid" });
            }

            // Set Role to "doctor" if it is neither "admin" nor "client"
            if (userObj.Role != "admin" && userObj.Role != "client")
            {
                userObj.Role = "doctor";
            }
            /*// check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist" });*/

            //check username
            if (await CheckIdNumberExistAsync(userObj.IdNumber))
                return BadRequest(new { Message = "Idnumber Already Exist" });
           
            

                /*var passMessage = CheckPasswordStrength(userObj.Password);
                if (!string.IsNullOrEmpty(passMessage))
                    return BadRequest(new { Message = passMessage.ToString() });*/

            await _authContext.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "User Added!"
            });
        }

        /*private Task<bool> CheckEmailExistAsync(string? email)
            => _authContext.Users.AnyAsync(x => x.Email == email);*/

        private Task<bool> CheckIdNumberExistAsync(string? idnumber)
            => _authContext.Users.AnyAsync(x => x.IdNumber == idnumber);

        /*private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter" + Environment.NewLine);
            return sb.ToString();
        }*/


        [HttpGet("getUser")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _authContext.Users.ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine(ex);

                // Return an error response
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


    }


}
