namespace Login.Domain.Extensions
{
    public static class TokenPurposeExtensions
    {
        public static string GetName(this TokenPurpose tokenPurpose) => tokenPurpose.ToString();
        public static string GetProviderName(this TokenPurpose tokenPurpose) => $"{tokenPurpose}TokenProvider";
    }
}
