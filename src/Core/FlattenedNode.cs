// // Copyright (c) 2021-2022 Jeevan James
// // This file is licensed to you under the MIT license.
// // See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace NStructuredDataModel;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class FlattenedNode
{
    internal FlattenedNode(IList<string> keyPath, object? value)
    {
        string[] keyPathArray = new string[keyPath.Count];
        keyPath.CopyTo(keyPathArray, 0);
        KeyPath = keyPathArray;

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
