using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTask
{
  public class TaskHandler : ITaskHandler
  {
    private float timeAccount;

    /// <summary>
    /// Event occurs after a new task was added.
    /// </summary>
    public event TaskAddedEventHandler OnTaskAdded;

    /// <summary>
    /// Event occurs after a task was removed.
    /// </summary>
    public event TaskRemovedEventHandler OnTaskRemoved;

    /// <summary>
    /// Event occurs after a task was handled.
    /// </summary>
    public event TaskHandledEventHandler OnTaskHandled;

    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the task queue that is organizing the taks.
    /// </summary>
    public TaskQueue TaskQueue { get; set; } = new TaskQueue();

    public List<ITask> Tasks { get; set; } = new List<ITask>();

    /// <summary>
    /// Sets the time account value.
    /// </summary>
    /// <param name="value">Time account value.</param>
    public void SetTimeAccount(float value)
    {
      this.timeAccount = value;
    }

    /// <summary>
    /// Gets the time account.
    /// </summary>
    /// <returns>Time account.</returns>
    public float GetTimeAccount()
    {
      return this.timeAccount;
    }

    /// <summary>
    /// Gets the time to work on task.
    /// </summary>
    /// <param name="task">Task to work on.</param>
    /// <returns>Time to work on task.</returns>
    public float GetTimeToWorkOnTask(ITask task)
    {
      if (this.TaskQueue.IsTaskReachable(task))
      {
        return task.GetChildTasks().All(x => x.IsFinished()) ? this.timeAccount : 0.0f;
      }

      return 0.0f;
    }

    /// <summary>
    /// Gets the efficiency on a task.
    /// This could be used to calculate different time costs per task handler.
    /// </summary>
    /// <param name="task">Task.</param>
    /// <returns>Efficiency from 0.0f to 1.0f.</returns>
    public float GetEfficiencyFactorOnTask(ITask task)
    {
      return 1.0f;
    }

    public void HandleTask(ITask task, float deltaTime)
    {
      this.timeAccount -= deltaTime;
      task.InvestedTime += deltaTime;
      this.OnTaskHandled?.Invoke(this, task);
    }

    /// <summary>
    /// Adding a task to handler.
    /// </summary>
    /// <param name="task">Task to add.</param>
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

    protected void OnTaskFinished(object sender, EventArgs args)
    {
      var task = (ITask)sender;
      this.RemoveTask(task);
    }

    protected void OnTaskStarted(object sender, EventArgs args)
    {
    }

    /// <summary>
    /// Remove a task from handler.
    /// </summary>
    /// <param name="task">Task to remove.</param>
    public void RemoveTask(ITask task)
    {
      this.Tasks.Remove(task);
      this.OnTaskRemoved?.Invoke(this, task);
    }
  }
}
