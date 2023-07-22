namespace Core.Management.Common
{
    public interface ISecuritySetting
    {
        string Key { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        string TokenLifetimeInMins { get; set; }
        string CodeLifetimeInMins { get; set; }
    }
}
