using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Aop;
using System.Web;
using Common.Logging;
using AopAlliance.Intercept;

namespace CloudyBank.Services.Logging
{
    public class LoggingAdvice : IMethodInterceptor
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LoggingAdvice));
        
        public object Invoke(IMethodInvocation invocation)
        {
            String user = HttpContext.Current.User.Identity.Name;
            log.Debug(String.Format("{0} - {1} - called by: {2}", invocation.Method.Name, invocation.TargetType.Name,user));
            object returnValue = invocation.Proceed();
            log.Debug(String.Format("{0} - {1} - finished", invocation.Method.Name, invocation.TargetType.Name));
            return returnValue;
        }
    }
}
