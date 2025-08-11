using Application.Interfaces;
using Application.Services;
using Domain.Models;
using FirstWorkWithPattern.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FirstWorkWithPattern.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class FileController : BaseController<FileController>
    {
        FileService _fileService;
        public FileController(ILogService<FileController> log, FileService fileService) : base(log)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Lấy danh sách file với phân trang cursor-based
        /// </summary>
        /// <param name="createdDate">Ngày tạo file (cursor)</param>
        /// <param name="pageSize">Số lượng record trên 1 trang (mặc định: 20)</param>
        /// <param name="direction">Hướng phân trang: next (tiếp theo) | prev (trước đó)</param>
        /// <returns>Danh sách file với thông tin phân trang</returns>
        [HttpGet("getfiles")]
        public ActionResult<ApiResponse<List<FileItemResponse>>> GetFiles(
            [FromQuery] DateTime cursor,
            [FromQuery] int? fileId,
            [FromQuery, Range(1, 100)] int pageSize = 20,
            [FromQuery] string direction = "next")
        {
            try
            {
                var request = new GetFilesRequest
                {
                    Cursor = cursor,
                    FileId = fileId,
                    PageSize = pageSize,
                    Direction = direction
                };

                var result = _fileService.GetFilesAsync(request);

                return Ok(new ApiResponse<List<FileItemResponse>>
                {
                    Succeeded = true,
                    Message = "Lấy danh sách file thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<FileItemResponse>>
                {
                    Succeeded = false,
                    Message = "Lỗi hệ thống",
                    ErrorCode = "INTERNAL_ERROR"
                });
            }
        }

        /// <summary>
        /// Lấy quyền truy cập file theo External File ID
        /// </summary>
        /// <param name="externalFileId">External File ID</param>
        /// <returns>Thông tin quyền truy cập file</returns>
        [HttpGet("permissions/{fileId}")]
        public async Task<ActionResult<ApiResponse<FilePermissionResponse>>> GetFilePermission(
            [FromRoute, Required] string fileId)
        {
            try
            {
                var result = await _fileService.GetFilePermissionAsync(fileId);

                if (result == null)
                {
                    return NotFound(new ApiResponse<FilePermissionResponse>
                    {
                        Succeeded = false,
                        Message = "Không tìm thấy file",
                        ErrorCode = "FILE_NOT_FOUND"
                    });
                }

                return Ok(new ApiResponse<FilePermissionResponse>
                {
                    Succeeded = true,
                    Message = "Lấy quyền truy cập file thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FilePermissionResponse>
                {
                    Succeeded = false,
                    Message = "Lỗi hệ thống",
                    ErrorCode = "INTERNAL_ERROR"
                });
            }
        }

        [HttpGet("{fileId}")]
        public IActionResult GetFile(string fileId)
        {
            var file = _fileService.GetFileDetail(fileId);
            if (file == null)
            {
                return NotFound(new ApiResponse<FileInfoDTO>
                {
                    Succeeded = false,
                    Message = "File not found",
                    ErrorCode = "FILE_NOT_FOUND"
                });
            }

            var filePath = Path.Combine("D:\\PO", file.FileName); // đường dẫn local
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream"; // hoặc dùng MimeMapping nếu cần chính xác
            return File(fileBytes, contentType, fileId);
        }
    }
}
