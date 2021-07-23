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

    public bool IsTaskReachable(ITask task)
    {
      return this.IsTaskReachable(task, this.rootNode);
    }

    public bool IsTaskReachable(ITask task, TaskQueueTreeNode currentNode)
    {
      if (currentNode.Value == task)
      {
        return true;
      }

      foreach (TaskQueueTreeNode node in currentNode.GetNodes())
      {
        if (currentNode.Value.ChildMode == TaskChildMode.Sequentiell)
        {
          if (node.Value != null && node.Value.GetProgress() < 1.0f)
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

    public bool AddTaskToNode(ITask task, TaskQueueTreeNode currentNode)
    {
      TaskQueueTreeNode treeNode = new TaskQueueTreeNode();
      treeNode.Value = task;
      currentNode.AddNode(treeNode);
      this.Nodes.Add(task, treeNode);

      foreach (ITask child in task.ChildTasks)
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
    /// Wenn ein task einem anderen task untergeordnet wird ändert sich auch die Position
    /// des tasks in der Queue.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
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

    public void OnTaskFinished(object sender, EventArgs eventArgs)
    {
      var task = (ITask)sender;
      this.RemoveTask(task);
    }
    public void RemoveTask(ITask task)
    {
      if (this.Nodes.ContainsKey(task))
      {
        TaskQueueTreeNode treeNode = this.Nodes[task];
        this.RemoveTreeNode(treeNode);
      }
    }

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

    public TaskQueueTreeNode rootNode { get; set; }

    public Dictionary<ITask, TaskQueueTreeNode> Nodes { get; set; } = new Dictionary<ITask, TaskQueueTreeNode>();
  }
}
