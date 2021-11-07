using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promt.Detokenize;
using Promt.Detokenize.contracts;
using Xunit;

namespace Promt.Tests
{
    public class TokenServiceTest
    {
        private readonly TokenService _service;
        private readonly ITextParser _parser;

        public TokenServiceTest()
        {
            _service = new TokenService();
            _parser = new TextParser();
        }
        [Fact]
        public void ItWork()
        {
            var inString = "however │C , the │C times │C has not been able to independently corro@@ b@@ orate this information . ||| однако │C the │C times │C не смогла самостоятельно подтвердить эту информацию . ||| 0-0 1-1 3-2 4-3 5-4 6-5 6-13 8-6 9-7 12-8 15-9 16-10 17-11 18-12";
            var textParts = _parser.Parse(inString);
            var matches = _service.CreateStructMatches(textParts);
            var result = _service.Generate(textParts, matches);
            
        }
    }
}
