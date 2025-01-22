namespace AppServer.DTO
{
    public class LevelDTO
    {
        public string Title { get; set; }
        public string Layout { get; set; }
        public int DifficultyId { get; set; }
        public LevelDTO() { }
        public LevelDTO(string title, string layout, int difficultyid)
        {
            this.Title = title;
            this.Layout = layout;
            this.DifficultyId = difficultyid;
        }
    }
}
