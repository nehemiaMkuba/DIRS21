﻿using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using DIRS21.API.Models.Common;

namespace DIRS21.API.Filters
{
    public class ModelStateFilter : ActionFilterAttribute
    {        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {

                ResponseObject<object> responseObject = new ResponseObject<object>
                {
                    Status = new ResponseStatus
                    {
                        Code = $"{(int)HttpStatusCode.BadRequest}",
                        Message = string.Join(" | ", context.ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage))
                    }
                };

                context.Result = new BadRequestObjectResult(responseObject);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}
