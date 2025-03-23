using System;
using System.Collections.Generic;

namespace TravelFoodCms.Models.DTOs
{
    public class DestinationDTO
    {
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public List<RestaurantDTO> Restaurants { get; set; }
    }

}  