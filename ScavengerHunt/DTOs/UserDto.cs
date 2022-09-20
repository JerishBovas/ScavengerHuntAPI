namespace ScavengerHunt.DTOs
{
    public struct UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public int Score { get; set; }
        public int Games { get; set; }
        public int Teams { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
