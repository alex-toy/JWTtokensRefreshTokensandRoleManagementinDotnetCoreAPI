﻿using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTO
{
    public class ChangePasswordDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
