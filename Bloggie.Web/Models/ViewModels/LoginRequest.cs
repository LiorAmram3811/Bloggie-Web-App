﻿namespace Bloggie.Web.Models.ViewModels
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
