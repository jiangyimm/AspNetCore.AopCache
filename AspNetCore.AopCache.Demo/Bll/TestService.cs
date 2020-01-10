using AspNetCore.AopCache.AopAttributes;
using System;

namespace AspNetCore.AopCache.Demo.Bll
{
    public class TestService : ITestService
    {
        [AspectCache]
        public DateTime TestMethod()
        {
            return DateTime.Now;
        }
    }
}
