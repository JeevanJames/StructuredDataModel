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

    public override async Task ImportAsync(TextReader reader, Node node, CancellationToken cancellationToken = default)
    {
        XDocument xdoc = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken)
            .ConfigureAwait(false);
        XmlImporter.Import(xdoc, node, cancellationToken);
    }

    public override async Task ExportAsync(TextWriter writer, Node node, CancellationToken cancellationToken = default)
    {
        XmlExporter exporter = new(_options);
        XDocument xdoc = exporter.Export(node, cancellationToken);
        await writer.WriteAsync(xdoc.ToString()).ConfigureAwait(false);
    }
}
