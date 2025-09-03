using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Common;
using System.Net;

namespace RealEstate.Api.Controllers
{
    /// <summary>
    /// Controller for managing real estate properties.
    /// Provides endpoints to create, update, and query properties, including images, price changes, and filtered lists.
    /// </summary>
    [ApiController]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesController"/> class.
        /// </summary>
        /// <param name="service">The property service injected via dependency injection.</param>
        public PropertiesController(IPropertyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Creates a new property.
        /// </summary>
        /// <param name="dto">The data transfer object containing property details to create.</param>
        /// <returns>The created property DTO with a 200 OK response.</returns>
        /// <response code="200">Property created successfully.</response>
        /// <response code="400">Invalid input data (e.g., missing required fields).</response>
        /// <response code="500">Internal server error during creation.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PropertyDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
        {
            return Ok(await _service.CreateAsync(dto));
        }

        /// <summary>
        /// Adds an image to an existing property.
        /// </summary>
        /// <param name="id">The ID of the property to add the image to.</param>
        /// <param name="dto">The data transfer object containing the image details.</param>
        /// <returns>A 204 No Content response if successful.</returns>
        /// <response code="204">Image added successfully.</response>
        /// <response code="404">Property not found.</response>
        /// <response code="400">Invalid image data.</response>
        /// <response code="500">Internal server error during addition.</response>
        [HttpPost("{id}/images")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddImage(int id, [FromBody] AddImageDto dto)
        {
            dto.PropertyId = id;
            await _service.AddImageAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// Changes the price of an existing property.
        /// This operation also adds a trace record for auditing.
        /// </summary>
        /// <param name="id">The ID of the property to update the price for.</param>
        /// <param name="dto">The data transfer object containing the new price.</param>
        /// <returns>A 204 No Content response if successful.</returns>
        /// <response code="204">Price changed successfully.</response>
        /// <response code="404">Property not found.</response>
        /// <response code="400">Invalid price (e.g., negative value).</response>
        /// <response code="500">Internal server error during update.</response>
        [HttpPatch("{id}/price")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangePrice(int id, [FromBody] ChangePriceDto dto)
        {
            dto.Id = id;
            await _service.ChangePriceAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing property's details.
        /// </summary>
        /// <param name="id">The ID of the property to update.</param>
        /// <param name="dto">The data transfer object containing updated property details.</param>
        /// <returns>The updated property DTO with a 200 OK response.</returns>
        /// <response code="200">Property updated successfully.</response>
        /// <response code="404">Property not found.</response>
        /// <response code="400">Invalid update data.</response>
        /// <response code="500">Internal server error during update.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PropertyDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyDto dto)
        {
            return Ok(await _service.UpdateAsync(id, dto));
        }

        /// <summary>
        /// Lists properties with optional filters and pagination.
        /// Supports filtering by name, address, code internal, price range, year range, and owner ID.
        /// </summary>
        /// <param name="filter">Query parameters for filtering and pagination (e.g., ?MinPrice=100000&amp;PageNumber=1).</param>
        /// <returns>A paged result of property DTOs with a 200 OK response.</returns>
        /// <response code="200">Properties listed successfully.</response>
        /// <response code="400">Invalid filter parameters (e.g., MinPrice > MaxPrice).</response>
        /// <response code="500">Internal server error during listing.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PropertyDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> List([FromQuery] PropertyFilter filter)
        {
            return Ok(await _service.ListAsync(filter));
        }

        /// <summary>
        /// Gets all properties owned by a specific owner.
        /// </summary>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <returns>A list of property DTOs owned by the specified owner with a 200 OK response.</returns>
        /// <response code="200">Properties retrieved successfully.</response>
        /// <response code="404">Owner not found or no properties associated.</response>
        /// <response code="500">Internal server error during retrieval.</response>
        [HttpGet("owner/{ownerId}/properties")]
        [ProducesResponseType(typeof(IEnumerable<PropertyDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetByOwner(int ownerId)
        {
            return Ok(await _service.GetPropertiesByOwnerIdAsync(ownerId));
        }

        /// <summary>
        /// Gets detailed information about a specific property, including owner, images, and traces.
        /// </summary>
        /// <param name="id">The ID of the property to retrieve details for.</param>
        /// <returns>The property details DTO with a 200 OK response.</returns>
        /// <response code="200">Property details retrieved successfully.</response>
        /// <response code="404">Property not found.</response>
        /// <response code="500">Internal server error during retrieval.</response>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(PropertyDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDetails(int id)
        {
            return Ok(await _service.GetPropertyDetailsAsync(id));
        }
    }
}