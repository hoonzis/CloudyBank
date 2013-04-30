using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Attributes;

namespace CloudyBank.UnitTests.MachineLearning
{
    public class Payment
    {
        [StringFeature(SplitType = StringType.Word)]
        public String Description { get; set; }

        [Feature]
        public Decimal Amount { get; set; }

        [Label]
        public String Category { get; set; }

        public static IEnumerable<Payment> GetData()
        {
            var list = new List<Payment>();
            list.Add(new Payment { Amount = 23, Description = "CARTE Monop", Category = "Food" });
            list.Add(new Payment { Amount = 123, Description = "SNCF bla bla", Category = "Travel" });
            list.Add(new Payment { Amount = 30, Description = "Monop", Category = "Food" });
            list.Add(new Payment { Amount = 22, Description = "Monop bla bla", Category = "Food" });
            list.Add(new Payment { Amount = 100, Description = "SNCF blusdfs", Category = "Travel" });
            list.Add(new Payment { Amount = 122, Description = "SNCF Bordeaux", Category = "Travel" });
            list.Add(new Payment { Amount = 130, Description = "Monop sfihj dgd bla", Category = "Household" });
            list.Add(new Payment { Amount = 105, Description = "Monop sdhihj gijn", Category = "Household" });

            return list;
        }
    }
}
