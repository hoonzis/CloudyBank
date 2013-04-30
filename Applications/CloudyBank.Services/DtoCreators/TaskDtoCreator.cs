using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.Services.DtoCreators
{
    public class TaskDtoCreator : IDtoCreator<Task, TaskDto>
    {
        public TaskDto Create(Task task)
        {
            TaskDto taskDto = new TaskDto();
            taskDto.Id = task.Id;
            taskDto.Title = task.Title;
            taskDto.Descritpion = task.Descritpion;
            return taskDto;
        }
    }
}
