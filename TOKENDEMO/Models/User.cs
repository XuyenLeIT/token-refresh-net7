using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TOKENDEMO.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "email is required")]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required(ErrorMessage ="name is required")]
        public string Password { get; set; }
        public string Role { get; set; }
        public string ? RefreshToken { get; set; }
        public DateTime ? RefreshTokenExpiryTime { get; set; }
    }
}
