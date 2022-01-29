// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace NStructuredDataModel;

/// <summary>
///     Represents a <see cref="Node"/> value. This can be an object value (must be convertable to
///     and from string) or another Node, forming a hierarchy.
/// </summary>
public readonly struct NodeValue
{
    private readonly Node? _node;

    public NodeValue(object? value)
    {
        if (value is Node node)
        {
            _node = node;
            ObjectValue = null;
            Value = null;
            IsNode = true;
            IsValue = false;
        }
        else
        {
            _node = null;
            ObjectValue = value;
            Value = ConvertToString(value);
            IsNode = false;
            IsValue = true;
        }
    }

    /// <summary>
    ///     Gets the original value of this node.
    /// </summary>
    public object? ObjectValue { get; }

    /// <summary>
    ///     Gets the string representation of the value of this node.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    ///     Gets a value indicating whether this node value contains a node.
    /// </summary>
    public bool IsNode { get; }

    /// <summary>
    ///     Gets a value indicating whether this node value is a value and not a node.
    /// </summary>
    public bool IsValue { get; }

    /// <summary>
    ///     Attempts to convert the node value to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the node value to.</typeparam>
    /// <returns>The value of the node converted to the specified type.</returns>
    public T? As<T>()
    {
        return (T?)As(typeof(T));
    }

    public object? As(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));
        if (IsNode)
            throw new InvalidOperationException("The current value is a node. Cannot retrieve it as a value.");

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        if (!converter.CanConvertFrom(typeof(string)))
            throw new InvalidOperationException($"Cannot convert to type '{type}' from a string.");
        return converter.ConvertFromString(Value);
    }

    public Node AsNode()
    {
        return IsNode ? _node! : throw new InvalidCastException("The current value is not a node.");
    }

    public override string ToString()
    {
        return IsValue ? (Value ?? string.Empty) : "[Node]";
    }

    private static string? ConvertToString(object? value)
    {
        if (value is null)
            return null;

        if (value is string str)
            return str;

        TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
        if (!converter.CanConvertTo(typeof(string)))
            throw new InvalidOperationException($"Cannot convert type '{value.GetType()}' to a string.");
        return converter.ConvertToString(value);
    }
}
