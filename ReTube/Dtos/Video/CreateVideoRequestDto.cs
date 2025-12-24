using System.ComponentModel.DataAnnotations;

namespace ReTube.Dtos.Video
{
    public class CreateVideoRequestDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be 5 characters")]
        [MaxLength(50, ErrorMessage = "Title cannot be over 50 characters")]
        public string Title { get; set; }
        [MinLength(5, ErrorMessage = "Description must be 5 characters")]
        [MaxLength(280, ErrorMessage = "Description cannot be over 280 characters")]
        public string? Description { get; set; }
        [Required]
        public IFormFile? Image { get; set; }
        public string? VideoFile { get; set; }
    }
}