// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NStructuredDataModel;

/// <summary>
///     Abstract base class for creating <see cref="IStructuredDataFormat"/> classes. This base
///     class provides support for options and provides sync versions of the <see cref="ImportAsync"/>
///     and <see cref="ExportAsync"/> methods.
/// </summary>
/// <typeparam name="TOptions">The type of the format options.</typeparam>
public abstract class StructuredDataFormatBase<TOptions> : IStructuredDataFormat
    where TOptions : FormatOptions
{
    protected StructuredDataFormatBase(TOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public TOptions Options { get; }

    /// <inheritdoc />
    public virtual Task ImportAsync(TextReader reader, Node node, CancellationToken cancellationToken = default)
    {
        Import(reader, node, cancellationToken);
        return Task.CompletedTask;
    }

    protected virtual void Import(TextReader reader, Node node, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual Task ExportAsync(TextWriter writer, Node node, CancellationToken cancellationToken = default)
    {
        Export(writer, node, cancellationToken);
        return Task.CompletedTask;
    }

    protected virtual void Export(TextWriter writer, Node node, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
