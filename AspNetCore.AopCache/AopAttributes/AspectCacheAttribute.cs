using AspectCore.DynamicProxy;
using AspectCore.Injector;
using AspNetCore.AopCache.CacheService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.AopCache.AopAttributes
{
    /// <summary>
    /// 缓存属性
    /// <para>
    /// 1）将此属性应用于方法上，方法返回值将被缓存；
    /// </para>
    /// <para>
    /// 2）后续调用此方法时，将直接从缓存取值；
    /// </para>
    /// <para>
    /// 3）可用于DI注入的接口或者实现方法上2
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AspectCacheAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 缓存有限期，单位：分钟，默认为null，不指定则按内部缓存时间
        /// </summary>
        public int? Expiration { get; set; }

        /// <summary>
        /// 缓存key值，默认为null，不指定则按内部规则产生key值
        /// </summary>
        public string CacheKey { get; set; } = null;

        [FromContainer]
        public ICacheService CacheService { get; set; }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var parameters = context.ServiceMethod.GetParameters();
            //判断Method是否包含ref / out参数
            if (parameters.Any(it => it.IsIn || it.IsOut))
            {
                await next(context);
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
                    await next(context);
                    dynamic returnValue = context.ReturnValue;
                    if (context.ServiceMethod.IsReturnTask())
                    {
                        returnValue = returnValue.Result;
                    }

                    CacheService.SetValue(key, (object)returnValue, Expiration);
                }
            }
        }
    }
}
