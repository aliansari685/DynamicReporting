namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(
    IJobQueueService jobQueueService,
    IReportValidation reportValidation,
    IReportGeneratedService generatedService,
    IServiceResolver serviceProvider,
    IExportJob exportJob,
    IReportMetadataService metadataService) : IExportBackgroundJobService
{
    public async Task<Guid> ExportInBackgroundAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn, ExportType type, CancellationToken cancellationToken)
    {
        serviceProvider.GetExportService(type);

        var reportGuid = Guid.NewGuid();
        var exportInBackgroundJobId = 0;
        try
        {
            var report = await metadataService.GetReportDefinitionAsync(reportDefinitionId);

            if (filtersList != null)
                reportValidation.ValidateFilteringColumn(report, filtersList);

            reportValidation.ValidateSortColumn(report, sortColumn);

            var jobIdString = jobQueueService.Enqueue<IExportJob>(x =>
                x.ExportJobAsync(reportDefinitionId, filtersList, sortColumn, type, reportGuid, cancellationToken));

            exportInBackgroundJobId = int.Parse(jobIdString);

            var generation = new ReportGenerationRequestDto
            {
                ReportGuid = reportGuid,
                JobId = exportInBackgroundJobId,
                FileType = type.ToString(),
                ReportDefinitionId = reportDefinitionId
            };

            if (!await generatedService.CreateAsync(generation))
                throw new InvalidOperationException("عملیات با خطا مواجه شد");

            jobQueueService.ContinueJob<IExportJob>(exportInBackgroundJobId,
                x => x.FinalizeExportJobAsync(reportGuid, cancellationToken));

            return reportGuid;
        }
        catch (Exception)
        {
            jobQueueService.Delete(exportInBackgroundJobId);
            throw;
        }
    }

    public async Task<MemoryStream> ExportDirectAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn, CancellationToken cancellationToken)
    {
        var report = await metadataService.GetReportDefinitionAsync(reportDefinitionId);

        if (filtersList != null)
            reportValidation.ValidateFilteringColumn(report, filtersList);

        reportValidation.ValidateSortColumn(report, sortColumn);

        return await exportJob.ExportDirectAsync(reportDefinitionId, filtersList, sortColumn, cancellationToken);
    }
}