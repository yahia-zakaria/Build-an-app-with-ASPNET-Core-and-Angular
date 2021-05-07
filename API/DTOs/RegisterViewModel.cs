using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
       // [StringLength(8, MinimumLength = 6)]
        public string Password { get; set; }

    }
}