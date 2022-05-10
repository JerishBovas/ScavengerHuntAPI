namespace ScavengerHunt_API.Models
{
    public record Location
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string description { get; init; }

        public string address { get; init; }

        public Coordinate coordinates { get; init; }
    }
}
