using MediaService.Application.Common;
using MediaService.Application.DTOs;
using MediaService.Application.Interfaces;

namespace MediaService.Application.Services;

public class MediaAppService : IMediaAppService
{
    private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IMediaRepository _repository;
    private readonly IMediaStorage _storage;

    public MediaAppService(IMediaRepository repository, IMediaStorage storage)
    {
        _repository = repository;
        _storage = storage;
    }

    public async Task<Result<UploadMediaResponse>> UploadAsync(
        Guid userId,
        string category,
        IReadOnlyList<(Stream Stream, string FileName, long Length)> files,
        int maxCount)
    {
        if (files.Count == 0)
        {
            return Result<UploadMediaResponse>.Failure("No files uploaded.");
        }

        if (files.Count > maxCount)
        {
            return Result<UploadMediaResponse>.Failure($"Maximum {maxCount} files allowed per upload.");
        }

        var urls = new List<string>();
        var assets = new List<MediaAssetResponse>();

        foreach (var (stream, fileName, length) in files)
        {
            await using (stream)
            {
                if (length == 0)
                {
                    continue;
                }

                var ext = Path.GetExtension(fileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    return Result<UploadMediaResponse>.Failure("Only JPG, JPEG, PNG, and WEBP files are allowed.");
                }

                var saved = await _storage.SaveAsync(stream, category, ext);
                var id = await _repository.CreateAsync(
                    userId,
                    category,
                    saved.FileName,
                    saved.Url,
                    "image/webp",
                    saved.SizeBytes);

                var asset = await _repository.GetByIdAsync(id);
                if (asset is not null)
                {
                    assets.Add(asset);
                }

                urls.Add(saved.Url);
            }
        }

        return Result<UploadMediaResponse>.Success(new UploadMediaResponse(urls, assets));
    }

    public async Task<Result<IReadOnlyList<MediaAssetResponse>>> GetMyMediaAsync(Guid userId, string? category)
    {
        var items = await _repository.GetByOwnerAsync(userId, category);
        return Result<IReadOnlyList<MediaAssetResponse>>.Success(items);
    }

    public async Task<Result<IReadOnlyList<MediaAssetResponse>>> GetAllMediaAsync(int page, int pageSize)
    {
        var items = await _repository.GetAllAsync(page, pageSize);
        return Result<IReadOnlyList<MediaAssetResponse>>.Success(items);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var asset = await _repository.GetByIdAsync(id);
        if (asset is null)
        {
            return Result<bool>.Failure("Media not found.");
        }

        var deleted = await _repository.DeleteAsync(id, userId, isAdmin);
        if (!deleted)
        {
            return Result<bool>.Failure("Not authorized to delete this file.");
        }

        await _storage.DeleteFileAsync(asset.Category, asset.FileName);
        return Result<bool>.Success(true);
    }
}
