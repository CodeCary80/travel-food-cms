using System;

namespace TravelFoodCms.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public int DestinationCount { get; set; }
        public int RestaurantCount { get; set; }
        public int OrderCount { get; set; }
        public int UserCount { get; set; }
        public int OrderItemCount { get; set; }
    }
}