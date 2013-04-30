using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.Core.DataAccess
{
    public interface ITaskRepository
    {
        List<Task> GetTasksForAdvisor(int advisorID);
    }
}
