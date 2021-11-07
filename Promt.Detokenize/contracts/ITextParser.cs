using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promt.Detokenize.contracts
{
    /// <summary>
    /// Parse original string and return text parts to further processing
    /// </summary>
    public interface ITextParser
    {
        TextParts Parse(string input);
    }
}
