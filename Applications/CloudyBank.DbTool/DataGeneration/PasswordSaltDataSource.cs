using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;
using CloudyBank.DataAccess.Security;
using CloudyBank.Core.Security;

namespace CloudyBank.DbTool.DataGeneration
{
    class PasswordSaltDataSource : IDatasource<String>
    {

        public object Next(IGenerationSession session)
        {
            return SHA256HashProvider.Instance.CreateSalt(8);
        }
    }
}
