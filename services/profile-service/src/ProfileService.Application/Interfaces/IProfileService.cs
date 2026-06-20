using ProfileService.Application.Common;
using ProfileService.Application.DTOs.Profiles;

namespace ProfileService.Application.Interfaces;

public interface IProfileService
{
    Task<Result<ProfileResponse>> CreateAsync(CreateProfileRequest request);
    Task<Result<ProfileResponse>> GetByIdAsync(Guid id);
    Task<Result<ProfileResponse>> GetByUserIdAsync(Guid userId);
    Task<Result<IReadOnlyList<ProfileResponse>>> GetAllAsync();
    Task<Result<ProfileResponse>> UpdateAsync(Guid id, UpdateProfileRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<ProfileResponse>> ActivateAsync(Guid id);
    Task<Result<ProfileResponse>> DeactivateAsync(Guid id);
    Task<Result<AddressResponse>> AddAddressAsync(Guid profileId, CreateAddressRequest request);
    Task<Result<AddressResponse>> UpdateAddressAsync(Guid profileId, Guid addressId, UpdateAddressRequest request);
    Task<Result<bool>> RemoveAddressAsync(Guid profileId, Guid addressId);
}
