namespace Movie.Api;

public static class ApiConstants
{
    public const string AdminUserPolicy = "Admin";
    public const string AdminUserClaim = "admin";
    public const string TrustedUserPolicy = "Trusted";
    public const string TrustedUserClaim = "trusted_member";
    public const string ApiKeyHeaderName = "x-api-key";
}