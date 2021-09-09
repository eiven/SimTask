using System;
using System.Collections.Generic;
using System.Text;

namespace SimTask
{
  public class TaskQueue
  {
    public TaskQueue()
    {
      ITask rootTask = new Task();
      rootTask.Name = "Root task UESTaskQueue";
      rootTask.ChildMode = TaskChildMode.Sequentiell;
      this.rootNode = new TaskQueueTreeNode();
      this.rootNode.Value = rootTask;
    }

    public TaskQueueTreeNode rootNode { get; set; }

    public Dictionary<ITask, TaskQueueTreeNode> Nodes { get; set; } = new Dictionary<ITask, TaskQueueTreeNode>();


    /// <summary>
    /// 
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public bool IsTaskReachable(ITask task)
    {
      return this.IsTaskReachable(task, this.rootNode);
    }

    public bool AddTaskToNode(ITask task, TaskQueueTreeNode currentNode)
    {
      TaskQueueTreeNode treeNode = new TaskQueueTreeNode();
      treeNode.Value = task;
      currentNode.AddNode(treeNode);
      this.Nodes.Add(task, treeNode);

      foreach (ITask child in task.GetChildTasks())
      {
        if (this.Nodes.ContainsKey(child))
        {
          TaskQueueTreeNode childTreeNode = this.Nodes[child];
          if (childTreeNode.PreviousNode != null)
          {
            childTreeNode.PreviousNode.RemoveNode(childTreeNode);
            treeNode.AddNode(childTreeNode);
          }
        }
      }

      return true;
    }

    public void AddTask(ITask task)
    {
      task.OnParentTaskChanged += this.OnParentTaskChanged;
      task.OnTaskFinished += this.OnTaskFinished;

      if (task.GetParentTask() != null && this.Nodes.ContainsKey(task.GetParentTask()))
      {
        this.AddTaskToNode(task, this.Nodes[task.GetParentTask()]);
      }
      else
        this.AddTaskToNode(task, this.rootNode);
    }

    /// <summary>
    /// If a task is subordinated to another task, the position of the task in the queue also changes.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="eventArgs">Event args.</param>
    public void OnParentTaskChanged(object sender, EventArgs eventArgs)
    {
      var task = (ITask)sender;
      if (this.Nodes.ContainsKey(task))
      {
        TaskQueueTreeNode treeNode = this.Nodes[task];
        if (treeNode.PreviousNode != null && !treeNode.PreviousNode.Value.Equals(task.GetParentTask()))
        {
          treeNode.PreviousNode.RemoveNode(treeNode);
        }

        if (this.Nodes.ContainsKey(task.GetParentTask()))
        {
          var previousNode = this.Nodes[task.GetParentTask()];
          previousNode.AddNode(treeNode);
        }
        else
        {
          this.rootNode.AddNode(treeNode);
        }
      }
    }

    /// <summary>
    /// After a task is finished the task could be removed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public void OnTaskFinished(object sender, EventArgs eventArgs)
    {
      var task = (ITask)sender;
      this.RemoveTask(task);
    }

    /// <summary>
    /// Removes <paramref name="task"/> from queue by removing the related <see cref="TaskQueueTreeNode"/>.
    /// </summary>
    /// <param name="task"></param>
    public void RemoveTask(ITask task)
    {
      if (this.Nodes.ContainsKey(task))
      {
        TaskQueueTreeNode treeNode = this.Nodes[task];
        this.RemoveTreeNode(treeNode);
      }
    }

    /// <summary>
    /// Removes <paramref name="treeNode"/> from <see cref="TaskQueue.Nodes"/>.
    /// </summary>
    /// <param name="treeNode"></param>
    public void RemoveTreeNode(TaskQueueTreeNode treeNode)
    {
      foreach (TaskQueueTreeNode childTreeNode in treeNode.GetNodes())
      {
        this.RemoveTreeNode(childTreeNode);
      }

      if (treeNode.PreviousNode != null)
      {
        treeNode.PreviousNode.RemoveNode(treeNode);
      }

      this.Nodes.Remove(treeNode.Value);
      treeNode.Value = null;
      treeNode.PreviousNode = null;
    }

    /// <summary>
    /// The queue uses a graph to represent the queue.
    /// To find out if a given <paramref name="task"/> is reachable we loop through the <see cref="TaskQueueTreeNode"/>s
    /// object tree recursively.
    /// </summary>
    /// <param name="task">Task to check.</param>
    /// <param name="currentNode">Current node in object tree to be checked.</param>
    /// <returns></returns>
    private bool IsTaskReachable(ITask task, TaskQueueTreeNode currentNode)
    {
      if (currentNode.Value == task)
      {
        return true;
      }

      foreach (TaskQueueTreeNode node in currentNode.GetNodes())
      {
        if (currentNode.Value.ChildMode == TaskChildMode.Sequentiell)
        {
          if (node.Value != null && !node.Value.IsFinished())
          {
            return this.IsTaskReachable(task, node);
          }
        }
        else if (currentNode.Value.ChildMode == TaskChildMode.Simultaneously)
        {
          return this.IsTaskReachable(task, node);
        }
        else if (currentNode.Value.ChildMode == TaskChildMode.SideBySide)
        {
          return this.IsTaskReachable(task, node);
        }
      }

      return false;
    }
  }
}
