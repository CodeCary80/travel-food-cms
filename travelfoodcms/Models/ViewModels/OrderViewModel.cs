using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelFoodCms.Models.ViewModels
{
   public class OrderViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Restaurant is required")]
        [Display(Name = "Restaurant")]
        public int RestaurantId { get; set; }

        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Amount must be positive")]
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot be longer than 20 characters")]
        public string Status { get; set; }

        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [Display(Name = "Item Count")]
        public int TotalItemCount { get; set; }

        // List of order items
        [Display(Name = "Order Items")]
        public List<OrderItemViewModel> OrderItems { get; set; }

        // Validation method
        public bool IsValid()
        {
            return RestaurantId > 0 && 
                   UserId > 0 && 
                   TotalAmount >= 0;
        }

        // Optional method to calculate item count if needed
        public int CalculateTotalItemCount()
        {
            return OrderItems?.Sum(item => item.Quantity) ?? 0;
        }
    }
}