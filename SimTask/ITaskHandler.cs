using System;
using System.Collections.Generic;
using System.Text;

namespace SimTask
{
  /// <summary>
  /// Interface defining the minimum set of methods needed for task handlers.
  /// A task handler is representation of someone or something that can handle tasks.
  /// </summary>
  public interface ITaskHandler
  {
    /// <summary>
    /// Event occurs after a new task was added.
    /// </summary>
    event TaskAddedEventHandler OnTaskAdded;

    /// <summary>
    /// Event occurs after a task was removed.
    /// </summary>
    event TaskRemovedEventHandler OnTaskRemoved;

    /// <summary>
    /// Gets or sets the name of the task handler.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the task queue that is organizing the taks.
    /// </summary>
    TaskQueue TaskQueue { get; set; }

    /// <summary>
    /// Gets or sets the time account for the task handler.
    /// </summary>
    float TimeAccount { get; set; }

    /// <summary>
    /// Handle task.
    /// </summary>
    /// <param name="task">Task.</param>
    /// <param name="deltaTime">Delta time.</param>
    void HandleTask(ITask task, float deltaTime);

    float GetTimeToWorkOnTask(Task task);

    void AddTask(ITask task);

    void RemoveTask(ITask task);
  }
}
