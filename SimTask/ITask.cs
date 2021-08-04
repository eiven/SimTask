using System;
using System.Collections.Generic;

namespace SimTask
{
  /// <summary>
  /// Enum for setting the mode for all child|subordinate tasks.
  /// </summary>
  public enum TaskChildMode : uint
  {
    /// <summary>
    /// All child tasks are running in a sequence on by another.
    /// </summary>
    Sequentiell = 1,

    /// <summary>
    /// All child tasks are running simultaneously.
    /// That means the progress of the taks will be equal.
    /// </summary>
    Simultaneously = 2,

    /// <summary>
    /// All child task are running side by side.
    /// That means all taks can have different progress.
    /// </summary>
    SideBySide = 3,
  }

  /// <summary>
  /// Basic interface defining a task.
  /// A task is a piece of work that has to be finished
  /// by a specific task handler (worker) and generates a certain amount of work.
  /// </summary>
  public interface ITask
  {
    /// <summary>
    /// Event is invoked after the parent task was changed.
    /// </summary>
    event EventHandler OnParentTaskChanged;

    /// <summary>
    /// Event is invoked after the task was finished.
    /// </summary>
    event EventHandler OnTaskFinished;

    /// <summary>
    /// Event is invoked after the task was started.
    /// </summary>
    event EventHandler OnTaskStarted;

    /// <summary>
    /// Event is invoked after the progress of the task was changed.
    /// </summary>
    event EventHandler OnProgressChanged;

    /// <summary>
    /// Gets the time costs for the task.
    /// </summary>
    float GetTimeCosts();

    /// <summary>
    /// Sets the time costs for the task;
    /// </summary>
    /// <param name="timeCosts">Time costs.</param>
    void SetTimeCosts(float timeCosts);

    /// <summary>
    /// Invested time on the task.
    /// </summary>
    float InvestedTime { get; set; }

    /// <summary>
    /// Gets the current progress of the <see cref="ITask"/>.
    /// </summary>
    /// <returns></returns>
    float GetProgress();

    /// <summary>
    /// Gets the time that can be worked on the <see cref="ITask"/>.
    /// </summary>
    /// <returns>Time to work on task.</returns>
    float GetTimeToWorkOn();

    /// <summary>
    /// Sets the progress of the <see cref="ITask"/>.
    /// </summary>
    /// <param name="value">Progress.</param>
    void SetProgress(float value);

    /// <summary>
    /// Gets <see cref="TaskChildMode"/> for the <see cref="ITask.ChildTasks"/>.
    /// </summary>
    TaskChildMode ChildMode { get; set; }

    /// <summary>
    /// Adding a <see cref="ITask"/> as child.
    /// </summary>
    /// <param name="task"></param>
    void AddChildTask(ITask task);

    /// <summary>
    /// Get a list of child <see cref="ITask"/>s.
    /// </summary>
    /// <returns>All child tasks.</returns>
    IList<ITask> GetChildTasks();

    /// <summary>
    /// Sets the parent task.
    /// </summary>
    /// <param name="task"></param>
    void SetParentTask(ITask task);

    /// <summary>
    /// Executes the task for a given <paramref name="timeToWorkOnTask"/>.
    /// </summary>
    /// <param name="timeToWorkOnTask">Time to work on task.</param>
    void Execute(float timeToWorkOnTask);

    /// <summary>
    /// Gets the parent <see cref="ITask"/>.
    /// </summary>
    /// <returns>Parent <see cref="ITask"/> or null.</returns>
    ITask GetParentTask();

    /// <summary>
    /// Gets the <see cref="ITaskHandler"/> of the task.
    /// </summary>
    ITaskHandler GetTaskHandler();

    /// <summary>
    /// Sets the <see cref="ITaskHandler"/> of the task.
    /// </summary>
    void SetTaskHandler(ITaskHandler taskHandler);

    /// <summary>
    /// Gets or sets the task name.
    /// </summary>
    string Name { get; set; }
  }
}
