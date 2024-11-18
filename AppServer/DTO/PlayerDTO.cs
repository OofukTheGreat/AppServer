namespace AppServer.DTO
{
    public class PlayerDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public PlayerDTO() { }
        public PlayerDTO(string email, string password, string displayname)
        {
            this.Email = email;
            this.Password = password;
            this.DisplayName = displayname;
        }
    }
}
