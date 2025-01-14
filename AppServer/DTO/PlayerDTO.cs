using System.ComponentModel.DataAnnotations;

namespace AppServer.DTO
{
    public class PlayerDTO
    {
        public int PlayerId { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? DisplayName { get; set; }

        public string? ProfilePicture { get; set; }

        public bool? IsAdmin { get; set; }
        public PlayerDTO() { }
        public PlayerDTO(Models.Player modelPlayer)
        {
            this.PlayerId = modelPlayer.PlayerId;
            this.Email = modelPlayer.Email;
            this.Password = modelPlayer.Password;
            this.DisplayName = modelPlayer.DisplayName;
            this.IsAdmin = modelPlayer.IsAdmin;
            this.ProfilePicture = modelPlayer.ProfilePicture;
        }

        public Models.Player GetModels()
        {
            Models.Player modelPlayer = new Models.Player()
            {
                PlayerId = this.PlayerId,
                Email = this.Email,
                Password = this.Password,
                DisplayName = this.DisplayName,
                ProfilePicture = this.ProfilePicture,
                IsAdmin = this.IsAdmin
            };
            return modelPlayer;
        }
    }
}
