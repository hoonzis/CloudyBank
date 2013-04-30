using System;
namespace CloudyBank.Core.Categorization
{
    public interface ICategorizationServices
    {
        bool CategorizePayments(int customerID);
    }
}
