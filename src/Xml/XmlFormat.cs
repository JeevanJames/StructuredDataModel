using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NStructuredDataModel.Xml
{
    public sealed class XmlFormat : StructuredDataFormatBase<XmlFormatOptions>
    {
        private readonly XmlFormatOptions _options;

        public XmlFormat()
            : base(XmlFormatOptions.Default)
        {
            _options = XmlFormatOptions.Default;
        }

        public XmlFormat(XmlFormatOptions options)
            : base(options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public override Task ImportAsync(TextReader reader, AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public override async Task ExportAsync(TextWriter writer, AbstractNode node)
        {
            XmlExporter exporter = new(_options);
            XDocument xdoc = exporter.Export(node);
            await writer.WriteAsync(xdoc.ToString()).ConfigureAwait(false);
        }
    }
}
