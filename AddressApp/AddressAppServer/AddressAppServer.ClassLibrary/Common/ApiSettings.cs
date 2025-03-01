namespace AddressAppServer.ClassLibrary.Common
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
