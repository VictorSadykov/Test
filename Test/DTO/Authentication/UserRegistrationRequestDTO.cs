﻿using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Authentication
{
    public class UserRegistrationRequestDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}