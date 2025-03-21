using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelFoodCms.Models
{
    public partial class Restaurant
    {
        [Key]
        public int RestaurantId { get; set; }
        
        [Required]
        public int DestinationId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(50)]
        public string CuisineType { get; set; }
        
        [StringLength(10)]
        public string PriceRange { get; set; }
        
        [StringLength(100)]
        public string ContactInfo { get; set; }
        
        [StringLength(100)]
        public string OperatingHours { get; set; }
        
        [StringLength(255)]
        public string Address { get; set; }
        
        [StringLength(255)]
        public string ImageUrl { get; set; }
        
        public DateTime Date { get; set; }
        
        // Navigation properties
        [ForeignKey("DestinationId")]
        public virtual Destination Destination { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; }
    }
}