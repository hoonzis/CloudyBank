using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Services.DtoCreators
{
    public class TagDtoCreator : IDtoCreator<Tag,TagDto>
    {
        public TagDto Create(Tag tag)
        {
            TagDto tagDto = new TagDto();
            tagDto.Id = tag.Id;
            tagDto.Title = tag.Name;
            tagDto.Description = tag.Description;
            return tagDto;
        }
    }
}
