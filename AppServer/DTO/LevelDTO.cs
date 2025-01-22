namespace AppServer.DTO
{
    public class LevelDTO
    {
        public int LevelId { get; set; }
        public string Title { get; set; }
        public string Layout { get; set; }
        public int CreatorId { get; set; }
        public int StatusId { get; set; }
        public int DifficultyId { get; set; }
        public LevelDTO() { }
        public LevelDTO(int levelId, string title, string layout, int difficultyId, int creatorId, int statusId)
        {
            this.LevelId = levelId;
            this.Title = title;
            this.Layout = layout;
            this.DifficultyId = difficultyId;
            this.CreatorId = creatorId;
            this.StatusId = statusId;
        }
        public Models.Level GetModels()
        {
            Models.Level modelLevel = new Models.Level()
            {
                LevelId = this.LevelId,
                Title = this.Title,
                Layout = this.Layout,
                CreatorId = this.CreatorId,
                StatusId = this.StatusId,
                DifficultyId = this.DifficultyId
            };
            return modelLevel;
        }
    }
}
