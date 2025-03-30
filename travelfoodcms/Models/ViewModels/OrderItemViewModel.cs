using System;
using System.ComponentModel.DataAnnotations;

namespace TravelFoodCms.Models.ViewModels
{
    public class OrderItemViewModel
    {
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Order is required")]
        [Display(Name = "Order")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Item Name is required")]
        [StringLength(100, ErrorMessage = "Item Name cannot be longer than 100 characters")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be positive")]
        [Display(Name = "Unit Price")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Restaurant")]
        public string RestaurantName { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; }

        // Validation method
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ItemName) && 
                   Quantity > 0 && 
                   UnitPrice > 0;
        }

        // Computed property can remain as a method if needed
        public decimal CalculateTotalPrice() => Quantity * UnitPrice;
    }
}