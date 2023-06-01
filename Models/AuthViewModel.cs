using System;
using System.ComponentModel.DataAnnotations;

namespace kinological_club.Models
{
    public class AuthViewModel
    {
        public int Id { get; set; }
        [Required]

        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        public string Role { get; set; }
        public string returnUrl { get; set; }
    }
}
