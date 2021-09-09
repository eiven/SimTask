using System;
using System.Collections.Generic;

namespace SimTask
{
  /// <summary>
  /// The task scheduler is used to work on tasks per tick. 
  /// </summary>
  public class TaskScheduler
  {
    public List<ITask> Tasks = new List<ITask>();
    private bool taskProgressChanged;

    public float SimulationTime { get; set; }

    public event TaskAddedEventHandler OnTaskAdded;

    public delegate void TaskAddedEventHandler(object sender, ITask task);
    public delegate void TaskHandlerRegistered(object sender, ITaskHandler taskHandler);

    public bool AddTask(ITask task)
    {
      this.Tasks.Add(task);
      task.OnProgressChanged += this.OnProgressChanged;
      task.OnTaskFinished += this.OnTaskFinished;
      this.OnTaskAdded?.Invoke(this, task);
      return true;
    }

    void OnProgressChanged(object sender, EventArgs args)
    {
      var task = (ITask)sender;
      this.taskProgressChanged = true;
    }

    void OnTaskFinished(object sender, EventArgs args)
    {
      var task = (ITask)sender;
      this.Tasks.Remove(task);
    }

    public void Tick(float deltaTime)
    {
      this.taskProgressChanged = true;
      foreach (var task in this.Tasks)
      {
        task.GetTaskHandler().SetTimeAccount(deltaTime);
      }

      while (this.taskProgressChanged)
      {
        this.taskProgressChanged = false;

        var tasks = this.Tasks.ToArray();
        foreach (var task in tasks)
        {
          if (task.GetProgress() < 1.0f)
          {
            if (task.GetParentTask() == null)
            {
              this.DoWork(task, task.GetTaskHandler().GetTimeToWorkOnTask(task));
            }
          }
        }
      }

      SimulationTime += deltaTime;
    }

    private void DoWork(ITask task, float timeToWorkOn)
    {
      if (task.GetChildTasks().Count > 0)
      {
        switch (task.ChildMode)
        {
          case TaskChildMode.Sequentiell:
            {
              ITask taskToWorkOnSequentiell = null;
              foreach (var childTask in task.GetChildTasks())
              {
                if (childTask.GetProgress() < 1.0f)
                {
                  taskToWorkOnSequentiell = childTask;
                  break;
                }
              }

              if (taskToWorkOnSequentiell != null)
              {
                float childTimeToWorkOn = taskToWorkOnSequentiell.GetTimeToWorkOn();
                DoWork(taskToWorkOnSequentiell, childTimeToWorkOn);
              }
              break;
            }

          case TaskChildMode.Simultaneously:
            {
              float timeToWorkSimultaneously = float.MaxValue;
              foreach (var childTask in task.GetChildTasks())
              {
                float timeToWorkOnMin = childTask.GetTimeToWorkOn();
                if (timeToWorkOnMin < timeToWorkSimultaneously)
                {
                  timeToWorkSimultaneously = timeToWorkOnMin;
                }
              }

              if (timeToWorkSimultaneously > 0)
              {
                foreach (var childTask in task.GetChildTasks())
                {
                  DoWork(childTask, timeToWorkSimultaneously);
                }
              }

              break;
            }

          case TaskChildMode.SideBySide:
            {
              foreach (var childTask in task.GetChildTasks())
              {
                DoWork(childTask, childTask.GetTimeToWorkOn());
              }
              break;
            }
        }
      }

      if (timeToWorkOn > task.GetTaskHandler().GetTimeAccount())
      {
        timeToWorkOn = task.GetTaskHandler().GetTimeAccount();
      }

      if (timeToWorkOn >= 0)
      {
        task.Execute(timeToWorkOn);
      }
    }
  }
}
