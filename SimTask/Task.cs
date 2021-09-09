using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTask
{
  /// <summary>
  /// A task is a piece of work that has to be finished
  /// by a specific task handler (worker) and generates a certain amount of work. 
  /// </summary>
  public class Task : ITask
  {
    /// <summary>
    /// Progress for the task.
    /// </summary>
    private float progress = 0.0f;

    /// <summary>
    /// Time costs for the task.
    /// </summary>
    private float timeCosts;

    /// <summary>
    /// Backreference to the parent task.
    /// </summary>
    private ITask parent { get; set; }

    /// <summary>
    /// Task handler.
    /// </summary>
    private ITaskHandler taskHandler { get; set; }

    /// <summary>
    /// List of child tasks.
    /// </summary>
    private IList<ITask> childTasks { get; set; } = new List<ITask>();

    /// <summary>
    /// Invested time on the task.
    /// </summary>
    public float InvestedTime { get; set; }

    /// <summary>
    /// Gets <see cref="TaskChildMode"/> for the <see cref="ITask.ChildTasks"/>.
    /// </summary>
    public TaskChildMode ChildMode { get; set; }

    /// <summary>
    /// Gets or sets the task name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Event is invoked after the parent task was changed.
    /// </summary>
    public event EventHandler OnParentTaskChanged;

    /// <summary>
    /// Event is invoked after the task was finished.
    /// </summary
    public event EventHandler OnTaskFinished;

    /// <summary>
    /// Event is invoked after the task was started.
    /// </summary>
    public event EventHandler OnTaskStarted;

    /// <summary>
    /// Event is invoked after the progress of the task was changed.
    /// </summary>
    public event EventHandler OnProgressChanged;

    /// <summary>
    /// Adding a <see cref="ITask"/> as child.
    /// </summary>
    /// <param name="task"></param>
    public void AddChildTask(ITask task)
    {
      if (this.GetProgress() > 0.0f && this.ChildMode == TaskChildMode.Simultaneously)
      {
        return;
      }

      task.SetParentTask(this);
      task.OnProgressChanged += this.OnChildTaskProgressChanged;
      this.childTasks.Add(task);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnChildTaskProgressChanged(object sender, EventArgs e)
    {
      if (this.IsFinished())
      {
        this.OnTaskFinished?.Invoke(this, new EventArgs());
      }
    }

    /// <summary>
    /// Executes the task for a given <paramref name="timeToWorkOnTask"/>.
    /// </summary>
    /// <param name="timeToWorkOnTask">Time to work on task.</param>
    public void Execute(float deltaTime)
    {
      this.GetTaskHandler().HandleTask(this, deltaTime);
      this.UpdateProgress();
    }

    /// <summary>
    /// Gets the parent <see cref="ITask"/>.
    /// </summary>
    /// <returns>Parent <see cref="ITask"/> or null.</returns>
    public ITask GetParentTask()
    {
      return this.parent;
    }

    /// <summary>
    /// Gets the current progress of the <see cref="ITask"/>.
    /// </summary>
    /// <returns></returns>
    public float GetProgress()
    {
      return progress;
    }

    /// <summary>
    /// Gets the time that can be worked on the <see cref="ITask"/>.
    /// </summary>
    /// <returns>Time to work on task.</returns>
    public float GetTimeToWorkOn()
    {
      if (this.progress >= 1)
      {
        return 0.0f;
      }

      float timeToWorkOn = this.GetTaskHandler() != null ? this.GetTaskHandler().GetTimeToWorkOnTask(this) : 0.0f;

      if (timeToWorkOn > 0 && timeToWorkOn > this.GetTimeCosts() - this.InvestedTime)
      {
        return this.GetTimeCosts() - this.InvestedTime;
      }

      return timeToWorkOn;
    }

    /// <summary>
    /// Sets the parent task.
    /// </summary>
    /// <param name="task"></param>
    public void SetParentTask(ITask task)
    {
      this.parent = task;
      this.OnParentTaskChanged?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Sets the progress of the <see cref="ITask"/>.
    /// </summary>
    /// <param name="value">Progress.</param>
    public void SetProgress(float value)
    {
      if (value == this.progress)
      {
        return;
      }

      if (value > 0.0F && this.GetProgress() == 0.0f)
      {
        this.OnTaskStarted?.Invoke(this, new EventArgs());
      }

      this.progress = value;
      this.OnProgressChanged?.Invoke(this, new EventArgs());
      if (this.IsFinished())
      {
        this.OnTaskFinished?.Invoke(this, new EventArgs());
      }
    }

    /// <summary>
    /// Update progess.
    /// </summary>
    protected void UpdateProgress()
    {
      float progressCalculated = 0.0f;
      progressCalculated = this.GetTimeCosts() <= 0 ? 0 : this.InvestedTime / this.GetTimeCosts();

      if (progressCalculated > 1.0f)
      {
        progressCalculated = 1.0f;
      }

      this.SetProgress(progressCalculated);
    }

    /// <summary>
    /// Gets the <see cref="ITaskHandler"/> of the task.
    /// </summary>
    public ITaskHandler GetTaskHandler()
    {
      return this.taskHandler;
    }

    /// <summary>
    /// Sets the <see cref="ITaskHandler"/> of the task.
    /// </summary>
    public void SetTaskHandler(ITaskHandler taskHandler)
    {
      this.taskHandler = taskHandler;
    }

    /// <summary>
    /// Gets the time costs for the task.
    /// </summary>
    public float GetTimeCosts()
    {
      return this.timeCosts;
    }

    /// <summary>
    /// Sets the time costs for the task;
    /// </summary>
    /// <param name="timeCosts">Time costs.</param>
    public void SetTimeCosts(float timeCosts)
    {
      this.timeCosts = timeCosts;
    }

    /// <summary>
    /// Get a list of child <see cref="ITask"/>s.
    /// </summary>
    /// <returns>All child tasks.</returns>
    public IList<ITask> GetChildTasks()
    {
      return this.childTasks;
    }

    /// <summary>
    /// Allows to check if a task was finished.
    /// </summary>
    /// <returns>True if the task was finished.</returns>
    public bool IsFinished()
    {
      return this.GetChildTasks().All(x => x.IsFinished()) && this.GetProgress() >= 1.0f;
    }
  }
}
