using System;
using System.Collections.Generic;
using System.Text;

namespace SimTask
{
  public class TaskHandler : ITaskHandler
  {
    /// <summary>
    /// Event occurs after a new task was added.
    /// </summary>
    public event TaskAddedEventHandler OnTaskAdded;

    /// <summary>
    /// Event occurs after a task was removed.
    /// </summary>
    public event TaskRemovedEventHandler OnTaskRemoved;

    public string Name { get; set; }

    public float TimeAccount { get; set; }

    /// <summary>
    /// Gets or sets the task queue that is organizing the taks.
    /// </summary>
    public TaskQueue TaskQueue { get; set; } = new TaskQueue();

    public List<ITask> Tasks { get; set; } = new List<ITask>();

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

    public void HandleTask(ITask task, float deltaTime)
    {
      this.TimeAccount -= deltaTime;
      task.InvestedTime += deltaTime;
    }

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

    public void RemoveTask(ITask task)
    {
      this.Tasks.Remove(task);
      this.OnTaskRemoved?.Invoke(this, task);
    }
  }
}
