using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using AspNetCore.AopCache.CacheService;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.AopCache.AopAttributes
{
    /// <summary>
    /// 缓存属性Frame
    /// <para>
    /// 1）将此属性应用于方法上，方法返回值将被缓存；
    /// </para>
    /// <para>
    /// 2）后续调用此方法时，将直接从缓存取值；
    /// </para>
    /// <para>
    /// 3）可用于DI注入的接口或者实现方法上
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AspectCacheAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 缓存有限期，单位：分钟，默认值：10
        /// </summary>
        public int Expiration { get; set; } = 10;

        /// <summary>
        /// 缓存key值，默认为null，不指定则按内部规则产生key值
        /// </summary>
        public string CacheKey { get; set; } = null;

        /// <summary>
        /// 是否监测计时，默认为false
        /// </summary>
        public bool IsStopWatch { get; set; } = false;

        /// <summary>
        /// 计时日志类别，默认为‘External Api’
        /// </summary>
        public string Category { get; set; } = "External Api";

        /// <summary>
        /// 计时警告毫秒，默认为1000ms
        /// </summary>
        public long WarningMilliseconds { get; set; } = 1000;


        [FromServiceContext]
        public ILogger<AspectCacheAttribute> Logger { get; set; }
        [FromServiceContext]
        public ICacheService CacheService { get; set; }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var parameters = context.ServiceMethod.GetParameters();
            //判断Method是否包含ref / out参数
            if (parameters.Any(it => it.IsIn || it.IsOut))
            {
                if (IsStopWatch)
                {
                    await InvokeStopWatch(context, next);
                }
                else
                {
                    await next(context);
                }
            }
            else
            {
                var key = CacheKey ?? CacheService.GetCacheKey(context.ServiceMethod, parameters, context.Parameters);
                var type = context.ServiceMethod.ReturnType;
                var resultTypes = type.GenericTypeArguments;
                if (CacheService.TryGetValue(key, out var value, resultTypes.FirstOrDefault()))
                {
                    if (context.ServiceMethod.IsReturnTask())
                    {
                        if (value == null)
                        {
                            context.ReturnValue = Task.FromResult((object)null);
                        }
                        else
                        {
                            dynamic temp = value;
                            context.ReturnValue = Task.FromResult(temp);
                        }
                    }
                    else
                    {
                        context.ReturnValue = value;
                    }
                }
                else
                {
                    if (IsStopWatch)
                    {
                        await InvokeStopWatch(context, next);
                    }
                    else
                    {
                        await next(context);
                    }
                    dynamic returnValue = context.ReturnValue;
                    if (context.ServiceMethod.IsReturnTask())
                    {
                        returnValue = returnValue.Result;
                    }

                    CacheService.SetValue(key, (object)returnValue, Expiration);
                }
            }
        }

        private async Task InvokeStopWatch(AspectContext context, AspectDelegate next)
        {
            var stopWatch = Stopwatch.StartNew();
            await next(context);
            stopWatch.Stop();
            var timeElapsed = stopWatch.ElapsedMilliseconds;
            var methodInfo = $"{context.ServiceMethod.DeclaringType?.Namespace}.{context.ServiceMethod.DeclaringType?.Name}.{context.ServiceMethod.Name}";
            var logMsg = $"--->>> Category:{Category};Invoke ElapsedMilliseconds:{timeElapsed};MethodInfo:{methodInfo}";
            if (timeElapsed > WarningMilliseconds)
            {
                Logger.LogWarning(logMsg);
            }
            else
            {
                Logger.LogInformation(logMsg);
            }
        }
    }
}
