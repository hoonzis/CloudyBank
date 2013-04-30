using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Aop;
using CloudyBank.Core.DataAccess;
using System.Web;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Services.Security
{
    class AuditTrailAdvice : IMethodBeforeAdvice
    {

        IRepository _repository;
        public AuditTrailAdvice(IRepository repository)
        {
            _repository = repository;
        }

        public void Before(System.Reflection.MethodInfo method, object[] args, object target)
        {
            String user = HttpContext.Current.User.Identity.Name;
            SimpleAudit audit = new SimpleAudit();

            audit.CalledBy = user;
            audit.CalledAt = DateTime.Now;
            audit.MethodName = method.Name;
            audit.ServiceName = target.GetType().Name;

            _repository.Save<SimpleAudit>(audit);
            _repository.Flush();
        }
    }
}
