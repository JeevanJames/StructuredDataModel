// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NStructuredDataModel.Xml;

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

    public override async Task ImportAsync(TextReader reader, Node node)
    {
        XDocument xdoc = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None)
            .ConfigureAwait(false);
        XmlImporter importer = new();
        importer.Import(xdoc, node);
    }

    public override async Task ExportAsync(TextWriter writer, Node node)
    {
        XmlExporter exporter = new(_options);
        XDocument xdoc = exporter.Export(node);
        await writer.WriteAsync(xdoc.ToString()).ConfigureAwait(false);
    }
}
