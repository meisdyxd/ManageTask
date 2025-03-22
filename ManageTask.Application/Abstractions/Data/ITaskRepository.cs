﻿using ManageTask.Domain;
using ResultSharp.Core;
namespace ManageTask.Application.Abstractions.Data
{
    public interface ITaskRepository: IRepository<Domain.Task, Guid>
    {
    }
}
