using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;

namespace AppServer.Controllers
{
    [Route("api/[controller]")]
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
        [HttpPost("Sign Up")]
        public IActionResult Register([FromBody] DTO.UserDTO userDto)
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
                DTO.UserDTO dtoUser = new DTO.UserDTO(modelsUser);
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
