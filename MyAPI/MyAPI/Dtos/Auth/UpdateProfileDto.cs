using System.ComponentModel.DataAnnotations;

namespace MyAPI.Dtos.Auth
{
    public class UpdateProfileDto
    {
        [Required, MaxLength(100)]
        public string Nickname { get; set; }

        // Nhận file upload
        public IFormFile? AvatarFile { get; set; }

        public DateTime? Dob { get; set; }
    }
}
