using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CloudyBank.Web.Ria.MVVM
{
    public static class SingletonMediator
    {
        /// <summary>
        /// Objet synchronisation
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// Instance du mediator
        /// </summary>
        private static volatile Mediator _instance;

        /// <summary>
        /// Gets Instance.
        /// </summary>
        public static Mediator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Mediator();
                    }
                }
                return _instance;
            }
        }
    }
}
