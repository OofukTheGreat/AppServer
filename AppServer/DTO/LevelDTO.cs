namespace AppServer.DTO
{
    public class LevelDTO
    {
        public int LevelId { get; set; }
        public string Title { get; set; }
        public string Layout { get; set; }
        public int CreatorId { get; set; }
        public int StatusId { get; set; }
        public int Size { get; set; }
        public LevelDTO() { }
        public LevelDTO(Models.Level modelLevel)
        {
            this.LevelId = modelLevel.LevelId;
            this.Title = modelLevel.Title;
            this.Layout = modelLevel.Layout;
            this.Size = modelLevel.Size;
            this.CreatorId = modelLevel.CreatorId;
            this.StatusId = modelLevel.StatusId;
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
                Size = this.Size
            };
            return modelLevel;
        }
    }
}
