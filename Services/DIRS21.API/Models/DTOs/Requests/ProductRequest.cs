using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DIRS21.API.Models.DTOs.Requests
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
        public int Capacity { get; set; }

        /// <summary>
        /// Cost per night
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "pricePerNight must be specified")]
        public decimal PricePerNight { get; set; }
    }
}
