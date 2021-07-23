using System;
using System.Collections.Generic;
using System.Text;

namespace SimTask
{
  public interface ITaskHandler
  {
    event TaskAddedEventHandler OnTaskAdded;

    event TaskRemovedEventHandler OnTaskRemoved;

    string Name { get; set; }

    TaskQueue TaskQueue { get; set; }

    float TimeAccount { get; set; }

    void HandleTask(ITask task, float deltaTime);

    float GetTimeToWorkOnTask(Task task);

    void AddTask(ITask task);

    void RemoveTask(ITask task);
  }
}
