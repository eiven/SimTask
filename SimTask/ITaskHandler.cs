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
    /// Sets the time account value.
    /// </summary>
    /// <param name="value">Time account value.</param>
    void SetTimeAccount(float value);

    /// <summary>
    /// Gets the time account.
    /// </summary>
    /// <returns>Time account.</returns>
    float GetTimeAccount();

    /// <summary>
    /// Handle task.
    /// </summary>
    /// <param name="task">Task.</param>
    /// <param name="timeToWorkOnTask">Time to work on task.</param>
    void HandleTask(ITask task, float timeToWorkOnTask);

    /// <summary>
    /// Gets the time the worker has available for the task.
    /// </summary>
    /// <param name="task">Task to work on.</param>
    /// <returns>Time to work on task.</returns>
    float GetTimeToWorkOnTask(ITask task);

    /// <summary>
    /// Gets the efficiency on a task.
    /// This could be used to calculate different time costs per task handler.
    /// </summary>
    /// <param name="task">Task.</param>
    /// <returns>Efficiency from 0.0f to 1.0f.</returns>
    float GetEfficiencyFactorOnTask(ITask task);

    /// <summary>
    /// Adding a task to handler.
    /// </summary>
    /// <param name="task">Task to add.</param>
    void AddTask(ITask task);

    /// <summary>
    /// Remove a task from handler.
    /// </summary>
    /// <param name="task">Task to remove.</param>
    void RemoveTask(ITask task);
  }
}
