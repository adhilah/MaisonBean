using MaisonBean.Domain.Entities;

public interface IMilkOptionRepository
{
    Task<MilkOption?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<MilkOption>> GetAllAsync(CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task AddAsync(MilkOption entity, CancellationToken ct);
    void Update(MilkOption entity);
    void Delete(MilkOption entity);
}