﻿namespace MyMultitenantApp.Api.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SlugTenant { get; set; }
    }
}
