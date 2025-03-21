using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelFoodCms.Models
{
    public partial class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        [Required]
        public int RestaurantId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "pending";
        
        public string SpecialRequests { get; set; }
        
        // Navigation properties
        [ForeignKey("RestaurantId")]
        public virtual Restaurant Restaurant { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}