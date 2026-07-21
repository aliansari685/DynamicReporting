namespace DynamicReporting.Api.Application.Services;

public class ReportDefinitionService(IUnitOfWork uow) : IReportDefinitionService
{
    public async Task<ReportDefinition> GetByIdAsync(int id)
    {
        return await uow.Repository<ReportDefinition>().GetByIdAsync(id) ??
               throw new NullReferenceException("شناسه وجود ندارد");
    }

    public IEnumerable<ReportDefinition> GetAll()
    {
        return uow.Repository<ReportDefinition>().GetAll();
    }

    public async Task<List<ReportDefinition>> GetAllToListAsync()
    {
        return await uow.Repository<ReportDefinition>().GetAllToListAsync();
    }

    public async Task<ReportDefinition?> GetByPropertyAsync(Expression<Func<ReportDefinition, bool>> predicate)
    {
        return await uow.Repository<ReportDefinition>().GetByPropertyAsync(predicate);
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


    public async Task UpdateAsync(int id, ReportDefinitionDto dto)
    {
        await uow.BeginTransactionAsync();
        var existing = await GetByIdAsync(id)
                       ?? throw new NullReferenceException("گزارشی با این شناسه وجود ندارد");

        if (dto.IsDefault && !existing.IsDefault)
            await uow.DbContext.ReportDefinitions
                .Where(r => r.IsDefault)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(r => r.IsDefault, false));

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
        var entity = await repo.GetByIdAsync(id) ?? throw new NullReferenceException("شناسه وجود ندارد");
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
                   ?? throw new NullReferenceException("شناسه وجود ندارد");

        item.IsDefault = true;

        await uow.CommitAsync();
    }
}