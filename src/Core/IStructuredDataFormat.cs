using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NStructuredDataModel
{
    public interface IStructuredDataFormat
    {
        Task ImportAsync(TextReader reader, AbstractNode node);

        Task ExportAsync(TextWriter writer, AbstractNode node);
    }

    public static class StructuredDataFormatExtensions
    {
        public static Task ImportAsync(this IStructuredDataFormat format, string data, AbstractNode node)
        {
            using StringReader reader = new(data);
            return format.ImportAsync(reader, node);
        }

        public static async Task<StructuredDataModel> ImportAsync(this IStructuredDataFormat format, TextReader reader)
        {
            StructuredDataModel model = new();
            await format.ImportAsync(reader, model).ConfigureAwait(false);
            return model;
        }

        public static Task<StructuredDataModel> ImportAsync(this IStructuredDataFormat format, string data)
        {
            using StringReader reader = new(data);
            return format.ImportAsync(reader);
        }

        public static async Task<string> ExportAsync(this IStructuredDataFormat format, AbstractNode node)
        {
            StringBuilder sb = new();
            await using StringWriter writer = new(sb);
            await format.ExportAsync(writer, node).ConfigureAwait(false);
            return writer.ToString();
        }
    }
}
