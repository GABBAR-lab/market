using MediaService.Application.Common;
using MediaService.Application.DTOs;

namespace MediaService.Application.Interfaces;

public interface IMediaAppService
{
    Task<Result<UploadMediaResponse>> UploadAsync(Guid userId, string category, IReadOnlyList<(Stream Stream, string FileName, long Length)> files, int maxCount);
    Task<Result<IReadOnlyList<MediaAssetResponse>>> GetMyMediaAsync(Guid userId, string? category);
    Task<Result<IReadOnlyList<MediaAssetResponse>>> GetAllMediaAsync(int page, int pageSize);
    Task<Result<bool>> DeleteAsync(Guid id, Guid userId, bool isAdmin);
}
