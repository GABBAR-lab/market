using ProfileService.Domain.Common;

namespace ProfileService.Domain.Entities;

public class UserAddress : AuditableEntity
{
    public Guid UserProfileId { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public string StreetLine1 { get; private set; } = string.Empty;
    public string? StreetLine2 { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }

    public UserProfile UserProfile { get; private set; } = null!;

    public static UserAddress Create(
        Guid userProfileId,
        string label,
        string streetLine1,
        string? streetLine2,
        string city,
        string state,
        string country,
        string postalCode,
        bool isDefault)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(streetLine1);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);

        return new UserAddress
        {
            UserProfileId = userProfileId,
            Label = label.Trim(),
            StreetLine1 = streetLine1.Trim(),
            StreetLine2 = streetLine2?.Trim(),
            City = city.Trim(),
            State = state.Trim(),
            Country = country.Trim(),
            PostalCode = postalCode.Trim(),
            IsDefault = isDefault
        };
    }

    public void Update(
        string label,
        string streetLine1,
        string? streetLine2,
        string city,
        string state,
        string country,
        string postalCode,
        bool isDefault)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(streetLine1);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);

        Label = label.Trim();
        StreetLine1 = streetLine1.Trim();
        StreetLine2 = streetLine2?.Trim();
        City = city.Trim();
        State = state.Trim();
        Country = country.Trim();
        PostalCode = postalCode.Trim();
        IsDefault = isDefault;
        MarkAsUpdated();
    }

    public void ClearDefault()
    {
        if (IsDefault)
        {
            IsDefault = false;
            MarkAsUpdated();
        }
    }
}
