using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetProducts()
    {
        var response = await _productService.GetProductsAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var response = await _productService.GetProductAsync(id);
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPost("")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductResponse>> CreateProduct(ProductRequest productRequest)
    {
        var response = await _productService.CreateProductAsync(productRequest);
        return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(int id, ProductRequest productRequest)
    {
        var response = await _productService.UpdateProductAsync(id, productRequest);
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveProduct(int id)
    {
        var response = await _productService.RemoveProductAsync(id);
        if (response == DeleteProductResult.NotFound)
        {
            return NotFound();
        }

        if (response == DeleteProductResult.HasSuppliers)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "No se puede eliminar un producto con proveedores asociados");
        }

        return NoContent();
    }
}
