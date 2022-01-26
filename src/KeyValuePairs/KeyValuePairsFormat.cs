// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NStructuredDataModel.KeyValuePairs;

public sealed class KeyValuePairsFormat : StructuredDataFormatBase<KeyValuePairsFormatOptions>
{
    public KeyValuePairsFormat()
        : base(KeyValuePairsFormatOptions.Default)
    {
    }

    public KeyValuePairsFormat(KeyValuePairsFormatOptions options)
        : base(options)
    {
    }

    public override async Task ExportAsync(TextWriter writer, Node node)
    {
        string propertyNameSeparator = Options.PropertyNameSeparator;
        string newLine = Options.NewLine;

        await node.Traverse(valueVisitor: async (path, value) =>
        {
            string propertyName = string.Join(propertyNameSeparator, path
                .Select(p => Options.ConvertPropertyName(p)));
            await writer.WriteAsync($"{propertyName}={value ?? string.Empty}")
                .ConfigureAwait(false);
            await writer.WriteAsync(newLine).ConfigureAwait(false);
        }, recursive: true).ConfigureAwait(false);
    }

    public override async Task ImportAsync(TextReader reader, Node node)
    {
        string? line = await reader.ReadLineAsync().ConfigureAwait(false);
        while (line is not null)
        {
            string[] lineParts = line.Split(new[] { '=' }, 2, StringSplitOptions.None);
            string? value = lineParts.Length == 1 ? null : lineParts[1];
            node.Write(lineParts[0].Split(Options.PropertyNameSeparator), value);

            line = await reader.ReadLineAsync().ConfigureAwait(false);
        }
    }
}
