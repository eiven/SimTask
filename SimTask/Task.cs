using System;
using System.Collections.Generic;
using System.Text;

namespace SimTask
{
  public class Task : ITask
  {
    private float progress = 0.0f;

    private ITask parent { get; set; }

    private ITaskHandler taskHandler { get; set; }

    public float TimeRunning { get; set; }

    public float TimeCosts { get; set; }

    public float InvestedTime { get; set; }

    public IList<ITask> ChildTasks { get; set; } = new List<ITask>();

    public TaskChildMode ChildMode { get; set; }

    public string Name { get; set; }

    public event EventHandler OnParentTaskChanged;

    public event EventHandler OnTaskFinished;

    public event EventHandler OnTaskStarted;

    public event EventHandler OnProgressChanged;

    public void AddChildTask(ITask task)
    {
      if (this.GetProgress() > 0.0f && this.ChildMode == TaskChildMode.Simultaneously)
      {
        return;
      }

      task.SetParentTask(this);
      this.ChildTasks.Add(task);
    }

    public void Execute(float deltaTime)
    {
      this.GetTaskHandler().HandleTask(this, deltaTime);
      this.UpdateProgress();
    }

    public ITask GetParentTask()
    {
      return this.parent;
    }

    public float GetProgress()
    {
      return progress;
    }

    public float GetTimeToWorkOn()
    {
      if (this.progress >= 1)
      {
        return 0.0f;
      }

      float timeToWorkOn = this.GetTaskHandler() != null ? this.GetTaskHandler().GetTimeToWorkOnTask(this) : 0.0f;

      if (timeToWorkOn > 0 && timeToWorkOn > this.TimeCosts - this.InvestedTime)
      {
        return this.TimeCosts - this.InvestedTime;
      }

      return timeToWorkOn;
    }

    public void SetParentTask(ITask task)
    {
      this.parent = task;
      this.OnParentTaskChanged?.Invoke(this, new EventArgs());
    }

    public void SetProgress(float value)
    {
      if (value == this.progress)
      {
        return;
      }

      if (value > 0.0F)
      {
        this.OnTaskStarted?.Invoke(this, new EventArgs());
      }

      this.progress = value;
      this.OnProgressChanged?.Invoke(this, new EventArgs());
      if (this.GetProgress() >= 1.0f)
      {
        this.OnTaskFinished?.Invoke(this, new EventArgs());
      }
    }

    protected void UpdateProgress()
    {
      float progressCalculated = 0.0f;
      if (this.ChildTasks.Count > 0)
      {
        float progressSum = 0.0f;
        float investedTimeSum = 0.0f;
        float timeCostsSum = 0.0f;
        foreach (var task in this.ChildTasks)
        {
          investedTimeSum += task.InvestedTime;
          timeCostsSum += task.TimeCosts;
          progressSum += task.GetProgress();
        }

        progressCalculated = progressSum / this.ChildTasks.Count;
        if (progressCalculated > 1.0f)
        {
          progressCalculated = 1.0f;
        }

        this.InvestedTime = investedTimeSum;
        this.TimeCosts = timeCostsSum;
        progressCalculated = progressSum / this.ChildTasks.Count;
      }
      else
        progressCalculated = this.TimeCosts <= 0 ? 0 : this.InvestedTime / this.TimeCosts;

      if (progressCalculated > 1.0f)
      {
        progressCalculated = 1.0f;
      }

      this.SetProgress(progressCalculated);
    }

    public ITaskHandler GetTaskHandler()
    {
      return this.taskHandler;
    }

    public void SetTaskHandler(ITaskHandler taskHandler)
    {
      this.taskHandler = taskHandler;
    }
  }
}
