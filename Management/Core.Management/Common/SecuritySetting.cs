namespace Core.Management.Common
{
    public class SecuritySetting : ISecuritySetting
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string TokenLifetimeInMins { get; set; } = null!;
        public string CodeLifetimeInMins { get; set; } = null!;
    }
}
