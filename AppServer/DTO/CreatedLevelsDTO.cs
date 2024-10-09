namespace AppServer.DTO
{
    public class CreatedLevelsDTO
    {
        public string Title { get; set; }
        public string Layout { get; set; }
        public int DifficultyId { get; set; }
        public CreatedLevelsDTO() { }
        public CreatedLevelsDTO(Models.Level modelLevel)
        {
            this.Title = modelLevel.Title;
            this.Layout = modelLevel.Layout;
            this.DifficultyId = modelLevel.DifficultyId;
        }
    }
}
