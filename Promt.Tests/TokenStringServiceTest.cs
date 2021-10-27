using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promt.Detokenize;
using Xunit;

namespace Promt.Tests
{
    public class TokenStringServiceTest 
    {
        private readonly TokenStringService _service;

        public TokenStringServiceTest()
        {
            _service = new TokenStringService();
        }

        [Fact]
        public void CreateStrToken_ShouldCreateToken()
        {
            var inString = "however │C , the │C times │C has not been able to independently corro@@ b@@ orate this information . ||| однако │C the │C times │C не смогла самостоятельно подтвердить эту информацию . ||| 0-0 1-1 3-2 4-3 5-4 6-5 6-13 8-6 9-7 12-8 15-9 16-10 17-11 18-12";
            string[] arrayString =
            {
                "however │C , the │C times │C has not been able to independently corro@@ b@@ orate this information .",
                "однако │C the │C times │C не смогла самостоятельно подтвердить эту информацию .",
                "0-0 1-1 3-2 4-3 5-4 6-5 6-13 8-6 9-7 12-8 15-9 16-10 17-11 18-12"
            }; 

            var result = _service.CreateArrayStrings(inString);
            
            Assert.Equal(arrayString[0], result[0]);
            Assert.Equal(arrayString[1], result[1]);
            Assert.Equal(arrayString[2], result[2]);
        }

        [Fact]
        public void CreateToken_ShouldMakeTokenFromTheString()
        {
            string str =
                "however │C , the │C times │C has not been able to independently corro@@ b@@ orate this information .";
            string[] arrayTokens =
            {
                "however", "│C", ",", "the", "│C", "times", "│C", "has", "not", "been", "able", "to", "independently",
                "corro@@", "b@@", "orate", "this", "information", "."
            };
            string str1 =
                "однако │C the │C times │C не смогла самостоятельно подтвердить эту информацию .";
            string[] arrayTokens1 =
            {
                "однако","│C","the","│C","times","│C","не","смогла","самостоятельно","подтвердить","эту","информацию","."
            };

            var result = _service.CreateArrayTokens(str);
            var result1 = _service.CreateArrayTokens(str1);

            Assert.Equal(arrayTokens, result);
        }

        [Fact]
        public void CreateListOfIndex()
        {
            var str = "0-0 1-2 2-2 3-8";
            List<int> temp = new List<int>() { 0, 0, 1, 2, 2, 2, 3, 8 };

            var result = _service.CreateStringRange(str);
            
        }

        [Fact]
        public void UseStaffTokens()
        {
            var str =
                "however │C , the │C times │C has not ￨U been able ￨U to independently corro@@ b@@ orate @-@ this information .";
            var expectedResult =
                "However, The Times has NOT been ABLE to independently corroborate-this information.";

           var result = _service.UsageStaffTokens(str);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void UseMatch()
        {

            var str = "0-0 1-1 3-2 4-3 5-4 6-5 6-13 8-6 9-7 12-8 15-9 16-10 17-11 18-12";
            string[] arrayTokens =
            {
                "however", "│C", ",", "the", "│C", "times", "│C", "has", "not", "been", "able", "to", "independently",
                "corro@@", "b@@", "orate", "this", "information", "."
            };
            string[] arrayTokens1 =
            {
                "однако","│C","the","│C","times","│C","не","смогла","самостоятельно","подтвердить","эту","информацию","."
            };
            string aaa = "Однако The Times не смогла самостоятельно подтвердить эту информацию.";


            var matchList = _service.CreateStringRange(str);
            var aaaa = _service.MatchStr(arrayTokens, arrayTokens1, matchList);
            
        }

        [Fact]
        public void ItNotWork()
        {
            var inString = "however │C , the │C times │C has not been able to independently corro@@ b@@ orate this information . ||| однако │C the │C times │C не смогла самостоятельно подтвердить эту информацию . ||| 0-0 1-1 3-2 4-3 5-4 6-5 6-13 8-6 9-7 12-8 15-9 16-10 17-11 18-12";
            var await = _service.ItsNotWork(inString);

            List<string> result = new();
            result.Add("[0]However,[/0] [1]The[/1] [2]Times[/2] has [3]not[/3] [4]been[/4] able to [5]independently[/ 5][6]corroborate[/ 6][7]this[/ 7][8]information.[/ 8]");
            result.Add("[0]Однако[/0] [1]The[/1] [2]Times[/2] [3]не[/3] [4]смогла[/4] [5]самостоятельно[/ 5][6]подтвердить[/ 6][7]эту[/ 7][8]информацию.[/ 8]");

            Assert.Equal(result, await);
        }
    }
}
