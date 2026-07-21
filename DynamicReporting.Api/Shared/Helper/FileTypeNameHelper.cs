namespace DynamicReporting.Api.Shared.Helper;

public class FileTypeNameHelper
{
    /// <summary>
    ///     دریافت نوع محتوا برای دانلود یا استریم
    /// </summary>
    /// <param name="fileType">نوع فایل</param>
    /// <returns></returns>
    public static string GetContentType(string fileType)
    {
        return fileType.ToLower() switch
        {
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "pdf" => "application/pdf",
            "csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    ///     دریافت پسوند فایل
    /// </summary>
    /// <param name="type">نوع فایل</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GetFileType(ServiceResolver.ExportType type)
    {
        return type.ToString().ToLower() switch
        {
            "excel" => ".xlsx",
            "pdf" => ".pdf",
            _ => throw new ArgumentOutOfRangeException(type.ToString(), "ورودی فایل وجود ندارد")
        };
    }
}