using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.Dto;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;

using CloudyBank.CoreDomain.Advisors;
using CloudyBank.Core.Dto;

namespace CloudyBank.Services
{
    public class TaskServices : ITaskServices
    {
        private readonly IRepository _repository;
        private readonly ITaskRepository _taskRepository;
        private readonly IDtoCreator<Task,TaskDto> _taskDtoCreator;

        public TaskServices(IRepository repository, ITaskRepository taskRepository, IDtoCreator<Task, TaskDto> taskDtoCreator)
        {
            _repository = repository;
            _taskRepository = taskRepository;
            _taskDtoCreator = taskDtoCreator;
        }

        public IList<TaskDto> GetTasksForAdvisor(int advisorID)
        {
            IList<Task> tasks = _taskRepository.GetTasksForAdvisor(advisorID);
            if (tasks != null)
            {
                return tasks.Select(_taskDtoCreator.Create).ToList();
            }
            return null;
        }
    }
}
