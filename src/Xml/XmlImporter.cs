// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Xml.Linq;

namespace NStructuredDataModel.Xml;

internal static class XmlImporter
{
    internal static void Import(XDocument xdoc, Node node, CancellationToken cancellationToken)
    {
        if (xdoc.Root is not null)
            Import(xdoc.Root, node, cancellationToken);
    }

    private static void Import(XElement element, Node node, CancellationToken cancellationToken)
    {
        foreach (XElement childElement in element.Elements())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (childElement.FirstNode is XText textElement)
                node.TryAdd(childElement.Name.LocalName, DetectValue(textElement.Value));
            else
            {
                Node childNode = new();
                node.Add(childElement.Name.LocalName, new NodeValue(childNode));
                Import(childElement, childNode, cancellationToken);
            }
        }
    }

    private static NodeValue DetectValue(string value)
    {
        if (double.TryParse(value, out double doubleValue))
            return new NodeValue(doubleValue);
        if (long.TryParse(value, out long longValue))
            return new NodeValue(longValue);
        if (bool.TryParse(value, out bool boolValue))
            return new NodeValue(boolValue);
        return new NodeValue(value);
    }
}
