using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(TagServicesContracts))]
    public interface ITagServices
    {
        IList<TagDto> GetTagsForCustomer(int customerID);

        void CreateTag(int customerID, TagDto tag);

        void TagOperation(int operationID, int tagID);

        void UpdateTag(TagDto tag,int customerID);

        void SaveOrUpdateTag(TagDto tag, int customerID);

        void RemoveTag(int tagID, int customerID);

        List<TagDto> GetStandardTags();
    }

    [ContractClassFor(typeof(ITagServices))]
    public abstract class TagServicesContracts : ITagServices
    {

        public IList<TagDto> GetTagsForCustomer(int customerID)
        {
            throw new NotImplementedException();
        }

        public void CreateTag(int customerID, TagDto tag)
        {
            Contract.Requires<TagServicesException>(tag != null);
            Contract.Requires<TagServicesException>(tag.Title != null);
            Contract.Requires<TagServicesException>(tag.Description != null);
        }

        public void TagOperation(int operationID, int tagID)
        {
            throw new NotImplementedException();
        }

        public void UpdateTag(TagDto tag, int customerID)
        {
            Contract.Requires<TagServicesException>(tag != null);
            Contract.Requires<TagServicesException>(tag.Title != null);
            Contract.Requires<TagServicesException>(tag.Description != null);
        }

        public void SaveOrUpdateTag(TagDto tag, int customerID)
        {
            Contract.Requires<TagServicesException>(tag != null);
            Contract.Requires<TagServicesException>(tag.Title != null);
            Contract.Requires<TagServicesException>(tag.Description != null);
        }

        public void RemoveTag(int tagID, int customerID)
        {
            throw new NotImplementedException();
        }


        public List<TagDto> GetStandardTags()
        {
            throw new NotImplementedException();
        }
    }
}
