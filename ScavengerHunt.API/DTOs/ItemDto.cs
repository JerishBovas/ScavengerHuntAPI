namespace ScavengerHunt.API.DTOs
{
    public record struct ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemLocation { get; set; }
        public string ImageName { get; set; }
        public int RoomId { get; set; }
    }
}