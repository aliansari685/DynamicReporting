namespace DynamicReporting.Mvc.ViewModels
{
    public sealed class ReportGenerationVm
    {
        public Guid ReportGuid { get; init; }

        public int JobId { get; init; }

        public string Status { get; init; } = string.Empty;

        public int? UserId { get; init; }

        public string? DownloadUrl { get; init; }

        public DateTime ExpDateTime { get; init; }

        public DateTime CreateAt { get; init; }

        public string? FileType { get; init; }
    }
}