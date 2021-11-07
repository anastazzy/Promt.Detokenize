using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promt.Detokenize.contracts
{
    interface IOutputDataGenerator
    {
        OutputData Generate(TextParts textParts, ResultMatching resultMatching);
    }
}
