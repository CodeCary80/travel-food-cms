using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelFoodCms.Models
{
    public partial class OrderItem
    {
        [Key]
        public int ItemId { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal UnitPrice { get; set; }
        
        // Navigation property
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}