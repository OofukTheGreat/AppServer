namespace AppServer.DTO
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public UserDTO() { }
        public UserDTO(Models.Player modelPlayer)
        {
            this.Email = modelPlayer.Email;
            this.Password = modelPlayer.Password;
            this.DisplayName = modelPlayer.DisplayName;
        }
    }
}
