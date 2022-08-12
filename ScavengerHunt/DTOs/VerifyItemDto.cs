using System;
namespace ScavengerHunt.DTOs
{
    public struct VerifyItemDto
    {
        public Guid GameScoreId { get; set; }
        public Guid ItemId { get; set; }
        public byte[] Image { get; set; }
    }
}

