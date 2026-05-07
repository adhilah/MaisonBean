using MaisonBean.Application.Addresses.Commands;
using MaisonBean.Application.Addresses.Requests;
using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaisonBean.API.Controllers;

[Authorize]
[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/address")]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;

    public AddressController(
        IAddressRepository repo,
        IUnitOfWork uow,
        IMediator mediator)
    {
        _repo = repo;
        _uow = uow;
        _mediator = mediator;
    }

    // =========================
    // ADD ADDRESS
    // POST: api/address
    // =========================
    [HttpPost]
    public async Task<IActionResult> Add(
        [FromBody] CreateAddressRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var address = new Address
        {
            UserId = userId,
            DeliveryAddress = request.DeliveryAddress,
            City = request.City,
            Phone = request.Phone
        };

        await _repo.AddAsync(address, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new
        {
            addressId = address.Id,
            deliveryAddress = address.DeliveryAddress,
            city = address.City,
            phone = address.Phone
        });
    }

    // =========================
    // GET USER ADDRESSES
    // GET: api/address
    // =========================
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var addresses = await _repo.GetByUserIdAsync(userId, ct);

        return Ok(addresses);
    }

    // =========================
    // UPDATE ADDRESS
    // PUT: api/address/{id}
    // =========================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAddressRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var address = await _repo.GetByIdAsync(id, ct);

        if (address == null)
            return NotFound(new
            {
                message = "Address not found"
            });

        // Prevent updating another user's address
        if (address.UserId != userId)
            return Forbid();

        address.DeliveryAddress = request.DeliveryAddress;
        address.City = request.City;
        address.Phone = request.Phone;

        _repo.Update(address);

        await _uow.SaveChangesAsync(ct);

        return Ok(new
        {
            message = "Address updated successfully"
        });
    }

    // =========================
    // DELETE ADDRESS
    // DELETE: api/address/{id}
    // =========================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _mediator.Send(new DeleteAddressCommand
        {
            Id = id,
            UserId = userId
        }, ct);

        return Ok(new
        {
            message = "Address deleted successfully"
        });
    }
}