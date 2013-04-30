using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.DataAccess;
using CloudyBank.Dto;

using CloudyBank.CoreDomain.Customers;
using System.Transactions;
using CloudyBank.Core.Dto;

namespace CloudyBank.Services
{
    public class TagServices : ITagServices
    {
        IRepository _repository;
        ITagRepository _tagRepository;
        IDtoCreator<Tag, TagDto> _tagDtoCreator;

        public TagServices(IRepository repository, ITagRepository tagRepository, IDtoCreator<Tag, TagDto> tagDtoCreator)
        {
            _repository = repository;
            _tagRepository = tagRepository;
            _tagDtoCreator = tagDtoCreator;
        }

        public IList<TagDto> GetTagsForCustomer(int customerID)
        {
            var tags = _tagRepository.GetTagsForCustomer(customerID);
            if (tags != null)
            {
                return tags.Select(x => _tagDtoCreator.Create(x)).ToList();
            }
            return null;
        }

        public void CreateTag(int customerID, TagDto tagDto)
        {
            UserTag tag = new UserTag();
            tag.Name = tagDto.Title;
            tag.Description = tagDto.Description;

            Customer customer = _repository.Load<Customer>(customerID);
            if (customer != null)
            {
                customer.Tags.Add(tag);
            }

            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Save<Tag>(tag);
                _repository.Update<Customer>(customer);
                scope.Complete();
            }
        }


        public void TagOperation(int operationID, int tagID)
        {
            var operation = _repository.Get<Operation>(operationID);
            var tag = _repository.Get<Tag>(tagID);

            if(operation!=null && tag!=null){
                operation.Tag = tag;
                using (TransactionScope scope = new TransactionScope())
                {
                    _repository.Update(operation);
                    scope.Complete();
                }
            }
        }


        public void UpdateTag(TagDto tagDto, int customerID)
        {
            Tag tag = _repository.Get<Tag>(tagDto.Id);
            tag.Name = tagDto.Title;
            tag.Description = tagDto.Description;

            
            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Update(tag);
                _repository.Flush();
                scope.Complete();
            }
        }

        public void SaveOrUpdateTag(TagDto tagDto,int customerID)
        {
            Tag tag = _repository.Get<Tag>(tagDto.Id);
            if (tag == null)
            {
                CreateTag(customerID, tagDto);
            }
            
            tag.Name = tagDto.Title;
            tag.Description = tagDto.Description;
            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Update(tag);
                _repository.Flush();
                scope.Complete();
            }   
        }

        public void RemoveTag(int tagID, int customerID)
        {
            Customer customer = _repository.Load<Customer>(customerID);
            if (customer == null)
            {
                throw new TagServicesException("Customer not found");
            }

            UserTag tag = _repository.Get<UserTag>(tagID);
            if (tag == null)
            {
                throw new TagServicesException("Tag not found");
            }

            customer.Tags.Remove(tag);

            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Update(customer);
                _repository.Flush();
                scope.Complete();
            } 
        }


        public List<TagDto> GetStandardTags()
        {
            var tags = _repository.GetAll<StandardTag>();
            if (tags != null)
            {
                return tags.Select(x => new TagDto() { Description = x.Description, Title = x.Name, Id = x.Id }).ToList();
            }
            return null;
        }
    }
}
