﻿using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductRequest
    {
        /// <summary>
        /// Defines the product type
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "categoryName must be specified")]
        public ProductCategory CategoryName { get; set; }

        /// <summary>
        /// Maximum limit of the product category
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "capacity must be specified")]
        [Range(1, long.MaxValue, ErrorMessage = "capacity must be greater than 0")]
        public int Capacity { get; set; }

        /// <summary>
        /// Cost per night
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "pricePerNight must be specified")]
        [Range(1, long.MaxValue, ErrorMessage = "capacity must be greater than 0")]
        public decimal PricePerNight { get; set; }
    }
}
