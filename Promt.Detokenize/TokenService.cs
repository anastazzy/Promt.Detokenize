using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promt.Detokenize.contracts;

namespace Promt.Detokenize
{
    public class TokenService : ITokenService, IOutputDataGenerator
    {
        private static readonly string[] SpecialTokens = { "│C", "￨U", "@@", "@-@" };
        private static readonly string[] PunctuationTokens = { ",", ".", "?", "!" };

        public ResultMatching CreateStructMatches(TextParts textParts)
        {
            var translateText = new List<string>();
            var originalText = new List<string>();
            var match = new List<Match>();
            for (var i = 0; i < textParts.Matches.Length; i++)
            {

                if ((textParts.TranslationTextTokens.Length > textParts.Matches[i].TranslationIndex) &&
                    (textParts.OriginalTextTokens.Length > textParts.Matches[i].OriginalIndex))
                {
                    /* if (string.IsNullOrEmpty(textParts.TranslationTextTokens[i]))
                     {
                         translateText.Add(textParts.TranslationTextTokens[textParts.Matches[i].TranslationIndex]);
                         originalText.Add(null);
                         match.Add(new Match(textParts.Matches[i].OriginalIndex, textParts.Matches[i].TranslationIndex));

                     }
                     else
                     {
                         if (string.IsNullOrEmpty(textParts.OriginalTextTokens[i]))
                         {
                             translateText.Add(null);
                             originalText.Add(textParts.OriginalTextTokens[textParts.Matches[i].OriginalIndex]);
                             match.Add(new Match(textParts.Matches[i].OriginalIndex, textParts.Matches[i].TranslationIndex));
                         }
                         else
                         {
                             translateText.Add(textParts.TranslationTextTokens[textParts.Matches[i].TranslationIndex]);
                             originalText.Add(textParts.OriginalTextTokens[textParts.Matches[i].OriginalIndex]);
                             match.Add(new Match(textParts.Matches[i].OriginalIndex, textParts.Matches[i].TranslationIndex));
                         }
                    }*/

                    var index = textParts.Matches[i].TranslationIndex;
                    translateText.Add(textParts.TranslationTextTokens[index]);
                    index = textParts.Matches[i].OriginalIndex;
                    originalText.Add(textParts.OriginalTextTokens[index]);
                    match.Add(new Match(textParts.Matches[i].OriginalIndex, textParts.Matches[i].TranslationIndex));

                }
            }
            return new ResultMatching(translateText.ToArray(), originalText.ToArray(), match.ToArray());
        }
        
        public OutputData Generate(TextParts textParts, ResultMatching resultMatching)
        {
            var translationText = new List<IndexAndWord>();
            var originalText = new List<IndexAndWord>();

            var translationToken = string.Join(' ',resultMatching.TranslationText);
            var translationTextWithoutStaffTokens =
                (UsageStaffTokens(translationToken)).Split(' ');
            var count = 0;
            var countIndex = 0;

            foreach (var currentToken in textParts.TranslationTextTokens)
            {
                if (SpecialTokens.Contains(currentToken) ||
                    PunctuationTokens.Contains(currentToken)) continue;
                if ((count >= translationTextWithoutStaffTokens.Length)) continue;
                if (resultMatching.TranslationText.Contains(currentToken))
                {
                    translationText.Add(new IndexAndWord(countIndex, translationTextWithoutStaffTokens[count]));
                    count++;
                    countIndex++;
                }
                else
                {
                    if (currentToken.Contains(SpecialTokens[2])) continue;
                    translationText.Add(new IndexAndWord(null, translationTextWithoutStaffTokens[count]));
                    count++;
                }
            }

            var originalToken = string.Join(' ',textParts.OriginalTextTokens);
            var originalTextWithoutStaffTokens =
                (UsageStaffTokens(originalToken)).Split(' ');
            count = 0;
            countIndex = 0;
            foreach (var currentToken in textParts.OriginalTextTokens)
            {
                if (SpecialTokens.Contains(currentToken) ||
                    PunctuationTokens.Contains(currentToken)) continue;
                if ((count >= originalTextWithoutStaffTokens.Length)) continue;
                if (resultMatching.OriginalText.Contains(currentToken))
                {
                    originalText.Add(new IndexAndWord(countIndex, originalTextWithoutStaffTokens[count]));
                    count++;
                    countIndex++;
                }
                else
                {
                    if (currentToken.Contains(SpecialTokens[2])) continue;
                    originalText.Add(new IndexAndWord(null, originalTextWithoutStaffTokens[count]));
                    count++;
                }
            }
            return new OutputData(translationText.ToArray(), originalText.ToArray());
        }

