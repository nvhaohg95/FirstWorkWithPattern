using Application.Base;
using Domain.Models;

namespace Application.Services
{
    public class FileService: ServiceBase
    {
        private readonly List<FileInfoDTO> _files = new List<FileInfoDTO>
        {
            new FileInfoDTO {
                FileId = "1",
                FileName = "1.ST AN PHÚ.pdf",
                CreatedDate = new DateTime(2025,7,1),
                ModifiedOn = new DateTime(2025,8,10),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                IsDelete = true,
            },
            new FileInfoDTO {
                FileId = "2",
                FileName = "PO 3 SACH.pdf",
                CreatedDate = new DateTime(2025,7,5),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                ModifiedOn = new DateTime(2025,8,9),
                IsDelete = false
            },
            new FileInfoDTO {
                FileId = "3",
                FileName = "13311PO2404523184.pdf",
                CreatedDate = new DateTime(2025,8,1),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                ModifiedOn = null,
                IsDelete = false
            },
            new FileInfoDTO {
                FileId = "4",
                FileName = "PO FARMER.pdf",
                CreatedDate = new DateTime(2025,7,16),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                IsDelete = false,
                ModifiedOn = null
            },
            new FileInfoDTO {
                FileId = "5",
                FileName = "PO GENSHAI.pdf",
                CreatedDate = new DateTime(2025,8,6),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                IsDelete = false,
                ModifiedOn = null
            },
            new FileInfoDTO {
                FileId = "6",
                FileName = "PO ks ibis. HM.pdf",
                CreatedDate = new DateTime(2025,7,30),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                IsDelete = false,
                ModifiedOn = null
            },
            new FileInfoDTO {
                FileId = "7",
                FileName = "PO cty sasco. HM.pdf",
                CreatedDate = new DateTime(2025,7,21),
                FileUrl = "file:///D:/PO/1.ST%20AN%20PH%C3%9A.pdf",
                IsDelete = false,
                ModifiedOn = null
            }
        };

        private readonly Dictionary<string, List<string>> _filePermissions = new Dictionary<string, List<string>>
        {
            { "1", new List<string> { "101", "102", "103" } },
            { "2", new List<string> { "101", "104" } },
            { "3", new List<string> { "102", "103","105" } },
            { "4", new List<string> { "102", "103","105" } },
            { "5", new List<string> { "102", "103","105" } },
            { "6", new List<string> { "102", "103","105" } },
            { "7", new List<string> { "102", "103","105" } }
        };
        public List<FileItemResponse> GetFilesAsync(GetFilesRequest request)
        {
            var query = _files.AsQueryable();

            // Cursor-based pagination logic

            if (request.Direction == "next")
            {
                query = query
                    .Where(f => (f.ModifiedOn ?? f.CreatedDate) > request.Cursor)
                    .OrderBy(f => f.ModifiedOn ?? f.CreatedDate)
                    .ThenByDescending(f => f.FileId);
            }
            else if (request.Direction == "prev")
            {
                query = query
                    .Where(f => (f.ModifiedOn ?? f.CreatedDate) < request.Cursor)
                    .OrderByDescending(f => f.ModifiedOn ?? f.CreatedDate)
                    .ThenBy(f => f.FileId);
            }

            var files = query.Take(request.PageSize).ToList();


            // Nếu direction là prev, cần reverse lại kết quả
            if (request.Direction == "prev")
            {
                files.Reverse();
            }

            var response = files.Select(f => new FileItemResponse
            {
                FileId = f.FileId,
                FileName = f.FileName,
                CreatedDate = f.ModifiedOn ?? f.CreatedDate,
                FileUrl = f.FileUrl,
                IsDelete = f.IsDelete
            }).ToList();

            return response;
        }

        public FileInfoDTO GetFileDetail(string fileId)
        {
            return _files.FirstOrDefault(x => x.FileId == fileId);
        }

        public async Task<FilePermissionResponse?> GetFilePermissionAsync(string externalFileId)
        {
            var file = _files.FirstOrDefault(f => f.FileId == externalFileId);
            if (file == null)
            {
                return null;
            }

            var userIds = _filePermissions.GetValueOrDefault(externalFileId, new List<string>());

            return await Task.FromResult(new FilePermissionResponse
            {
                FileId = file.FileId,
                UserIds = string.Join(";", userIds)
            });
        }
    }
}
