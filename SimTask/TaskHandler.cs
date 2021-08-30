using System;
using System.Collections.Generic;

namespace SimTask
{
  /// <summary>
  /// A task handler is representation of someone or something that can handle tasks.
  /// </summary>
  public class TaskHandler : ITaskHandler
  {
    /// <summary>
    /// Stores the <see cref="ITask"/>s handled by the task handler. 
    /// </summary>
    private List<ITask> Tasks = new List<ITask>();

    /// <summary>
    /// Event occurs after a new task was added.
    /// </summary>
    public event TaskAddedEventHandler OnTaskAdded;

    /// <summary>
    /// Event occurs after a task was removed.
    /// </summary>
    public event TaskRemovedEventHandler OnTaskRemoved;

    /// <summary>
    /// Gets or sets the name of the task handler.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the time account for the task handler.
    /// </summary>
    public float TimeAccount { get; set; }

    /// <summary>
    /// Gets or sets the task queue that is organizing the taks.
    /// </summary>
    public TaskQueue TaskQueue { get; set; } = new TaskQueue();

    /// <summary>
    /// Gets the time the worker has available for the task.
    /// </summary>
    /// <param name="task">Task to work on.</param>
    /// <returns>Time to work on task.</returns>
    public float GetTimeToWorkOnTask(Task task)
    {
      if (this.TaskQueue.IsTaskReachable(task))
      {
        if (task.GetChildTasks().Count > 0)
        {
          return 0.0f;
        }

        return this.TimeAccount;
      }

      return 0.0f;
    }

    /// <summary>
    /// Handle task.
    /// </summary>
    /// <param name="task">Task.</param>
    /// <param name="timeToWorkOnTask">Time to work on task.</param>
    public void HandleTask(ITask task, float timeToWorkOnTask)
    {
      this.TimeAccount -= timeToWorkOnTask;
      task.InvestedTime += timeToWorkOnTask;
    }

    /// <summary>
    /// Adds a task to the handler.
    /// </summary>
    /// <param name="task">Task.</param>
    public void AddTask(ITask task)
    {
      if (task == null)
      {
        return;
      }

      task.SetTaskHandler(this);
      task.OnTaskFinished += this.OnTaskFinished;
      task.OnTaskStarted += this.OnTaskStarted;
      this.Tasks.Add(task);
      this.TaskQueue.AddTask(task);
      this.OnTaskAdded?.Invoke(this, task);
    }

    /// <summary>
    /// Removes a task from the handler.
    /// </summary>
    /// <param name="task">Task.</param>
    public void RemoveTask(ITask task)
    {
      this.Tasks.Remove(task);
      this.OnTaskRemoved?.Invoke(this, task);
    }

    /// <summary>
    /// On task finished.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="eventArgs">Event args.</param>
    protected virtual void OnTaskFinished(object sender, EventArgs eventArgs)
    {
      var task = (ITask)sender;
      this.RemoveTask(task);
    }

    /// <summary>
    /// On task started.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected virtual void OnTaskStarted(object sender, EventArgs args)
    {
    }
  }
}
