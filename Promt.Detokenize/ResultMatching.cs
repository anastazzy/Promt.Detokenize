using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promt.Detokenize
{
    public record ResultMatching(string[] TranslationText, string[] OriginalText, Match[] RangeWords);
}
