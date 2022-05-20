namespace ScavengerHunt.API.DTOs
{
    public record struct RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public int LocationId { get; set; }
        public ICollection<ItemDto> Items { get; set; }
    }
}