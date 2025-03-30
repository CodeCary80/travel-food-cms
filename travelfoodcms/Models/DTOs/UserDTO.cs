using System;
using System.Collections.Generic;

namespace TravelFoodCms.Models.DTOs
{
   public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; } = false;
        public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
        
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

     public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
    }


}  