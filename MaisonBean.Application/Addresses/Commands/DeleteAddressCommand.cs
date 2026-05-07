using MaisonBean.Application.Interfaces;
using MediatR;


namespace MaisonBean.Application.Addresses.Commands
{
    public class DeleteAddressCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
    }


    public class DeleteAddressHandler
     : IRequestHandler<DeleteAddressCommand, Unit>
    {
        private readonly IAddressRepository _repo;
        private readonly IUnitOfWork _uow;

        public DeleteAddressHandler(
            IAddressRepository repo,
            IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<Unit> Handle(
            DeleteAddressCommand request,
            CancellationToken ct)
        {
            var address = await _repo.GetByIdAsync(request.Id, ct);

            if (address == null)
                throw new Exception("Address not found");

            if (address.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            _repo.Delete(address);

            await _uow.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
