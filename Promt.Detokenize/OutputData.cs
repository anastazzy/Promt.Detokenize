using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promt.Detokenize
{
    public record OutputData(IndexAndWord[] OriginalStringOutput, IndexAndWord[] TranslationStringOutput);
}
