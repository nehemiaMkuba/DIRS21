using System;
using System.Net;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

using AutoMapper;

using Core.Domain.Enums;
using Core.Domain.Entities;
using API.Models.Common;
using Core.Management.Interfaces;
using API.Attributes;
using API.Models.DTOs.Requests;
using API.Models.DTOs.Responses;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [Route("v{version:apiVersion}/products"), SwaggerOrder("B")]
    [Authorize(Policy = nameof(AuthPolicy.GlobalRights))]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        public ProductsController(IMapper mapper,
            IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Create a product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("create")]
        [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProduct([FromBody, Required] ProductRequest request)
        {
            return Created(string.Empty, _mapper.Map<ProductDto>(await _productRepository.CreateProduct(request.CategoryName, request.Capacity, request.PricePerNight)));
        }

        /// <summary>
        /// Get availabilty of a product category between date range
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("availability")]
        [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> Availabity([FromBody, Required] AvailabilityRequest request)
        {
            return Ok(await _productRepository.Availability(request.ProductCategory, request.StartAt, request.EndAt));
        }

        /// <summary>
        /// Edit products
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut, Route("edit")]
        [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> EditProduct([FromBody, Required] EditProductRequest request)
        {
            return Ok(await _productRepository.EditProduct(request.Id, request.CategoryName, request.Capacity, request.PricePerNight));
        }


        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProduct([FromRoute, Required] string id)
        {
            return Ok(_mapper.Map<ProductDto>(await _productRepository.ValidateFindOneAsync(id)));
        }

        /// <summary>
        /// Delete product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProduct([FromRoute, Required] string id)
        {
            return Ok(await _productRepository.DeleteByIdAsync(id));
        }

        /// <summary>
        /// Return paginated list of products its uses cursor based pagination where users should specify records between certain range
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="capacity"></param>
        /// <param name="pricePerNight"></param>
        /// <param name="startingAfterProductId"></param>
        /// <param name="endingBeforeProductId"></param>
        /// <returns></returns>
        [HttpGet, Route("list"), AllowAnonymous]
        [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseObject<ProductDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductCategory? categoryName, [FromQuery] int? capacity, [FromQuery] decimal? pricePerNight, [FromQuery] string startingAfterProductId, [FromQuery] string endingBeforeProductId)
        {
            (IEnumerable<Product> products, int totalCount) = await _productRepository.GetProductList(categoryName, capacity, pricePerNight, startingAfterProductId, endingBeforeProductId);
            return Ok(new ResponseObject<ProductDto>
            {
                Data = _mapper.Map<IEnumerable<ProductDto>>(products),
                Paging = new Paging
                {
                    Count = totalCount
                }

            });
        }
    }
}
