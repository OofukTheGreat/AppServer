﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;
using AppServer.DTO;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
                dtoUser.ProfilePicture = GetProfileImageVirtualPath(dtoUser.Id);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("signUp")]
        public IActionResult SignUp([FromBody] DTO.PlayerDTO playerDto)
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
                dtoPlayer.ProfilePicture = GetProfileImageVirtualPath(dtoPlayer.Id);
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
        [HttpPost("uploadprofileimage")]
        public async Task<string> SaveProfileImageAsync(IFormFile file, [FromQuery] int userId)
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

                
                return GetProfileImageVirtualPath(userId);

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
            try //d
            {
                string? userMail = HttpContext.Session.GetString("loggedInUser");
                if (string.IsNullOrEmpty(userMail))
                {
                    return Unauthorized("User is not logged in >:(");
                }
                if (scoreDTO.HasWon)
                {
                    Score score = context.Scores.Where(s => s.HasWon == scoreDTO.HasWon && s.PlayerId == scoreDTO.PlayerId && s.LevelId == scoreDTO.LevelId).FirstOrDefault();
                    Score notWon = context.Scores.Where(s => s.HasWon == false && s.PlayerId == scoreDTO.PlayerId && s.LevelId == scoreDTO.LevelId).FirstOrDefault();
                    if (notWon != null)
                    {
                        context.Scores.Remove(notWon);
                    }
                    if (score != null)
                    {
                        score.Time = Math.Min(score.Time, scoreDTO.Time);
                        context.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        Models.Score modelsScore = scoreDTO.GetModels();
                        context.Scores.Add(modelsScore);
                        context.SaveChanges();
                        DTO.ScoreDTO dtoScore = new DTO.ScoreDTO(modelsScore);
                        return Ok(dtoScore);
                    }
                }
                else
                {
                    Score score = context.Scores.Where(s => s.HasWon == scoreDTO.HasWon && s.PlayerId == scoreDTO.PlayerId && s.LevelId == scoreDTO.LevelId).FirstOrDefault();
                    if (score != null)
                    {
                        context.Remove(score);
                    }
                    Models.Score modelsScore = scoreDTO.GetModels();
                    context.Scores.Add(modelsScore);
                    context.SaveChanges();
                    DTO.ScoreDTO dtoScore = new DTO.ScoreDTO(modelsScore);
                    return Ok(dtoScore);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getScoresByList")]
        public IActionResult GetScoresByList([FromQuery] int levelid)
        {
            try
            { 
                List<Models.Score> scores = context.GetScoresByLevel(levelid);
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
        [HttpGet("getScoresByPlayer")]
        public IActionResult GetScoresByPlayer([FromQuery] int playerid)
        {
            try
            { 
                List<Models.Score> scores = context.GetScoresByPlayer(playerid);
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
        [HttpGet("getPlayerWinningScores")]
        public IActionResult GetPlayerWinningScores([FromQuery] int playerid)
        {
            try
            { 
                List<Models.Score> scores = context.GetPlayerWinningScores(playerid).ToList();
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
        [HttpGet("getPlayers")]
        public IActionResult GetPlayers()
        {
            try
            {
                List<Models.Player> players = context.GetPlayers();
                List<DTO.PlayerDTO> Players = new List<PlayerDTO>();
                foreach (Models.Player p in players)
                {
                    p.ProfilePicture = GetProfileImageVirtualPath(p.PlayerId);
                    Players.Add(new DTO.PlayerDTO(p));
                }
                return Ok(Players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getApprovedLevels")]
        public IActionResult GetApprovedLevels()
        {
            try
            {
                List<Models.Level> modelLevels = context.GetApprovedLevels();
                List<DTO.LevelDTO> levelsDto = new List<LevelDTO>();
                foreach (Models.Level l in modelLevels)
                {
                    //p.ProfilePicture = GetProfileImageVirtualPath(p.PlayerId);
                    levelsDto.Add(new DTO.LevelDTO(l));
                }
                return Ok(levelsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getPendingLevels")]
        public IActionResult GetPendingLevels()
        {
            try
            {
                List<Models.Level> modelLevels = context.GetPendingLevels();
                List<DTO.LevelDTO> levelsDto = new List<LevelDTO>();
                foreach (Models.Level l in modelLevels)
                {
                    //p.ProfilePicture = GetProfileImageVirtualPath(p.PlayerId);
                    levelsDto.Add(new DTO.LevelDTO(l));
                }
                return Ok(levelsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getPendingLevelMakers")]
        public IActionResult GetPendingLevelMakers()
        {
            try
            {
                List<Models.Player> modelPlayers = context.GetPendingLevelMakers();
                List<DTO.PlayerDTO> playersDto = new List<PlayerDTO>();
                foreach (Models.Player p in modelPlayers)
                {
                    //p.ProfilePicture = GetProfileImageVirtualPath(p.PlayerId);
                    playersDto.Add(new DTO.PlayerDTO(p));
                }
                return Ok(playersDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("addLevel")]
        public IActionResult AddLevel([FromBody] DTO.LevelDTO levelDto)
        {
            try
            {
                string? userMail = HttpContext.Session.GetString("loggedInUser");
                if (string.IsNullOrEmpty(userMail))
                {
                    return Unauthorized("User is not logged in >:(");
                }
                //Get model user class from DB with matching email. 
                Models.Level modelsLevel = levelDto.GetModels();

                context.Levels.Add(modelsLevel);
                context.SaveChanges();

                //User was added!
                DTO.LevelDTO dtoLevel = new DTO.LevelDTO(modelsLevel);
                return Ok(dtoLevel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("approve")]
        public IActionResult Approve([FromBody] int levelid)
        {
            try
            {
                context.Levels.Where(l => l.LevelId == levelid).FirstOrDefault().StatusId = 2;
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("decline")]
        public IActionResult Decline([FromBody] int levelid)
        {
            try
            {
                context.Levels.Where(l => l.LevelId == levelid).FirstOrDefault().StatusId = 3;
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Backup")]
        public async Task<IActionResult> Backup()
        {
            string path = $"{this.webHostEnvironment.WebRootPath}\\..\\DBScripts\\backup.bak";

            bool success = await BackupDatabaseAsync(path);
            if (success)
            {
                return Ok("Backup was successful");
            }
            else
            {
                return BadRequest("Backup failed");
            }
        }

        [HttpGet("Restore")]
        public async Task<IActionResult> Restore()
        {
            string path = $"{this.webHostEnvironment.WebRootPath}\\..\\DBScripts\\backup.bak";

            bool success = await RestoreDatabaseAsync(path);
            if (success)
            {
                return Ok("Restore was successful");
            }
            else
            {
                return BadRequest("Restore failed");
            }
        }
        //this function backup the database to a specified path
        private async Task<bool> BackupDatabaseAsync(string path)
        {
            try
            {

                //Get the connection string
                string? connectionString = context.Database.GetConnectionString();
                //Get the database name
                string databaseName = context.Database.GetDbConnection().Database;
                //Build the backup command
                string command = $"BACKUP DATABASE {databaseName} TO DISK = '{path}'";
                //Create a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //Open the connection
                    await connection.OpenAsync();
                    //Create a command
                    using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                    {
                        //Execute the command
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //THis function restore the database from a backup in a certain path
        private async Task<bool> RestoreDatabaseAsync(string path)
        {
            try
            {
                //Get the connection string
                string? connectionString = context.Database.GetConnectionString();
                //Get the database name
                string databaseName = context.Database.GetDbConnection().Database;
                //Build the restore command
                string command = $@"
              USE master;
              ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
              RESTORE DATABASE {databaseName} FROM DISK = '{path}' WITH REPLACE;
              ALTER DATABASE {databaseName} SET MULTI_USER;";

                //Create a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //Open the connection
                    await connection.OpenAsync();
                    //Create a command
                    using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                    {
                        //Execute the command
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }

}
