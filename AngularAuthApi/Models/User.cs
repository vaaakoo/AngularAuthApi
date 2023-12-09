using System.ComponentModel.DataAnnotations;

namespace AngularAuthYtAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        public int? IdNumber { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }

        [Required]
        public string? Email { get; set; }


    }
}
