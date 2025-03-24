using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code has to be a minimum of three characters")]
        [MaxLength(3, ErrorMessage = "Code has to be a maximum of three characters")]
        public string Code { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Name can at max 100 character in length")]
        public string Name { get; set; }

        public string? RegionImageUrl { get; set; }
    }
}
