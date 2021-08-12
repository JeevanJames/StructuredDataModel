using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NStructuredDataModel.Json
{
    internal sealed class JsonExporter : IAsyncDisposable
    {
        private readonly MemoryStream _stream;
        private readonly Utf8JsonWriter _writer;

        internal JsonExporter()
        {
            _stream = new MemoryStream();
            _writer = new Utf8JsonWriter(_stream, new JsonWriterOptions { Indented = true });
        }

        internal async Task<ReadOnlyMemory<char>> ExportAsync(AbstractNode node)
        {
            _writer.WriteStartObject();
            ExportNode(node);
            _writer.WriteEndObject();
            await _writer.FlushAsync().ConfigureAwait(false);

            return Encoding.UTF8.GetString(_stream.ToArray()).AsMemory();
        }

        private void ExportNode(AbstractNode node)
        {
            foreach ((string property, object? value) in node)
            {
                if (value is AbstractNode childNode)
                {
                    _writer.WriteStartObject(property);
                    ExportNode(childNode);
                    _writer.WriteEndObject();
                }
                else
                {
                    switch (value)
                    {
                        case string str:
                            _writer.WriteString(property, str);
                            break;
                        case bool boolValue:
                            _writer.WriteBoolean(property, boolValue);
                            break;
                        case decimal decimalValue:
                            _writer.WriteNumber(property, decimalValue);
                            break;
                        case double doubleValue:
                            _writer.WriteNumber(property, doubleValue);
                            break;
                        case float floatValue:
                            _writer.WriteNumber(property, floatValue);
                            break;
                        case byte byteValue:
                            _writer.WriteNumber(property, byteValue);
                            break;
                        case sbyte sbyteValue:
                            _writer.WriteNumber(property, sbyteValue);
                            break;
                        case short shortValue:
                            _writer.WriteNumber(property, shortValue);
                            break;
                        case ushort ushortValue:
                            _writer.WriteNumber(property, ushortValue);
                            break;
                        case int intValue:
                            _writer.WriteNumber(property, intValue);
                            break;
                        case long longValue:
                            _writer.WriteNumber(property, longValue);
                            break;
                        case uint uintValue:
                            _writer.WriteNumber(property, uintValue);
                            break;
                        case ulong ulongValue:
                            _writer.WriteNumber(property, ulongValue);
                            break;
                        case null:
                            _writer.WriteNull(property);
                            break;
                        default:
                            _writer.WriteCommentValue("Unexpected value type encountered");
                            break;
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _writer.DisposeAsync().ConfigureAwait(false);
            await _stream.DisposeAsync().ConfigureAwait(false);
        }
    }
}
