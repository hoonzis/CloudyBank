using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using CloudyBank.Core.Services;

namespace CloudyBank.Core.Dto
{
    [ContractClass(typeof(DtoCreatorContracts<,>))]
    public interface IDtoCreator<T, T2>
    {
        T2 Create(T poco);
    }

    [ContractClassFor(typeof(IDtoCreator<,>))]
    public abstract class DtoCreatorContracts<T, T2> : IDtoCreator<T, T2>
    {

        T2 IDtoCreator<T, T2>.Create(T poco)
        {
            Contract.Requires<DtoCreationException>(poco != null);
            return default(T2);
        }
    }
}