        public string RemoveEntriesEnds(string str)
        {
            if ((str.EndsWith('.') || str.EndsWith(',') || str.EndsWith('?') ||
                str.EndsWith('!')) && (str[str.Length - 2]) == ' ')
            {
                str = str.Remove(str.Length - 2, 1);
            }
            return str;
        }

        public string AddedEntriesStart(string str, bool flag)
        {
            if (!(flag || (str.StartsWith(".") || str.StartsWith(",") ||
                         str.StartsWith("!") || str.StartsWith("?"))))
            {
                str = " " + str;
            }
            return str;
        }

        // применение специальных токенов к тексту для получения корректного рзультата - ДЕТОКЕНИЗАЦИЯ
        public string UsageStaffTokens(string strToken)
        {
            bool flag = false;
            if (strToken.Contains(SpecialTokens[0]))
            {
                var str = strToken.Split(SpecialTokens[0],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var strResult = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (i == 0)
                    {
                        str[i] = str[i].ToUpp();
                    }
                    else
                    {
                        if (i + 1 != str.Length) str[i] = str[i].ToUpp();
                    }

                    if (i == 0) { flag = true; } else { flag = false; }

                    strResult += AddedEntriesStart(str[i], flag);
                }
                strToken = strResult;
            }

            if (strToken.Contains(SpecialTokens[1]))
            {
                var str = strToken.Split(SpecialTokens[1],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var strResult = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (i + 1 != str.Length)
                    {
                        var temp = str[i].Substring(str[i].LastIndexOf(" "), str[i].Length - str[i].LastIndexOf(" ")).ToUpper();
                        str[i] = str[i].Substring(0, str[i].LastIndexOf(" ")) + temp;
                    }



                    if (i == 0) { flag = true; } else { flag = false; }

                    strResult += AddedEntriesStart(str[i], flag);
                }
                strToken = strResult;
            }

            if (strToken.Contains(SpecialTokens[2]))
            {
                var str = strToken.Split(SpecialTokens[2],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var strResult = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (i == 0) { flag = true; } else { flag = false; }
                    strResult += str[i];

                }
                strToken = strResult;
            }

            if (strToken.Contains(SpecialTokens[3]))
            {
                var str = strToken.Split(SpecialTokens[3],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var strResult = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (i + 1 != str.Length) str[i] += "-";
                    if (i == 0) { flag = true; } else { flag = false; }

                    strResult += str[i];
                }
                strToken = strResult;
            }

            strToken = RemoveEntriesEnds(strToken);


            return strToken;
        }
    }

    public static class StringExtension
    {
        public static string ToUpp(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var firstChar = "";
            if (str.Length == 1)
                return str[0].ToString().ToUpper();
            if ((str[0] == ',' || str[0] == '.' || str[0] == '!' || str[0] == '?' || str[0] == '-') && str.Length > 1)
            {
                firstChar = str[2].ToString().ToUpper();
                str = str.Substring(0, 2) + firstChar + str.Substring(3);
                return str;
            }
            else
            {
                firstChar = str[0].ToString().ToUpper();
                str = firstChar + str.Substring(1);
                return str;
            }
        }
    }
}

