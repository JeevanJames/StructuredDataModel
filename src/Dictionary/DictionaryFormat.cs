using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NStructuredDataModel.Dictionary
{
    public sealed class DictionaryFormat : StructuredDataFormatBase<DictionaryFormatOptions>
    {
        public DictionaryFormat()
            : base(DictionaryFormatOptions.Default)
        {
        }

        public DictionaryFormat(DictionaryFormatOptions options)
            : base(options)
        {
        }

        protected override void Import(TextReader reader, Node node, CancellationToken cancellationToken)
        {
            base.Import(reader, node, cancellationToken);
        }

        protected override void Export(TextWriter writer, Node node, CancellationToken cancellationToken)
        {
            base.Export(writer, node, cancellationToken);
        }
    }

    public sealed class DictionaryFormatOptions : FormatOptions
    {
        public static readonly DictionaryFormatOptions Default = new();
    }
}
