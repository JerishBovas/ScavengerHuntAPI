namespace ScavengerHunt.DTOs
{
    public record struct GetInfoDto
    {
        public UserDto User { get; set; }
        public int GamesCreated { get; set; }
        public int TeamsJoined { get; set; }
        public int GamesWon { get; set; }
        public GameDto GameOfTheDay { get; set; }
        public List<GameDto> OurFavorites { get; set; }
        public List<GameDto> MadeByYou { get; set; }
    }
}
