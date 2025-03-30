using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelFoodCms.Models.ViewModels
{
    public class DestinationViewModel
    {
        public int DestinationId { get; set; }

        [Required(ErrorMessage = "Destination Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Destination Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot be longer than 100 characters")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Destination Description")]
        public string Description { get; set; }

        [Display(Name = "Image")]
        [StringLength(255, ErrorMessage = "Image URL cannot be longer than 255 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Creator")]
        public int? CreatorUserId { get; set; }

        // Additional properties for view-specific logic
        [Display(Name = "Number of Restaurants")]
        public int RestaurantCount { get; set; }

        // List of associated restaurants
        public List<RestaurantViewModel> Restaurants { get; set; }

        // Validation method
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && 
                   !string.IsNullOrWhiteSpace(Location);
        }
    }
}