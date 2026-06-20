using IdentityService.Domain.Common;

namespace IdentityService.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public User User { get; private set; } = null!;

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiry must be in the future.", nameof(expiresAt));
        }

        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    public void Revoke()
    {
        if (!IsRevoked)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }

    public bool IsActive => !IsRevoked && !IsExpired;
}
