// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Linq;

namespace NStructuredDataModel.Xml;

internal sealed class XmlImporter
{
    internal void Import(XDocument xdoc, Node node)
    {
        if (xdoc.Root is not null)
            Import(xdoc.Root, node);
    }

    private static void Import(XElement element, Node node)
    {
        foreach (XElement childElement in element.Elements())
        {
            if (childElement.FirstNode is XText textElement)
                node.TryAdd(childElement.Name.LocalName, DetectValue(textElement.Value));
            else
            {
                Node childNode = new();
                node.Add(childElement.Name.LocalName, new NodeValue(childNode));
                Import(childElement, childNode);
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
