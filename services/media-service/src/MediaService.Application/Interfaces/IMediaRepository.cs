using MediaService.Application.DTOs;

namespace MediaService.Application.Interfaces;

public interface IMediaRepository
{
    Task<Guid> CreateAsync(Guid ownerUserId, string category, string fileName, string url, string contentType, long sizeBytes);
    Task<MediaAssetResponse?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<MediaAssetResponse>> GetByOwnerAsync(Guid ownerUserId, string? category = null);
    Task<IReadOnlyList<MediaAssetResponse>> GetAllAsync(int page = 1, int pageSize = 50);
    Task<bool> DeleteAsync(Guid id, Guid ownerUserId, bool isAdmin);
    Task<int> GetSettingIntAsync(string key, int defaultValue);
}
