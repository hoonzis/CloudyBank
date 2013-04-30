using CloudyBank.CoreDomain.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Services.PaymentClassification
{
    public class TextClassifier
    {
        Dictionary<String, List<Tag>> _categorizationTable = new Dictionary<string, List<Tag>>();

        public void Initialize(IList<Tag> tags)
        {
            foreach (var tag in tags)
            {
                foreach(var word in tag.Words)
                {
                    if (_categorizationTable.ContainsKey(word))
                    {
                        _categorizationTable[word].Add(tag);
                    }
                    else
                    {
                        var list = new List<Tag>();
                        list.Add(tag);
                        _categorizationTable.Add(word, list);
                    }
                }
            }
        }

        public Tag Categorize(String word)
        {
            if (word == null)
            {
                return null;
            }

            var list = _categorizationTable[word];
            if (list != null)
            {
                return list[0];
            }
            return null;
        }
    }
}
