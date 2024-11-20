using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;
using AppServer.DTO;

namespace AppServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NRAPIController : ControllerBase
    {
        //a variable to hold a reference to the db context!
        private DBContext context;
        //a variable that hold a reference to web hosting interface (that provide information like the folder on which the server runs etc...)
        private IWebHostEnvironment webHostEnvironment;
        //Use dependency injection to get the db context and web host into the constructor
        public NRAPIController(DBContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.webHostEnvironment = env;
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            try
            {              
                return Ok("works");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.Player? modelsUser = context.GetUser(loginDto.Email);

                //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
                if (modelsUser == null || modelsUser.Password != loginDto.Password)
                {
                    return Unauthorized();
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", modelsUser.Email);

                DTO.PlayerDTO dtoUser = new DTO.PlayerDTO(modelsUser.Email,modelsUser.Password,modelsUser.DisplayName);
                //dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("Sign Up")]
        public IActionResult Register([FromBody] DTO.PlayerDTO userDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.Player modelsUser = new Player()
                {
                    Email = userDto.Email,
                    Password = userDto.Password,
                    DisplayName = userDto.DisplayName
                };

                context.Players.Add(modelsUser);
                context.SaveChanges();

                //User was added!
                DTO.PlayerDTO dtoUser = new DTO.PlayerDTO(modelsUser.Email, modelsUser.Password, modelsUser.DisplayName);
                //dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);0
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
