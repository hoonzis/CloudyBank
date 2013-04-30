using System.Collections.Generic;
using System;
using System.Diagnostics.Contracts;
using CloudyBank.CoreDomain.Bank;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Globalization;
using CloudyBank.Core.DataAccess;

namespace CloudyBank.Services.Technical
{
    public static class Utils
    {
        public static bool AreListsEquals<T>(IList<T> list1, IList<T> list2)
        {
            if (list1 == null || list2 == null || list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; ++i)
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static String GetAppDataPath(String fileName)
        {
            String basePath = AppDomain.CurrentDomain.BaseDirectory;
            return basePath + "\\App_Data\\" + fileName;
        }

        public static String GetStringAfterLastSlash(String url)
        {

            Contract.Requires(!String.IsNullOrWhiteSpace(url));
            
            var lastSlash = url.LastIndexOf("/") + 1;
            var methodName = url.Substring(lastSlash, url.Length - lastSlash);
            return methodName;
        }
    }
}
