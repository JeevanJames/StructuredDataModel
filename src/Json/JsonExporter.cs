// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NStructuredDataModel.Json;

internal sealed class JsonExporter : IAsyncDisposable
{
    private readonly JsonFormatOptions _options;

    private readonly MemoryStream _stream;
    private readonly Utf8JsonWriter _writer;

    internal JsonExporter(JsonFormatOptions options)
    {
        _options = options;
        _stream = new MemoryStream();
        _writer = new Utf8JsonWriter(_stream, new JsonWriterOptions { Indented = true });
    }

    internal async Task<ReadOnlyMemory<char>> ExportAsync(Node node)
    {
        _writer.WriteStartObject();
        ExportNode(node);
        _writer.WriteEndObject();
        await _writer.FlushAsync().ConfigureAwait(false);

        return Encoding.UTF8.GetString(_stream.ToArray()).AsMemory();
    }

    private void ExportNode(Node node)
    {
        foreach ((string property, NodeValue value) in node)
        {
            string propertyName = _options.ConvertPropertyName(property);

            if (value.IsNode)
            {
                _writer.WriteStartObject(propertyName);
                ExportNode(value.AsNode());
                _writer.WriteEndObject();
            }
            else
            {
                switch (value.ObjectValue)
                {
                    case string str:
                        _writer.WriteString(propertyName, str);
                        break;
                    case char charValue:
                        _writer.WriteString(propertyName, charValue.ToString());
                        break;
                    case bool boolValue:
                        _writer.WriteBoolean(propertyName, boolValue);
                        break;
                    case decimal decimalValue:
                        _writer.WriteNumber(propertyName, decimalValue);
                        break;
                    case double doubleValue:
                        _writer.WriteNumber(propertyName, doubleValue);
                        break;
                    case float floatValue:
                        _writer.WriteNumber(propertyName, floatValue);
                        break;
                    case byte byteValue:
                        _writer.WriteNumber(propertyName, byteValue);
                        break;
                    case sbyte sbyteValue:
                        _writer.WriteNumber(propertyName, sbyteValue);
                        break;
                    case short shortValue:
                        _writer.WriteNumber(propertyName, shortValue);
                        break;
                    case ushort ushortValue:
                        _writer.WriteNumber(propertyName, ushortValue);
                        break;
                    case int intValue:
                        _writer.WriteNumber(propertyName, intValue);
                        break;
                    case long longValue:
                        _writer.WriteNumber(propertyName, longValue);
                        break;
                    case uint uintValue:
                        _writer.WriteNumber(propertyName, uintValue);
                        break;
                    case ulong ulongValue:
                        _writer.WriteNumber(propertyName, ulongValue);
                        break;
                    case null:
                        _writer.WriteNull(propertyName);
                        break;
                    default:
                        if (value.Value is not null)
                            _writer.WriteString(propertyName, value.Value);
                        else
                            _writer.WriteNull(propertyName);
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
