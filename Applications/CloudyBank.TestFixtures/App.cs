using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Context;
using NHibernate;
using Spring.Context.Support;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using NHibernate.Context;
using System.Web.Hosting;
using System.Web;
using System.Security.Principal;
using System.Threading;
using System.IO;
using CloudyBank.TestFixtures.TestHelper;

namespace CloudyBank.TestFixtures
{
    public static class App
    {

        private static IApplicationContext _applicationContext;
        private static ISessionFactory _sessionFactory;

        public static T GetObject<T>(string id)
        {
            return (T)_applicationContext.GetObject(id);
        }

        public static T GetObject<T>()
        {
            return (T)_applicationContext.GetObject(typeof(T).Name);
        }

        public static void InitSessionFactory()
        {
            _applicationContext = ContextRegistry.GetContext();
            _sessionFactory = GetObject<ISessionFactory>("SessionFactory");
            
        }

        public static void Bind()
        {
            CurrentSessionContext.Bind(NhibernateHelper.SessionFactory.OpenSession());
            //CurrentSessionContext.Bind(_sessionFactory.OpenSession());
        }

        public static void CloseCurrentSession()
        {
            ISession session = CurrentSessionContext.Unbind(_sessionFactory);

            if (session != null)
            {
                session.Close();
            }
        }

        public static void InitApp()
        {
            SimpleWorkerRequest request = new SimpleWorkerRequest("/dummy", @"c:\inetpub\wwwroot\dummy", "dummy.html", null, new StringWriter());
            HttpContext context = new HttpContext(request);
            HttpContext.Current = context;

            var identity = new GenericIdentity("tester");
            var roles = new[] { @"BUILTIN\Users" };
            var principal = new GenericPrincipal(identity, roles);


            //CassiniDev.CassiniDevServer server = new CassiniDev.CassiniDevServer();
            //server.StartServer(
            HttpContext.Current.User = principal;
            Thread.CurrentPrincipal = principal;


            CloudyBank.Web.Global.InitSessionFactory();
            CloudyBank.Web.Global.InitCloud();
        }

    }
}
