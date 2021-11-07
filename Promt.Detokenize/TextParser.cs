using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promt.Detokenize.contracts;

namespace Promt.Detokenize
{
    public class TextParser : ITextParser
    {
        private const string TextDelimiter = " ||| ";
        
        public TextParts Parse(string input)
        {

            var strTokens = input.Split(TextDelimiter, StringSplitOptions.RemoveEmptyEntries);
            var originalTextToken = GetTextTokens(strTokens[0]);
            var translationTextToken = GetTextTokens(strTokens[1]);
            var matches = GetListOfMatches(strTokens[2]);
            return new TextParts(originalTextToken, translationTextToken, matches);
        }

        public string[] GetTextTokens(string strTokens)
        {
            return strTokens.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Match[] GetListOfMatches(string strRange)
        {
            List<Match> strIndex = new();
            var coupleIndex = strRange.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var index in coupleIndex)
            {
                var arrayIndex = index.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var originalIndex = Convert.ToInt32(arrayIndex[0]);
                var translationIndex = Convert.ToInt32(arrayIndex[1]);
                strIndex.Add(new Match(originalIndex, translationIndex));
            }
            return strIndex.ToArray();
        }
    }
}
