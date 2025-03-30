using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelFoodCms.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string Email { get; set; }

        [Display(Name = "Admin Status")]
        public bool IsAdmin { get; set; }

        // Password properties for registration and updates
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        // Additional properties for view-specific logic
        [Display(Name = "Total Orders")]
        public int TotalOrderCount { get; set; }

        [Display(Name = "Total Spending")]
        [DataType(DataType.Currency)]
        public decimal TotalSpending { get; set; }

        // List of user's orders
        public List<OrderViewModel> Orders { get; set; }

        // Validation method
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Email);
        }

        // Method to check if password is valid during registration or update
        public bool IsPasswordValid()
        {
            return !string.IsNullOrWhiteSpace(NewPassword) && 
                   NewPassword == ConfirmPassword && 
                   NewPassword.Length >= 6;
        }
    }

    // Additional view models for authentication
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel : UserViewModel
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public new string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmation password is required")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public new string ConfirmPassword { get; set; }
    }
}