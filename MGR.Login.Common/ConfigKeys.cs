namespace MGR.Login.Common
{
    public static class Connections
    {
        public const string LoginDb = "LoginDb";
    }

    public static class Jwt
    {
        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";
        public const string ExpiryInHours = "Jwt:ExpiryInHours";
        public const string SecurityKey = "Jwt:SecurityKey";
    }
}
