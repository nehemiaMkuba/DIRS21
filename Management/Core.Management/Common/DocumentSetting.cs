namespace Core.Management.Common
{
    public class DocumentSetting : IDocumentSetting
    {
        public string DatabaseName { get; set; } 
        public string ClientsCollection { get; set; }
        public string ProductsCollection { get; set; }
    }
}
