using Core.Domain.Enums;
using System;

namespace API.Models.DTOs.Requests
{
    public class AvailabilityRequest
    {
        public ProductCategory ProductCategory { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
