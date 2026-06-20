using ProfileService.Domain.Common;
using ProfileService.Domain.Enums;

namespace ProfileService.Domain.Entities;

public class UserProfile : AuditableEntity
{
    private readonly List<UserAddress> _addresses = [];

    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Website { get; private set; }
    public string Language { get; private set; } = "en";
    public string Currency { get; private set; } = "USD";
    public string Timezone { get; private set; } = "UTC";
    public bool EmailNotifications { get; private set; } = true;
    public bool SmsNotifications { get; private set; }
    public ProfileStatus Status { get; private set; } = ProfileStatus.Active;

    public IReadOnlyCollection<UserAddress> Addresses => _addresses.AsReadOnly();

    public static UserProfile Create(
        Guid userId,
        string firstName,
        string lastName,
        string? bio = null,
        string? avatarUrl = null,
        DateTime? dateOfBirth = null,
        Gender? gender = null,
        string? phoneNumber = null,
        string? website = null,
        string language = "en",
        string currency = "USD",
        string timezone = "UTC",
        bool emailNotifications = true,
        bool smsNotifications = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        return new UserProfile
        {
            UserId = userId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Bio = bio?.Trim(),
            AvatarUrl = avatarUrl?.Trim(),
            DateOfBirth = dateOfBirth,
            Gender = gender,
            PhoneNumber = phoneNumber?.Trim(),
            Website = website?.Trim(),
            Language = language.Trim(),
            Currency = currency.Trim(),
            Timezone = timezone.Trim(),
            EmailNotifications = emailNotifications,
            SmsNotifications = smsNotifications,
            Status = ProfileStatus.Active
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string? bio,
        string? avatarUrl,
        DateTime? dateOfBirth,
        Gender? gender,
        string? phoneNumber,
        string? website,
        string language,
        string currency,
        string timezone,
        bool emailNotifications,
        bool smsNotifications)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Bio = bio?.Trim();
        AvatarUrl = avatarUrl?.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        PhoneNumber = phoneNumber?.Trim();
        Website = website?.Trim();
        Language = language.Trim();
        Currency = currency.Trim();
        Timezone = timezone.Trim();
        EmailNotifications = emailNotifications;
        SmsNotifications = smsNotifications;
        MarkAsUpdated();
    }

    public UserAddress AddAddress(
        string label,
        string streetLine1,
        string? streetLine2,
        string city,
        string state,
        string country,
        string postalCode,
        bool isDefault)
    {
        if (isDefault)
        {
            foreach (var existingAddress in _addresses)
            {
                existingAddress.ClearDefault();
            }
        }

        var address = UserAddress.Create(
            Id,
            label,
            streetLine1,
            streetLine2,
            city,
            state,
            country,
            postalCode,
            isDefault);

        _addresses.Add(address);
        MarkAsUpdated();
        return address;
    }

    public void UpdateAddress(
        Guid addressId,
        string label,
        string streetLine1,
        string? streetLine2,
        string city,
        string state,
        string country,
        string postalCode,
        bool isDefault)
    {
        var address = GetAddress(addressId);

        if (isDefault)
        {
            foreach (var existing in _addresses.Where(a => a.Id != addressId))
            {
                existing.ClearDefault();
            }
        }

        address.Update(label, streetLine1, streetLine2, city, state, country, postalCode, isDefault);
        MarkAsUpdated();
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = GetAddress(addressId);
        _addresses.Remove(address);
        MarkAsUpdated();
    }

    public void Activate()
    {
        Status = ProfileStatus.Active;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        Status = ProfileStatus.Inactive;
        MarkAsUpdated();
    }

    public void SoftDelete()
    {
        Status = ProfileStatus.Deleted;
        MarkAsUpdated();
    }

    private UserAddress GetAddress(Guid addressId)
    {
        return _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new InvalidOperationException("Address not found.");
    }
}
