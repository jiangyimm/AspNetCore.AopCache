using System;
using System.Threading.Tasks;

namespace AspNetCore.AopCache.Demo.Bll
{
    public interface ITestService
    {
        DateTime TestMethod();

        Task<object> TestMethodAsync();
    }
}
