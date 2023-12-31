﻿using System.ComponentModel.DataAnnotations;

namespace AngularAuthYtAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string Role { get; set; } = "client";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IdNumber { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl {  get; set; }
        public string? CvUrl { get; set; }
        public bool IsAdmin { get; set; } = false;

    }
}
