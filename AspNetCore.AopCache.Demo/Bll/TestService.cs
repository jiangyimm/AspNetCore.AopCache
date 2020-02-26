using AspNetCore.AopCache.AopAttributes;
using System;
using System.Threading.Tasks;

namespace AspNetCore.AopCache.Demo.Bll
{
    public class TestService : ITestService
    {
        [AspectCache]
        public DateTime TestMethod()
        {
            return DateTime.Now;
        }

        [AspectCache]
        public async Task<object> TestMethodAsync()
        {

            return null;
        }
    }
}
