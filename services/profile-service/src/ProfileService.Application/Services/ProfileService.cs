using ProfileService.Application.Common;
using ProfileService.Application.DTOs.Profiles;
using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;
using ProfileService.Domain.Enums;

namespace ProfileService.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<Result<ProfileResponse>> CreateAsync(CreateProfileRequest request)
    {
        if (await _profileRepository.UserIdExistsAsync(request.UserId))
        {
            return Result<ProfileResponse>.Failure("Profile already exists for this user.");
        }

        if (!TryParseGender(request.Gender, out var gender, out var genderError))
        {
            return Result<ProfileResponse>.Failure(genderError!);
        }

        var profile = UserProfile.Create(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Bio,
            request.AvatarUrl,
            request.DateOfBirth,
            gender,
            request.PhoneNumber,
            request.Website,
            request.Language,
            request.Currency,
            request.Timezone,
            request.EmailNotifications,
            request.SmsNotifications);

        await _profileRepository.AddAsync(profile);

        return Result<ProfileResponse>.Success(MapToResponse(profile));
    }

    public async Task<Result<ProfileResponse>> GetByIdAsync(Guid id)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<ProfileResponse>.Failure("Profile not found.");
        }

        return Result<ProfileResponse>.Success(MapToResponse(profile));
    }

    public async Task<Result<ProfileResponse>> GetByUserIdAsync(Guid userId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<ProfileResponse>.Failure("Profile not found.");
        }

        return Result<ProfileResponse>.Success(MapToResponse(profile));
    }

    public async Task<Result<PublicSellerProfileResponse>> GetPublicSellerProfileAsync(Guid userId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        if (profile is null || profile.Status != ProfileStatus.Active)
        {
            return Result<PublicSellerProfileResponse>.Failure("Seller profile not found.");
        }

        return Result<PublicSellerProfileResponse>.Success(new PublicSellerProfileResponse(
            profile.UserId,
            $"{profile.FirstName} {profile.LastName}".Trim(),
            profile.AvatarUrl,
            profile.Bio,
            profile.CreatedAt));
    }

    public async Task<Result<IReadOnlyList<ProfileResponse>>> GetAllAsync()
    {
        var profiles = await _profileRepository.GetAllAsync();
        var activeProfiles = profiles
            .Where(p => p.Status != ProfileStatus.Deleted)
            .Select(MapToResponse)
            .ToList();

        return Result<IReadOnlyList<ProfileResponse>>.Success(activeProfiles);
    }

    public async Task<Result<ProfileResponse>> UpdateAsync(Guid id, UpdateProfileRequest request)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<ProfileResponse>.Failure("Profile not found.");
        }

        if (!TryParseGender(request.Gender, out var gender, out var genderError))
        {
            return Result<ProfileResponse>.Failure(genderError!);
        }

        profile.Update(
            request.FirstName,
            request.LastName,
            request.Bio,
            request.AvatarUrl,
            request.DateOfBirth,
            gender,
            request.PhoneNumber,
            request.Website,
            request.Language,
            request.Currency,
            request.Timezone,
            request.EmailNotifications,
            request.SmsNotifications);

        await _profileRepository.UpdateAsync(profile);

        return Result<ProfileResponse>.Success(MapToResponse(profile));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<bool>.Failure("Profile not found.");
        }

        profile.SoftDelete();
        await _profileRepository.UpdateAsync(profile);

        return Result<bool>.Success(true);
    }

    public async Task<Result<ProfileResponse>> ActivateAsync(Guid id)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<ProfileResponse>.Failure("Profile not found.");
        }

        profile.Activate();
        await _profileRepository.UpdateAsync(profile);

        return Result<ProfileResponse>.Success(MapToResponse(profile));
    }

    public async Task<Result<ProfileResponse>> DeactivateAsync(Guid id)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<ProfileResponse>.Failure("Profile not found.");
        }

        profile.Deactivate();
        await _profileRepository.UpdateAsync(profile);

        return Result<ProfileResponse>.Success(MapToResponse(profile));
    }

    public async Task<Result<AddressResponse>> AddAddressAsync(Guid profileId, CreateAddressRequest request)
    {
        var profile = await _profileRepository.GetByIdAsync(profileId);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<AddressResponse>.Failure("Profile not found.");
        }

        var address = profile.AddAddress(
            request.Label,
            request.StreetLine1,
            request.StreetLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            request.IsDefault);

        await _profileRepository.UpdateAsync(profile);

        return Result<AddressResponse>.Success(MapAddress(address));
    }

    public async Task<Result<AddressResponse>> UpdateAddressAsync(
        Guid profileId,
        Guid addressId,
        UpdateAddressRequest request)
    {
        var profile = await _profileRepository.GetByIdAsync(profileId);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<AddressResponse>.Failure("Profile not found.");
        }

        try
        {
            profile.UpdateAddress(
                addressId,
                request.Label,
                request.StreetLine1,
                request.StreetLine2,
                request.City,
                request.State,
                request.Country,
                request.PostalCode,
                request.IsDefault);
        }
        catch (InvalidOperationException)
        {
            return Result<AddressResponse>.Failure("Address not found.");
        }

        await _profileRepository.UpdateAsync(profile);

        var address = profile.Addresses.First(a => a.Id == addressId);
        return Result<AddressResponse>.Success(MapAddress(address));
    }

    public async Task<Result<bool>> RemoveAddressAsync(Guid profileId, Guid addressId)
    {
        var profile = await _profileRepository.GetByIdAsync(profileId);
        if (profile is null || profile.Status == ProfileStatus.Deleted)
        {
            return Result<bool>.Failure("Profile not found.");
        }

        try
        {
            profile.RemoveAddress(addressId);
        }
        catch (InvalidOperationException)
        {
            return Result<bool>.Failure("Address not found.");
        }

        await _profileRepository.UpdateAsync(profile);

        return Result<bool>.Success(true);
    }

    private static bool TryParseGender(string? value, out Gender? gender, out string? error)
    {
        gender = null;
        error = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        if (Enum.TryParse<Gender>(value, ignoreCase: true, out var parsed))
        {
            gender = parsed;
            return true;
        }

        error = "Invalid gender value.";
        return false;
    }

    private static ProfileResponse MapToResponse(UserProfile profile) => new(
        profile.Id,
        profile.UserId,
        profile.FirstName,
        profile.LastName,
        profile.Bio,
        profile.AvatarUrl,
        profile.DateOfBirth,
        profile.Gender?.ToString(),
        profile.PhoneNumber,
        profile.Website,
        profile.Language,
        profile.Currency,
        profile.Timezone,
        profile.EmailNotifications,
        profile.SmsNotifications,
        profile.Status.ToString(),
        profile.CreatedAt,
        profile.UpdatedAt,
        profile.Addresses.Select(MapAddress).ToList());

    private static AddressResponse MapAddress(UserAddress address) => new(
        address.Id,
        address.Label,
        address.StreetLine1,
        address.StreetLine2,
        address.City,
        address.State,
        address.Country,
        address.PostalCode,
        address.IsDefault,
        address.CreatedAt);
}
