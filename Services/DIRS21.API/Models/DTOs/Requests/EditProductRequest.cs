using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DIRS21.API.Models.DTOs.Requests
{
    public class EditProductRequest
    {
        /// <summary>
        /// Unique identifier for product being edited
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id must be provided")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Defines the product type
        /// </summary>
        public ProductCategory? CategoryName { get; set; }

        /// <summary>
        /// Maximum limit of the product category
        /// </summary>
        public int? Capacity { get; set; }

        /// <summary>
        /// Cost per night
        /// </summary>
        public decimal? PricePerNight { get; set; }
    }
}
