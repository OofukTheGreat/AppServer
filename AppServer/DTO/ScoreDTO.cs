namespace AppServer.DTO
{
    public class ScoreDTO
    {
        public int PlayerId { get; set; }

        public int LevelId { get; set; }

        public string? CurrentProgress { get; set; }

        public int Time { get; set; }
        public bool HasWon { get; set; }
        public ScoreDTO() { }
        public ScoreDTO(Models.Score modelsScore)
        {
            this.PlayerId = modelsScore.PlayerId;
            this.LevelId = modelsScore.LevelId;
            this.CurrentProgress = modelsScore.CurrentProgress;
            this.Time = modelsScore.Time;
            this.HasWon = modelsScore.HasWon;
        }

        public Models.Score GetModels()
        {
            Models.Score modelsScore = new Models.Score()
            {
                PlayerId = this.PlayerId,
                LevelId = this.LevelId,
                CurrentProgress = this.CurrentProgress,
                Time = this.Time,
                HasWon = this.HasWon
            };
            return modelsScore;
        }
    }
}
