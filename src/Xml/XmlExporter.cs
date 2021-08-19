// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Linq;

namespace NStructuredDataModel.Xml
{
    internal sealed class XmlExporter
    {
        private readonly XmlFormatOptions _options;
        private readonly string _arrayElementName;
        private readonly string _rootElementName;

        internal XmlExporter(XmlFormatOptions options)
        {
            _options = options;

            _arrayElementName = _options.ArrayElementName ?? "Value";
            _arrayElementName = _options.PropertyNameConverter?.Invoke(_arrayElementName) ?? _arrayElementName;

            _rootElementName = _options.RootElementName ?? "Root";
            _rootElementName = _options.PropertyNameConverter?.Invoke(_rootElementName) ?? _rootElementName;
        }

        internal XDocument Export(AbstractNode node)
        {
            XDocument xdoc = new();

            XElement rootElement = new(_rootElementName);
            xdoc.Add(rootElement);

            ExportNode(node, rootElement);

            return xdoc;
        }

        private void ExportNode(AbstractNode node, XElement element)
        {
            if (node.TryGetAsArray(out object?[] array))
            {
                foreach (object? arrayValue in array)
                {
                    switch (arrayValue)
                    {
                        case AbstractNode childNode:
                            XElement childElement = new(_arrayElementName);
                            element.Add(childElement);
                            ExportNode(childNode, childElement);
                            break;
                        default:
                            XElement valueElement = new(_arrayElementName, new XText(arrayValue?.ToString() ?? string.Empty));
                            element.Add(valueElement);
                            break;
                    }
                }
            }
            else
            {
                foreach ((string property, NodeValue value) in node)
                {
                    string propertyName = !char.IsLetter(property[0]) ? $"_{property}" : property;
                    propertyName = _options.PropertyNameConverter?.Invoke(propertyName) ?? propertyName;
                    if (value.ValueType == NodeValueType.Node)
                    {
                        XElement childElement = new(propertyName);
                        element.Add(childElement);
                        ExportNode(value.AsNode(), childElement);
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
}
