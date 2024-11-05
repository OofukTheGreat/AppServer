namespace AppServer.DTO
{
    public class CreatedLevelsDTO
    {
        public string Title { get; set; }
        public string Layout { get; set; }
        public int DifficultyId { get; set; }
        public CreatedLevelsDTO() { }
        public CreatedLevelsDTO(string title, string layout, int difficultyid)
        {
            this.Title = title;
            this.Layout = layout;
            this.DifficultyId = difficultyid;
        }
    }
}
