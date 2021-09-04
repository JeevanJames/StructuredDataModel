// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;

namespace NStructuredDataModel
{
    public abstract class StructuredDataFormatBase<TOptions> : IStructuredDataFormat
        where TOptions : FormatOptions
    {
        protected StructuredDataFormatBase(TOptions options)
        {
            Options = options;
        }

        public TOptions Options { get; }

        public virtual Task ImportAsync(TextReader reader, AbstractNode node)
        {
            Import(reader, node);
            return Task.CompletedTask;
        }

        protected virtual void Import(TextReader reader, AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public virtual Task ExportAsync(TextWriter writer, AbstractNode node)
        {
            Export(writer, node);
            return Task.CompletedTask;
        }

        protected virtual void Export(TextWriter writer, AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
