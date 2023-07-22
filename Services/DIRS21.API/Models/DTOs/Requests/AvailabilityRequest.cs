using Core.Domain.Enums;
using System;

namespace DIRS21.API.Models.DTOs.Requests
{
    public class AvailabilityRequest
    {
        public ProductCategory ProductCategory { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
