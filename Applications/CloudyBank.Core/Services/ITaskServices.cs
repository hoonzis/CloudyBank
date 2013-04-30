using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Dto;

namespace CloudyBank.Core.Services
{
    public interface ITaskServices
    {
        /// <summary>
        /// Returns list of all task for given advisor
        /// </summary>
        /// <param name="advisorID">Id of the advisor</param>
        /// <returns></returns>
        IList<TaskDto> GetTasksForAdvisor(int advisorID);
    }
}
