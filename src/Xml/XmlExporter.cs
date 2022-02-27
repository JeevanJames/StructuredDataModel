// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace NStructuredDataModel.Xml;

internal sealed class XmlExporter
{
    private readonly XmlFormatOptions _options;
    private readonly string _arrayElementName;
    private readonly string _rootElementName;

    internal XmlExporter(XmlFormatOptions options)
    {
        _options = options;
        _arrayElementName = _options.ConvertPropertyName(_options.ArrayElementName ?? "Value");
        _rootElementName = _options.ConvertPropertyName(_options.RootElementName ?? "Root");
    }

    internal XDocument Export(Node node, CancellationToken cancellationToken)
    {
        XDocument xdoc = new();

        XElement rootElement = new(_rootElementName);
        xdoc.Add(rootElement);

        ExportNode(node, rootElement, cancellationToken);

        return xdoc;
    }

    private void ExportNode(Node node, XElement element, CancellationToken cancellationToken)
    {
        if (node.TryGetAsArray(out IList<NodeValue> array))
        {
            foreach (NodeValue arrayValue in array)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (arrayValue.IsNode)
                {
                    XElement childElement = new(_arrayElementName);
                    element.Add(childElement);
                    ExportNode(arrayValue.AsNode(), childElement, cancellationToken);
                }
                else
                {
                    XElement valueElement = new(_arrayElementName, new XText(arrayValue.Value ?? string.Empty));
                    element.Add(valueElement);
                }
            }
        }
        else
        {
            foreach ((string property, NodeValue value) in node)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string propertyName = !char.IsLetter(property[0]) ? $"_{property}" : property;
                propertyName = _options.ConvertPropertyName(propertyName);
                if (value.IsNode)
                {
                    XElement childElement = new(propertyName);
                    element.Add(childElement);
                    ExportNode(value.AsNode(), childElement, cancellationToken);
                }
                else
                {
                    XElement valueElement = new(propertyName, new XText(value.Value?.ToString() ?? string.Empty));
                    element.Add(valueElement);
                }
            }
        }
    }
}
