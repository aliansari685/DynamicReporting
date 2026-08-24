namespace DynamicReporting.Mvc.ViewModels
{
    public class ReportGenerationVm
    {
        public Guid ReportGuid { get; init; }

        public int JobId { get; init; }

        public string Status { get; init; } = string.Empty;

        public int? UserId { get; init; }

        public string? DownloadUrl { get; init; }

        public DateTime ExpDateTime { get; init; }

        public DateTime CreateAt { get; init; }

        public string? FileType { get; init; }

        public int ReportDefinitionId { get; init; }

        public string? ReportDefinitionName { get; set; }

        public string CreateAtDisplay =>
            ToIranDateTime(CreateAt);

        public string ExpDateTimeDisplay =>
            ToIranDateTime(ExpDateTime);

        #region Helper Method
        private static string ToIranDateTime(DateTime dateTime)
        {
            var utcDateTime =
                DateTime.SpecifyKind(
                    dateTime,
                    DateTimeKind.Utc);

            var iranTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    OperatingSystem.IsWindows()
                        ? "Iran Standard Time"
                        : "Asia/Tehran");

            var iranDateTime =
                TimeZoneInfo.ConvertTimeFromUtc(
                    utcDateTime,
                    iranTimeZone);

            var persianCalendar =
                new System.Globalization.PersianCalendar();

            return
                string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}",
                    persianCalendar.GetYear(iranDateTime),
                    persianCalendar.GetMonth(iranDateTime),
                    persianCalendar.GetDayOfMonth(iranDateTime),
                    iranDateTime.Hour,
                    iranDateTime.Minute);
        }
        #endregion
    }
}