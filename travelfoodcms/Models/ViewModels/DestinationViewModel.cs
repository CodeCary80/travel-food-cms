using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
    public string? ImageUrl { get; set; }

    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; } 

    [Display(Name = "Created Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    [Display(Name = "Creator")]
    public int? CreatorUserId { get; set; }

    [Display(Name = "Number of Restaurants")]
    public int RestaurantCount { get; set; }

    public List<RestaurantViewModel> Restaurants { get; set; } = new List<RestaurantViewModel>();
    }
}