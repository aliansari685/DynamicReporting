namespace DynamicReporting.Api.Shared.Helper
{
    public class FileTypeNameHelper
    {
        public static string GetContentType(string fileType) =>
            fileType.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

        public static string GetFileType(string type)
        {
            return type.ToLower() switch
            {
                "excel" => ".xlsx",
                "pdf" => ".pdf",
                _ => throw new ArgumentOutOfRangeException(type, "ورودی فایل وجود ندارد")
            };
        }
    }
}
