using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Dto;

namespace CloudyBank.Services.DtoCreators
{
    public class UserIdentityDtoCreator : IDtoCreator<UserIdentity, UserIdentityDto>
    {
        public UserIdentityDto Create(UserIdentity poco)
        {
            UserIdentityDto userDto = new UserIdentityDto();
            userDto.Id = poco.Id;
            userDto.Identification = poco.Identification;
            userDto.UserType = poco.UserType;
            userDto.Email = poco.Email;
            return userDto;
        }
    }
}
