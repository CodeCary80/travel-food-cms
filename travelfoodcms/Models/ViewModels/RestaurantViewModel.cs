using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelFoodCms.Models.ViewModels
{
    public class RestaurantViewModel
    {
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        [Display(Name = "Destination")]
        public int DestinationId { get; set; }

        [Required(ErrorMessage = "Restaurant Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Restaurant Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Cuisine Type is required")]
        [StringLength(50, ErrorMessage = "Cuisine Type cannot be longer than 50 characters")]
        [Display(Name = "Cuisine Type")]
        public string CuisineType { get; set; }

        [Required(ErrorMessage = "Price Range is required")]
        [StringLength(10, ErrorMessage = "Price Range cannot be longer than 10 characters")]
        [Display(Name = "Price Range")]
        public string PriceRange { get; set; }

        [Required(ErrorMessage = "Contact Information is required")]
        [StringLength(100, ErrorMessage = "Contact Information cannot be longer than 100 characters")]
        [Display(Name = "Contact Info")]
        public string ContactInfo { get; set; }

        [Required(ErrorMessage = "Operating Hours are required")]
        [StringLength(100, ErrorMessage = "Operating Hours cannot be longer than 100 characters")]
        [Display(Name = "Operating Hours")]
        public string OperatingHours { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255, ErrorMessage = "Address cannot be longer than 255 characters")]
        public string Address { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
        
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Destination Name")]
        public string DestinationName { get; set; }

        [Display(Name = "Total Orders")]
        public int TotalOrders { get; set; }

        // List of associated orders
        public List<OrderViewModel> Orders { get; set; }

        // Validation method
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && 
                   !string.IsNullOrWhiteSpace(CuisineType);
        }
    }
}