namespace MediaService.Application.DTOs;

public record MediaAssetResponse(
    Guid Id,
    Guid OwnerUserId,
    string Category,
    string FileName,
    string Url,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAt);

public record UploadMediaResponse(IReadOnlyList<string> Urls, IReadOnlyList<MediaAssetResponse> Assets);
