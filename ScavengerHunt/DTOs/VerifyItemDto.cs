using System;
namespace ScavengerHunt.DTOs
{
    public struct VerifyItemDto
    {
        public string GameScoreId { get; set; }
        public string ItemId { get; set; }
        public byte[] Image { get; set; }
    }
}

