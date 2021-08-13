namespace NStructuredDataModel.Xml
{
    public sealed class XmlFormatOptions : StructuredDataFormatOptions
    {
        public string? RootElementName { get; set; }

        public string? ArrayElementName { get; set; }

        public static readonly XmlFormatOptions Default = new();
    }
}
