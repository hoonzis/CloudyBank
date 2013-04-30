using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using System.Data;
using NHibernate.SqlTypes;
using CloudyBank.DataAccess.Security;

namespace CloudyBank.DataAccess.UserTypes
{
    public class EncryptedString : IUserType
    {

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object NullSafeGet(IDataReader rs, String[] names, object owner)
        {
            object r = rs[names[0]];
            if (r == DBNull.Value)
            {
                return null;
            }
            return DesEncryptionProvider.Decrypt((String)r);
        }

        public void NullSafeSet(System.Data.IDbCommand cmd, object value, int index)
        {
            object paramValue = DBNull.Value;
            if (value != null)
            {
                paramValue = DesEncryptionProvider.Encrypt((String)value);
            }
            IDataParameter parameter = (IDataParameter)cmd.Parameters[index];
            parameter.Value = paramValue;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public Type ReturnedType
        {
            get { return typeof(string); }
        }

        public NHibernate.SqlTypes.SqlType[] SqlTypes
        {
            get { return new SqlType[] { new StringSqlType() }; }
        }
    }
}
