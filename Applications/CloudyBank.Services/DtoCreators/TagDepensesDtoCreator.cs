using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;

namespace CloudyBank.Services.DtoCreators
{
    public class TagDepensesDtoCreator : IDtoCreator<TagDepenses, TagDepensesDto>
    {
        public TagDepensesDto Create(TagDepenses poco)
        {
            TagDepensesDto dto = new TagDepensesDto();
            dto.TagName = poco.Tag.Name;
            dto.TagId = poco.Tag.Id;
            dto.Depenses = poco.Depenses;

            return dto;
        }
    }
}
