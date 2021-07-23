using System;
using System.Collections.Generic;
using System.Text;

namespace SimTask
{
  public class TaskQueueTreeNode
  {
    public ITask Value { get; set; }

    public TaskQueueTreeNode PreviousNode { get; set; }
    
    public void AddNode(TaskQueueTreeNode node)
    {
      this.Nodes.Add(node);
      node.PreviousNode = this;
    }

    public void RemoveNode(TaskQueueTreeNode node)
    {
      node.PreviousNode = null;
      this.Nodes.Remove(node);
    }
    
    public List<TaskQueueTreeNode> GetNodes()
    {
      return this.Nodes;
    }

    public List<TaskQueueTreeNode> Nodes { get; set; } = new List<TaskQueueTreeNode>();
  }
}
