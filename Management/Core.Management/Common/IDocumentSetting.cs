namespace Core.Management.Common
{
    public interface IDocumentSetting
    {
        string DatabaseName { get; set; }
        string ClientsCollection { get; set; }

        string ProductsCollection { get; set; }

    }
}
