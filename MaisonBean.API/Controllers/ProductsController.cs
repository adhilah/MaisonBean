using MaisonBean.Application.Products.Commands;
using MaisonBean.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaisonBean.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    // POST /api/products
    [Authorize(Roles = "Admin")]
    [HttpPost("product/ad")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    // PUT /api/products/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}update/ad")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand cmd)
    {
        cmd.Id = id;
        await _mediator.Send(cmd);
        return NoContent();
    }

    //toggle
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/block/ad")]
    public async Task<IActionResult> Toggle(int id)
    {
        var isBlocked = await _mediator.Send(new ToggleProductCommand(id));

        return Ok(new
        {
            message = isBlocked
                ? "Product successfully blocked"
                : "Product successfully unblocked"
        });
    }

    // DELETE 
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}/ad")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteProductCommand { Id = id });
        if (!result) return NotFound($"Product {id} not found");
        return NoContent();
    }

    // GET
    [HttpGet("{id:int}")] 
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        if (result is null)
            return NotFound($"Product {id} not found");
        return Ok(result);
    }

    // GET - category
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var result = await _mediator.Send(new GetProductsByCategoryQuery(category));
        if (!result.Any()) return NotFound($"No products found in category '{category}'.");
        return Ok(result);
    }

    // GET /api/products
    [HttpGet] 
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(), ct);
        if (!result.Any()) return NotFound("No products found.");
        return Ok(result);
    }
}