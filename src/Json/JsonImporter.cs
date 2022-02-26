// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Threading;

namespace NStructuredDataModel.Json;

internal static class JsonImporter
{
    internal static void Import(Node node, ReadOnlySpan<byte> jsonBytes, CancellationToken cancellationToken)
    {
        Utf8JsonReader reader = new(jsonBytes, new JsonReaderOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });
        ImportNode(node, reader, cancellationToken);
    }

    private static void ImportNode(Node node, Utf8JsonReader reader, CancellationToken cancellationToken)
    {
        while (reader.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();
            node.Add(Guid.NewGuid().ToString("N"), new NodeValue(reader.TokenType.ToString()));
        }
    }
}
