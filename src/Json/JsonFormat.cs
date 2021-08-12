using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NStructuredDataModel.Json
{
    public sealed class JsonFormat : StructuredDataFormatBase
    {
        public override async Task ExportAsync(TextWriter writer, AbstractNode node)
        {
            await using JsonExporter exporter = new();
            ReadOnlyMemory<char> json = await exporter.ExportAsync(node).ConfigureAwait(false);
            await writer.WriteAsync(json).ConfigureAwait(false);
        }

        public override async Task ImportAsync(TextReader reader, AbstractNode node)
        {
            string json = await reader.ReadToEndAsync().ConfigureAwait(false);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            JsonImporter importer = new();
            importer.Import(node, jsonBytes);
        }
    }
}
