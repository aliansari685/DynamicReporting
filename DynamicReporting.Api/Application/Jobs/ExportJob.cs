using DynamicReporting.Api.Domain.Enums;

namespace DynamicReporting.Api.Application.Jobs;

public class ExportJob(
    IJobQueueService jobQueueService,
    IReportGeneratedService generatedService,
    IServiceResolver serviceResolver,
    IReportExportService exportService,
    IReportNotificationService notificationService) : IExportJob
{
    public async Task ExportJobAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn,
        ExportType type, Guid reportGuid, CancellationToken cancellationToken)
    {
        var fullPath = CreateExportFile(type, reportGuid);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None,
            2 * 1024 * 1024, true);

        var service = serviceResolver.GetExportService(type);
        if (await service.ExportAsync(reportDefinitionId, filtersList, fileStream, sortColumn, cancellationToken))
        {
            var fileName = $"report_{reportGuid}";

            var downloadUrl = @$"Exports\{type}\{fileName}{FileTypeNameHelper.GetFileType(type)}";

            var dto = new ReportGenerationUpdateDto
            {
                ReportGuid = reportGuid,
                DownloadUrl = downloadUrl
            };
            await generatedService.UpdateAsync(dto);
        }
        else
        {
            var responseDto = await generatedService.GetByGuidAsync(reportGuid);
            jobQueueService.Delete(responseDto.JobId);
            throw new InvalidDataException("داده ای وجود ندارد");
        }
    }

    public async Task FinalizeExportJobAsync(Guid reportGuid)
    {
        try
        {
            var responseDto = await generatedService.GetByGuidAsync(reportGuid);

            var status = jobQueueService.GetStatusByJobId(responseDto.JobId);

            if (string.Equals(status, nameof(HangfireJobState.Succeeded),
                    StringComparison.CurrentCultureIgnoreCase))
            {
                await EntityUpdateAsync(responseDto.JobId, reportGuid);
                await notificationService.NotifyReportReadyAsync(reportGuid);

                //اگر درخواست خروجی اکسل داد
                if (string.Equals(responseDto.FileType, nameof(ExportType.Excel),
                        StringComparison.CurrentCultureIgnoreCase))
                    await exportService.SetAutoFitColumnsWithPathAsync(responseDto.DownloadUrl ??
                                                                       throw new FileNotFoundException(
                                                                           "فایل پیدا نشد"));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "خطای داخلی");
        }
    }

    public async Task<MemoryStream> ExportDirectAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn, CancellationToken cancellationToken)
    {
        var excelService = serviceResolver.GetExportService(ExportType.Excel);
        var stream = new MemoryStream();
        await excelService.ExportAsync(reportDefinitionId, filtersList, stream, sortColumn, cancellationToken);
        stream.Position = 0;
        return await exportService.SetAutoFitColumnsWithStreamAsync(stream, cancellationToken);
    }


    #region Helper Method

    private async Task EntityUpdateAsync(int jobId, Guid reportGuid)
    {
        var expDateTime = jobQueueService.GetExpireDateTimeByJobId(jobId);
        var dto = new ReportGenerationUpdateDto
        {
            ReportGuid = reportGuid,
            JobId = jobId,
            ExpDateTime = expDateTime
        };
        await generatedService.UpdateAsync(dto);
    }

    private static string CreateExportFile(ExportType type, Guid reportGuid)
    {
        var fileName = $"report_{reportGuid}{FileTypeNameHelper.GetFileType(type)}";
        var directory = Path.Combine(Directory.GetCurrentDirectory(), @$"Exports\{type}");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }

    #endregion
}