namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(
    IJobQueueService jobQueueService,
    IReportValidation reportValidation,
    IReportGeneratedService generatedService,
    IReportMetadataService metadataService) : IExportBackgroundJobService
{
    public async Task<Guid> ExportInBackground(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn, ServiceResolver.ExportType type, CancellationToken cancellationToken)
    {
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
                FileType = type.ToString()
            };

            if (!await generatedService.CreateAsync(generation))
                throw new InvalidOperationException("عملیات با خطا مواجه شد");

            jobQueueService.ContinueJob<IExportJob>(exportInBackgroundJobId,
                x => x.FinalizeExportJobAsync(reportGuid));

            return reportGuid;
        }
        catch (Exception)
        {
            jobQueueService.Delete(exportInBackgroundJobId);
            throw;
        }
    }
}