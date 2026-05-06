using MaisonBean.Application.Addresses.Requests;
using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaisonBean.API.Controllers;
[Authorize(Roles = "Customer")]
[ApiController]
[Route("api/address")]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository _repo;
    private readonly IUnitOfWork _uow;

    public AddressController(IAddressRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

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

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var addresses = await _repo.GetByUserIdAsync(userId!, ct);

        return Ok(addresses);
    }

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
            return NotFound("Address not found");

        
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
}