using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/suppliers")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<SupplierResponse>>> GetSuppliers([FromQuery] PageRequest pageRequest)
    {
        var response = await _supplierService.GetSuppliersAsync(pageRequest);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierResponse>> GetSupplier(int id)
    {
        var response = await _supplierService.GetSupplierAsync(id);
        if (response == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.SupplierNotFound);
        }

        return Ok(response);
    }

    [HttpPost("")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SupplierResponse>> CreateSupplier(SupplierRequest supplierRequest)
    {
        var result = await _supplierService.CreateSupplierAsync(supplierRequest);
        if (result.Status == SaveSupplierStatus.DuplicateName)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.SupplierNameDuplicated);
        }

        return CreatedAtAction(nameof(GetSupplier), new { id = result.Supplier!.Id }, result.Supplier);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SupplierResponse>> UpdateSupplier(int id, SupplierRequest supplierRequest)
    {
        var result = await _supplierService.UpdateSupplierAsync(id, supplierRequest);
        if (result.Status == SaveSupplierStatus.NotFound)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.SupplierNotFound);
        }

        if (result.Status == SaveSupplierStatus.DuplicateName)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.SupplierNameDuplicated);
        }

        return Ok(result.Supplier);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveSupplier(int id)
    {
        var response = await _supplierService.RemoveSupplierAsync(id);
        if (response == DeleteSupplierResult.NotFound)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.SupplierNotFound);
        }

        if (response == DeleteSupplierResult.HasProductSuppliers)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.SupplierHasProductSuppliers);
        }

        return NoContent();
    }
}
