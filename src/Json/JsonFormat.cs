// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NStructuredDataModel.Json
{
    public sealed class JsonFormat : StructuredDataFormatBase<JsonFormatOptions>
    {
        public JsonFormat()
            : base(JsonFormatOptions.Default)
        {
        }

        public JsonFormat(JsonFormatOptions options)
            : base(options)
        {
        }

        public override async Task ExportAsync(TextWriter writer, AbstractNode node)
        {
            await using JsonExporter exporter = new(Options);
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
