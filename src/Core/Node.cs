// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NStructuredDataModel;

/// <summary>
///     Represents a node in the structured data model. Nodes can have named values and child nodes.
/// </summary>
[Serializable]
public partial class Node : Dictionary<string, NodeValue>
{
    public Node()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    protected Node(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }

    /// <summary>
    ///     Adds a child node to the current node.
    /// </summary>
    /// <param name="key">The name of the child node.</param>
    /// <returns>A reference to the newly-created child node.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if <paramref name="key"/> is <c>null</c> or whitespace.
    /// </exception>
    public Node AddNode(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Node key cannot be null or whitespace.", nameof(key));
        Node node = new();
        Add(key, new NodeValue(node));
        return node;
    }

    /// <summary>
    ///     Traverses the current node.
    /// </summary>
    /// <param name="nodeVisitor">Optional delegate to execute whenever a node is visited.</param>
    /// <param name="valueVisitor">
    ///     Optional delegate to execute whenever a node value is visited.
    /// </param>
    /// <param name="recursive">Indicates whether to traverse recursively into child nodes.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Traverse(Func<IList<string>, Task>? nodeVisitor = null,
        Func<IList<string>, object?, Task>? valueVisitor = null,
        bool recursive = false)
    {
        return Traverse<object?>(null,
            (path, _) => nodeVisitor?.Invoke(path) ?? Task.CompletedTask,
            (path, value, _) => valueVisitor?.Invoke(path, value) ?? Task.CompletedTask,
            recursive);
    }

    /// <summary>
    ///     Traverses the current node.
    /// </summary>
    /// <param name="state">Custom state data to be passed into the visitor delegates.</param>
    /// <param name="nodeVisitor">Optional delegate to execute whenever a node is visited.</param>
    /// <param name="valueVisitor">
    ///     Optional delegate to execute whenever a node value is visited.
    /// </param>
    /// <param name="recursive">Indicates whether to traverse recursively into child nodes.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Traverse<TState>(TState state = default!,
        Func<IList<string>, TState, Task>? nodeVisitor = null,
        Func<IList<string>, object?, TState, Task>? valueVisitor = null,
        bool recursive = false)
    {
        List<string> path = new();
        return TraverseRecursive(this, state, path, nodeVisitor, valueVisitor, recursive);
    }

    // Common method used to traverse the structured data model from the specified node downwards.
    // The state param can be used to pass shared state to all visitor lambdas.
    // The path param is used to track the current node path from the original node.
    private async Task TraverseRecursive<TState>(Node node,
        TState state,
        IList<string> path,
        Func<IList<string>, TState, Task>? nodeVisitor,
        Func<IList<string>, object?, TState, Task>? valueVisitor,
        bool recursive)
    {
        foreach ((string key, NodeValue value) in node)
        {
            if (value.IsNode && recursive)
            {
                if (nodeVisitor is not null)
                    await nodeVisitor(path, state).ConfigureAwait(false);
                path.Add(key);
                await TraverseRecursive(value.AsNode(), state, path, nodeVisitor, valueVisitor, recursive)
                    .ConfigureAwait(false);
            }
            else
            {
                path.Add(key);
                if (valueVisitor is not null)
                    await valueVisitor(path, value.Value, state).ConfigureAwait(false);
            }

            path.RemoveAt(path.Count - 1);
        }
    }

    public async Task<IEnumerable<NodeEntryValue>> GetNodeEntryValues(bool recursive = false)
    {
        List<NodeEntryValue> values = new();
        await Traverse(values, valueVisitor: (nodeKey, nodeValue, state) =>
        {
            state.Add(new NodeEntryValue(nodeKey, nodeValue));
            return Task.CompletedTask;
        }, recursive: recursive).ConfigureAwait(false);
        return values;
    }

    /// <summary>
    ///     Attempts to get the children of the current node as an array. This will happen only
    ///     if there are keys that represent sequential numbers starting at zero.
    /// </summary>
    /// <param name="array">
    ///     The array of child items that have keys that represent sequential numbers.
    /// </param>
    /// <returns>
    ///     <c>True</c>, if an array could be obtained from the child nodes; otherwise <c>false</c>.
    /// </returns>
    public bool TryGetAsArray(out object?[] array)
    {
        if (Count == 0)
        {
            array = Array.Empty<object?>();
            return false;
        }

        // To avoid guess-allocating arrays for each node, check if there is a "0" element, and
        // if so, then proceed with the assumption that it is an array.
        if (!TryGetValue("0", out NodeValue firstValue))
        {
            array = Array.Empty<object?>();
            return false;
        }

        // Create an array and populate the first item.
        array = new object?[Count];
        array[0] = firstValue.Value;

        // Proceed with populating the rest of the array until we determine it is not an array.
        int index = 1;
        while (index < Count)
        {
            if (!TryGetValue(index.ToString(), out NodeValue value))
                return false;
            array[index] = value.Value;
            index++;
        }

        return true;
    }

    public T? GetValue<T>(string propertyName, T? defaultValue = default)
    {
        return TryGetValue(propertyName, out NodeValue value) ? value.As<T>() : defaultValue;
    }

    public T? Read<T>(ICollection<string> propertyPath, T? defaultValue = default)
    {
        ValidatePropertyPath(propertyPath);

        Node propertyParent = GetPropertyParentNode(propertyPath);
        return propertyParent.GetValue(propertyPath.Last(), defaultValue);
    }

    public T? Read<T>(string propertyName, T? defaultValue = default)
    {
        if (propertyName is null)
            throw new ArgumentNullException(nameof(propertyName));

        string[] propertyNames = propertyName.Split('.');
        return Read(propertyNames, defaultValue);
    }

    public Node Write<T>(ICollection<string> propertyPath, T? value)
    {
        ValidatePropertyPath(propertyPath);
        ValidatePropertyValue(value);

        Node parentNode = GetPropertyParentNode(propertyPath);
        string propertyName = propertyPath.Last();
        parentNode.AddOrUpdate(propertyName, new NodeValue(value));

        return this;
    }

    public Node Write<T>(string propertyName, T? value)
    {
        if (propertyName is null)
            throw new ArgumentNullException(nameof(propertyName));

        string[] propertyPath = propertyName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (propertyPath.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException($"Property name '{propertyName}' is invalid.", nameof(propertyName));

        return Write(propertyPath, value);
    }

    private static void ValidatePropertyPath(ICollection<string> propertyPath)
    {
        if (propertyPath is null)
            throw new ArgumentNullException(nameof(propertyPath));
        if (propertyPath.Count == 0)
            throw new ArgumentException("The property path cannot be empty.", nameof(propertyPath));
        if (propertyPath.Any(p => string.IsNullOrWhiteSpace(p) || !PropertyNamePattern.IsMatch(p)))
            throw new ArgumentException("The specified property path contains invalid characters.", nameof(propertyPath));
    }

    private static readonly Regex PropertyNamePattern = new(@"^[0-9A-Za-z_][\w\.-]*$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));

    /// <summary>
    ///     Returns the parent of a property path in the Variable. If parts of the property
    ///     tree do not exist, then they will be created.
    /// </summary>
    private Node GetPropertyParentNode(ICollection<string> propertyPath)
    {
        // This StringBuilder is used just to construct the error message.
        StringBuilder propertyNameBuilder = new();

        // Start traversal at the root object in the variable. If this is for a root property
        // (propertyPath is empty), then the Variable.Data property is returned.
        Node current = this;

        foreach (string property in propertyPath.SkipLast(1))
        {
            // Build up the property name up to the current property in the traversal.
            if (propertyNameBuilder.Length > 0)
                propertyNameBuilder.Append('.');
            propertyNameBuilder.Append(property);

            // If the property does not have a value, create a new tree node.
            if (!current.TryGetValue(property, out NodeValue propertyValue))
            {
                propertyValue = new NodeValue(new Node());
                current.Add(property, propertyValue);
            }

            // If the property value is not a Node, then it is a leaf node value. But
            // that should not be the case here as we expect a parent node, which should be a
            // VariableNode.
            else if (!propertyValue.IsNode)
            {
                string errorMessage =
                    $"The property {propertyNameBuilder} has already been assigned a scalar or array value. "
                    + "It cannot be a complex object.";
                throw new InvalidOperationException(errorMessage);
            }

            current = propertyValue.AsNode();
        }

        return current;
    }

    private static void ValidatePropertyValue(object? value)
    {
        // It's ok if the value is null
        if (value is null)
            return;

        Type valueType = value.GetType();

        // It's ok if the value type belongs to the valid list.
        if (ValidPropertyTypes.Contains(valueType))
            return;

        // Check if the value implements IEnumerable<>
        Type? enumerableInterface = Array.Find(valueType.GetInterfaces(), intf => intf.IsGenericType
            && intf.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (enumerableInterface is null)
            throw new ArgumentException("Property value is invalid.", nameof(value));

        // If the value implements IEnumerable<>, the generic type must belong to the valid list.
        Type enumerableType = enumerableInterface.GenericTypeArguments[0];
        if (!ValidPropertyTypes.Contains(enumerableType))
        {
            string errorMessage = $"Property value is an enumerable that contains an invalid type {enumerableType}.";
            throw new ArgumentException(errorMessage, nameof(value));
        }
    }

    private static readonly List<Type> ValidPropertyTypes = new()
    {
        typeof(string),
        typeof(char),
        typeof(bool),
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
    };
}

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class NodeEntryValue
{
    internal NodeEntryValue(IList<string> keyPath, object? value)
    {
        KeyPath = new List<string>(keyPath);
        Value = value;
    }

    public IList<string> KeyPath { get; }

    public object? Value { get; }

    public string KeyPathAsString(string nameSeparator)
    {
        return string.Join(nameSeparator, KeyPath);
    }

    public string DebuggerDisplay()
    {
        return $"{KeyPathAsString(".")} = {Value ?? "<NULL>"}";
    }
}
