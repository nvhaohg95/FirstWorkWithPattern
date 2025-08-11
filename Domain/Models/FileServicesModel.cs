namespace Domain.Models
{
    public class FileItemResponse
    {
        public string FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
    }

    public class FilePermissionResponse
    {
        public string FileId { get; set; }
        public string UserIds { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
    }

    public class FileInfoDTO
    {
        public string FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string ExternalFileId { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
    }

    public class GetFilesRequest
    {
        public DateTime Cursor { get; set; }
        public int? FileId { get; set; }
        public int PageSize { get; set; } = 20;
        public string? Direction { get; set; } = "next"; // "next" or "prev"
    }
}
