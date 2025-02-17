﻿using Microsoft.AspNetCore.Http;
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

                DTO.PlayerDTO dtoUser = new DTO.PlayerDTO(modelsUser);
                //dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("signUp")]
        public IActionResult Register([FromBody] DTO.PlayerDTO playerDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.Player modelsPlayer = playerDto.GetModels();

                context.Players.Add(modelsPlayer);
                context.SaveChanges();

                //User was added!
                DTO.PlayerDTO dtoPlayer = new DTO.PlayerDTO(modelsPlayer);
                dtoPlayer.ProfilePicture = GetProfileImageVirtualPath(dtoPlayer.PlayerId);
                return Ok(dtoPlayer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetProfileImageVirtualPath(int userId)
        {
            string virtualPath = $"/profileImages/{userId}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/profileImages/default.png";
                }
            }

            return virtualPath;
        }


        //THis function gets a userId and a profile image file and save the image in the server
        //The function return the full path of the file saved
        private async Task<string> SaveProfileImageAsync(int userId, IFormFile file)
        {
            //Read all files sent
            long imagesSize = 0;

            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    throw new Exception("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                        throw new Exception("File sent is not an image");
                    }

                }

                return filePath;

            }

            throw new Exception("File in size 0");
        }
        private static bool IsImage(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }
        [HttpPost("addScore")]
        public IActionResult AddScore([FromBody] DTO.ScoreDTO scoreDTO)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Create model user class
                Models.Score modelsScore = scoreDTO.GetModels();

                context.Scores.Add(modelsScore);
                context.SaveChanges();

                //User was added!
                DTO.ScoreDTO dtoUser = new DTO.ScoreDTO(modelsScore);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("getScores")]
        public IActionResult GetScores(int levelid)
        {
            try
            {
                List<Models.Score> scores = context.GetScores(levelid);
                List<DTO.ScoreDTO> Scores = new List<ScoreDTO>();
                foreach (Models.Score s in scores)
                {
                    Scores.Add(new DTO.ScoreDTO(s));
                }
                return Ok(Scores);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("getPlayers")]
        public IActionResult GetPlayers(int levelid)
        {
            try
            {
                List<Models.Player> players = context.GetPlayers();
                List<DTO.PlayerDTO> Players = new List<PlayerDTO>();
                foreach (Models.Player p in players)
                {
                    Players.Add(new DTO.PlayerDTO(p));
                }
                return Ok(Players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }

}
