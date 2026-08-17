namespace DynamicReporting.Api.Application.Services;

public class ReportDefinitionService(IUnitOfWork uow) : IReportDefinitionService
{
    public async Task<ReportDefinition> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await uow.Repository<ReportDefinition>().GetByIdAsync(id, cancellationToken) ??
               throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");
    }

    public IEnumerable<ReportDefinition> GetAll()
    {
        return uow.Repository<ReportDefinition>().GetAll();
    }

    public async Task<List<ReportDefinition>> GetAllToListAsync()
    {
        return await uow.Repository<ReportDefinition>().GetAllToListAsync();
    }

    public async Task<ReportDefinition?> GetByPropertyAsync(Expression<Func<ReportDefinition, bool>> predicate, CancellationToken cancellationToken)
    {
        return await uow.Repository<ReportDefinition>().GetByPropertyAsync(predicate, cancellationToken);
    }

    public async Task CreateAsync(ReportDefinitionDto dto)
    {
        await uow.BeginTransactionAsync();

        if (dto.IsDefault)
            await uow.DbContext.ReportDefinitions
                .Where(r => r.IsDefault)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(r => r.IsDefault, false));

        var reportDefinition = dto.Adapt<ReportDefinition>();

        uow.Repository<ReportDefinition>().Add([reportDefinition]);
        await uow.CommitAsync();
    }


    public async Task UpdateAsync(int id, ReportDefinitionDto dto, CancellationToken cancellationToken)
    {
        await uow.BeginTransactionAsync();
        var existing = await GetByIdAsync(id, cancellationToken)
                       ?? throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");

        if (dto.IsDefault && !existing.IsDefault)
            await uow.DbContext.ReportDefinitions
                .Where(r => r.IsDefault)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(r => r.IsDefault, false), cancellationToken: cancellationToken);

        //todo : Here you can use the resolve method to automatically select the base table if you leave the base table empty.
        //if (dto.SelectedColumns != existing.SelectedColumns)
        //    existing.BaseTable = baseTableResolver.Resolve(dto.SelectedColumns);

        dto.Adapt(existing);
        existing.UpdatedAt = DateTime.UtcNow;

        await uow.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await uow.BeginTransactionAsync();

        var repo = uow.Repository<ReportDefinition>();
        var entity = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");
        uow.Repository<ReportDefinition>().Remove([entity]);
        await uow.CommitAsync();
    }

    public async Task SetDefaultAsync(int id)
    {
        await uow.BeginTransactionAsync();

        var repo = uow.Repository<ReportDefinition>();

        var all = await repo.GetAllToListAsync();
        all.ToList().ForEach(x => x.IsDefault = false);

        var item = all.FirstOrDefault(x => x.Id == id)
                   ?? throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");

        item.IsDefault = true;

        await uow.CommitAsync();
    }
}