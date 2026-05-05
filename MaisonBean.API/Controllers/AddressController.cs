using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaisonBean.API.Controllers;
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
    public async Task<IActionResult> Add(Address request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var address = new Address
        {
            UserId = userId!,
            DeliveryAddress = request.DeliveryAddress,
            City = request.City,
            Phone = request.Phone
        };

        await _repo.AddAsync(address, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(address);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var addresses = await _repo.GetByUserIdAsync(userId!, ct);

        return Ok(addresses);
    }
}