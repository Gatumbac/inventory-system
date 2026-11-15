using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products/{productId}/suppliers")]
public class ProductSupplierController : ControllerBase
{
    private readonly IProductSupplierService _productSupplierService;

    public ProductSupplierController(IProductSupplierService productSupplierService)
    {
        _productSupplierService = productSupplierService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProductSupplierResponse>>> GetProductSuppliers(int productId)
    {
        var response = await _productSupplierService.GetProductSuppliersAsync(productId);
        if (response == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.ProductNotFound);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductSupplierResponse>> GetProductSupplier(int productId, int id)
    {
        var response = await _productSupplierService.GetProductSupplierAsync(productId, id);
        if (response == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.ProductSupplierNotFound);
        }

        return Ok(response);
    }

    [HttpPost("")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductSupplierResponse>> CreateProductSupplier(
        int productId,
        CreateProductSupplierRequest request)
    {
        var result = await _productSupplierService.CreateProductSupplierAsync(productId, request);
        if (result.Status != SaveProductSupplierStatus.Saved)
        {
            return ToFailure(result.Status);
        }

        var routeValues = new { productId, id = result.ProductSupplier!.Id };
        return CreatedAtAction(nameof(GetProductSupplier), routeValues, result.ProductSupplier);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductSupplierResponse>> UpdateProductSupplier(
        int productId,
        int id,
        ProductSupplierRequest request)
    {
        var result = await _productSupplierService.UpdateProductSupplierAsync(productId, id, request);
        if (result.Status != SaveProductSupplierStatus.Saved)
        {
            return ToFailure(result.Status);
        }

        return Ok(result.ProductSupplier);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveProductSupplier(int productId, int id)
    {
        var response = await _productSupplierService.RemoveProductSupplierAsync(productId, id);
        if (response == DeleteProductSupplierResult.NotFound)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.ProductSupplierNotFound);
        }

        return NoContent();
    }

    private ObjectResult ToFailure(SaveProductSupplierStatus status)
    {
        switch (status)
        {
            case SaveProductSupplierStatus.ProductNotFound:
                return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.ProductNotFound);
            case SaveProductSupplierStatus.SupplierNotFound:
                return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.SupplierNotFound);
            case SaveProductSupplierStatus.DuplicateLot:
                return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.ProductSupplierLotDuplicated);
            default:
                return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.ProductSupplierNotFound);
        }
    }
}
