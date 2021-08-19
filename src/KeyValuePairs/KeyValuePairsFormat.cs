// Copyright (c) 2021 Jeevan James
// This file is licenses to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NStructuredDataModel.KeyValuePairs
{
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

        public override async Task ExportAsync(TextWriter writer, AbstractNode node)
        {
            string propertyNameSeparator = Options.PropertyNameSeparator ?? ".";
            string propertyFormat = string.IsNullOrWhiteSpace(Options.PropertyFormat)
                ? "{0}={1}" : Options.PropertyFormat;
            string newLine = Options.NewLine ?? Environment.NewLine;

            await Traverse(node, _ => Task.CompletedTask, async (path, value) =>
            {
                string propertyName = string.Join(propertyNameSeparator, path
                    .Select(p => Options.PropertyNameConverter?.Invoke(p) ?? p));
                await writer.WriteAsync(string.Format(propertyFormat, propertyName, value ?? string.Empty));
                await writer.WriteAsync(newLine);
            });
        }
    }
}
