using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelFoodCms.Models
{
    public partial class Destination
    {
        [Key]
        public int DestinationId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Location { get; set; }
        
        public string Description { get; set; }
        
        [StringLength(255)]
        public string ImageUrl { get; set; }
        
        public DateTime Date { get; set; }

        public int? CreatorUserId { get; set; }
        // Navigation property
        public virtual ICollection<Restaurant> Restaurants { get; set; }

        [ForeignKey("CreatorUserId")]
        public virtual User Creator { get; set; }
    }
}
