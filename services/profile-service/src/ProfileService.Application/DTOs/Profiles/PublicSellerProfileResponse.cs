namespace ProfileService.Application.DTOs.Profiles;

public record PublicSellerProfileResponse(
    Guid UserId,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    DateTime MemberSince);
