using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Promt.Detokenize
{
    public class TokenStringService
    {
        public struct Match
        {
            public int OriginalIndex { get; set; }
            public int TranslationIndex { get; set; }
        }

        public class Result
        {
            public string OriginalText { get; set; }
            public string TranslationText { get; set; }
            public Match Match { get; set; }

            public Result(string originalText, string translationText, Match match)
            {
                OriginalText = originalText;
                TranslationText = translationText;
                Match = match;
            }
        }

        public List<string> ItsNotWork(string str)
        {
            var arrayString = CreateArrayStrings(str);
            var matchList = CreateStringRange(arrayString[2]);
            var str1 = CreateArrayTokens(arrayString[0]);
            var str2 = CreateArrayTokens(arrayString[1]);
            var aaa = MatchStr(str1, str2, matchList);
            return aaa;
        }


        //создание массива строк из 3 строк - оригинала, перевода и диапазона
        public string[] CreateArrayStrings(string inString)
        {
            var str = " ||| ";
            var strTokens = inString.Split(str, StringSplitOptions.RemoveEmptyEntries);
            return strTokens;
        }
        
        //создание структуры соответствий
        public List<Match> CreateStringRange(string strRange)
        {
            List<Match> strIndex = new();
            var coupleIndex = strRange.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var index in coupleIndex)
            {
                var arrayIndex = index.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var temp1 = Convert.ToInt32(arrayIndex[0]);
                var temp2 = Convert.ToInt32(arrayIndex[1]);
                strIndex.Add(new Match
                {
                    OriginalIndex = temp1,
                    TranslationIndex = temp2
                });
            }

            return strIndex;
        }
        //разделение строки на токены - массив токенов для 1 и 2 масссивов 
        public string[] CreateArrayTokens(string strTokens)
        {
            var token = strTokens.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return token;
        }

        public List<string> MatchStr(string[] str1, string[] str2, List<Match> chart)
        {
            var bigResult = new List<Result>();
            for (var i = 0; i < chart.Count; i ++)
            {
                if ((str1.Length> chart[i].OriginalIndex) && (str2.Length > chart[i].TranslationIndex))
                {
                    var r = new Result(str1[chart[i].OriginalIndex], str2[chart[i].TranslationIndex], chart[i]);
                    bigResult.Add(r);
                }
            }

            var transResult1 = bigResult.Select(x => x.OriginalText);
            var resultResult1 = UsageStaffTokens(string.Join(' ', transResult1));
            var resultAfterDetokenize = UsageStaffTokens(string.Join(" ", str1));
            

            var transResult2 = bigResult.Select(x => x.TranslationText);
            var resultResult2 = UsageStaffTokens(string.Join(' ', transResult2));

            var res = Print(resultResult1);
            List<string> resList = new();
            resList.Add(res);
            res = Print(resultResult2);
            resList.Add(res);
            res = Print(resultAfterDetokenize);
            resList.Add(res);
            return resList;
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
            var spesialTokens = new[] { "│C", "￨U", "@@", "@-@" };
            bool flag = false;
            if (strToken.Contains(spesialTokens[0]))
            {
                var str = strToken.Split(spesialTokens[0],
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

                    if (i == 0) {flag = true;} else {flag = false;}
                    
                    strResult += AddedEntriesStart(str[i], flag);
                }
                strToken = strResult;
            }

            if (strToken.Contains(spesialTokens[1]))
            {
                var str = strToken.Split(spesialTokens[1],
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

            if (strToken.Contains(spesialTokens[2]))
            {
                var str = strToken.Split(spesialTokens[2],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var strResult = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (i == 0) { flag = true; } else { flag = false; }
                    strResult += str[i];

                }
                strToken = strResult;
            }

            if (strToken.Contains(spesialTokens[3]))
            {
                var str = strToken.Split(spesialTokens[3],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var strResult = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (i+1 != str.Length) str[i] += "-";
                    if (i == 0) { flag = true; } else { flag = false; }

                    strResult += str[i];
                }
                strToken = strResult;
            }

            strToken = RemoveEntriesEnds(strToken);


            return strToken;
        }

        public string Print(string str)
        {
            var strin = "";
            var arr = str.Split(" ");
            for (var i=0; i<arr.Length; i++)
            {
                strin += $"[{i}]{arr[i]}[/{i}] ";
            }

            return strin;
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
                str = str.Substring(0,2)+ firstChar + str.Substring(3);
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
