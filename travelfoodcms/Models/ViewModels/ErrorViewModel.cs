using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelFoodCms.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}