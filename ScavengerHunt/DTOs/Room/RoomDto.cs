namespace ScavengerHunt.DTOs
{
    public record struct RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
    }
}