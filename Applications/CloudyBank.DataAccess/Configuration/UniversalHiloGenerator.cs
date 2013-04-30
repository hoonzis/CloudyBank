using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.DataAccess.Configuration
{
    //This is an envelop for classic HiLo generator of identities.
    //The class is created in order to have the possibility to override the SQL statement which is used to 
    //create the table for identities.
    //In Azure all the tables have to have CLUSTERED INDEX. NHibernate by itself creates a table "hibernate_unique_id"
    //without clustered index - here the SQL is changed to add the clustered index.
    public class UniversalHiloGenerator : NHibernate.Id.TableHiLoGenerator
    {
        
        //SQLLite does not support IF THEN and CLUSTERED INDEX constructs - so thes lines have to be commented out
        //SQL Lite is used for Unit test and also for the Fixtures!
        //the dialect is used decide which one to use
        public override string[] SqlCreateStrings(NHibernate.Dialect.Dialect dialect)
        {
            List<String> commands = new List<string>();
            var dialectName = dialect.ToString();
            
            if(dialectName != "NHibernate.Dialect.SQLiteDialect")
                commands.Add("IF OBJECT_ID('dbo.NH_HiLo', 'U') IS NOT NULL \n DROP TABLE dbo.NH_HiLo; \nGO");

            commands.Add("CREATE TABLE NH_HiLo (TableKey varchar(50), NextHi int)");
            
            if (dialectName != "NHibernate.Dialect.SQLiteDialect")
                commands.Add("CREATE CLUSTERED INDEX NH_HiLoIndex ON NH_HiLo (TableKey)");

            string[] tables = {"Operation","BusinessPartner","PaymentEvent","BalancePoint","Account","Agency","Tag","TagDepense", "CustomerProfile","UserIdentity"};
            
            
            var returnArray = commands.Concat(GetInserts(tables)).ToArray();
            return returnArray;
        }

        private IEnumerable<String> GetInserts(string[] tables)
        {
            foreach (var table in tables)
            {
                yield return String.Format("insert into NH_HiLo values ('{0}',1)", table);
            }
        }

        public override void Configure(NHibernate.Type.IType type, IDictionary<string, string> parms, NHibernate.Dialect.Dialect dialect)
        {
            base.Configure(type, parms, dialect);
        }
    }
}
