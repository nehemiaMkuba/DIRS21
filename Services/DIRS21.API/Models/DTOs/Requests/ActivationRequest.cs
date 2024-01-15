using System;
using System.ComponentModel.DataAnnotations;

using API.Models.DTOs.Enums;

namespace API.Models.DTOs.Requests
{
    /// <summary>
    /// Object to activate a client
    /// </summary>
    public class ActivationRequest
    {
        /// <summary>
        /// ApiKey as provided when registering
        /// </summary>        
        [Required(ErrorMessage = "apiKey must be provided", AllowEmptyStrings = false)]
        public string ApiKey { get; set; }

        /// <summary>
        /// Role to be assigned
        /// </summary>
        [Required(ErrorMessage = "role must be provided", AllowEmptyStrings = false)]
        public AssignableRoles Role { get; set; }
    }

}
