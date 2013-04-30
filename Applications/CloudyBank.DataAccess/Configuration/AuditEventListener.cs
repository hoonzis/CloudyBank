using System;
using NHibernate.Event;
using System.Web;
using NHibernate.Persister.Entity;
using System.Data.SqlClient;

namespace CloudyBank.DataAccess.Configuration
{
    /// <summary>
    /// NHibernate event listener which might be used to audit every access to database.
    /// The access to audit table itself has to be excluded
    /// </summary>
    class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            SqlConnection connection = (SqlConnection)@event.Session.ConnectionManager.GetConnection();
            

            var tableName = ((ILockable)@event.Persister).RootTableName.ToLower();
            Log(@event, @event.State, @event.OldState,tableName, connection);
            connection.Dispose();
            
            return false;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            //This runs ok when running againts SQL Server
            //but if we run against other database it will fail
            SqlConnection connection = (SqlConnection)@event.Session.ConnectionManager.GetConnection();
            
            var tableName = ((ILockable)@event.Persister).RootTableName.ToLower();
            Log(@event, @event.State, null, tableName, connection);

            connection.Dispose();
            return false;
        }

        private void SaveAudit(String user, DateTime time, String newvalue, String oldvalue, String property, String tableName, SqlConnection connection)
        {
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText =
                        "insert into audit (AffectedUser, ChangeTime, NewValue, OldValue,Property, TableName) values "
                        + " (@AffectedUser, @ChangeTime, @NewValue, @OldValue, @Property, @TableName)";

                    SqlParameter affectedUserParam = new SqlParameter("@AffectedUser", user);
                    SqlParameter changeTimeParam = new SqlParameter("@ChangeTime", time);
                    SqlParameter newValueParam = new SqlParameter("@NewValue", newvalue);
                    SqlParameter oldValueParam = new SqlParameter("@OldValue", oldvalue);
                    SqlParameter propertyParam = new SqlParameter("@Property", property);
                    SqlParameter tableNameParam = new SqlParameter("@TableName", tableName);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                //any exeption thrown in the audit should not stop the application
            }
        }

        public void Log(AbstractPreDatabaseOperationEvent @event,object[] newstate,object[] oldstate,string tableName, SqlConnection connection)
        {
            //if we are writing audit table - we dont log the information
            if(tableName == "[simpleaudit]" || tableName == "\"simpleaudit\"")
            {
                return;
            }

            var time = DateTime.Now;
            string user;
            if (HttpContext.Current != null)
            {
                user = HttpContext.Current.User.Identity.Name;
            }
            else
            {
                user = "DBTool";
            }

            if (oldstate != null)
            {
                var dirties = @event.Persister.FindDirty(newstate, oldstate, @event.Entity, @event.Session);
                foreach (int dirty in dirties)
                {
                    var property = @event.Persister.PropertyNames[dirty];

                    var oldvalue = "null";
                    if (oldstate[dirty] != null)
                    {
                        oldvalue = oldstate[dirty].ToString();
                    }
                    var newvalue = "null";
                    if (newstate[dirty] != null)
                    {
                        newvalue = newstate[dirty].ToString();
                    }

                    SaveAudit(user, time, newvalue, oldvalue, property, tableName, connection);
                    
                }
            }
            else
            {
                for(int i=0;i<newstate.Length;i++)
                {
                    object obj = newstate[i];
                    if (obj != null)
                    {
                        var newValue = obj.ToString();
                        var value = newValue.Substring(0, Math.Min(newValue.Length, 100));
                        var property = @event.Persister.PropertyNames[i];
                        SaveAudit(user, time, value, "no", property, tableName, connection);
                    }
                }
            }
        }
    }
}
