using Backend.Data;
using System.ComponentModel;

namespace Backend;
public class Exercise
{
    public string Name { get; }
    public ExerciseNode? CurrentNode { get; private set; }
    public int NodePointer {  get; private set; }
    public List<ExerciseNode> ExerciseNodes { get; private set; }

    public Exercise(string name) 
    {
        Name = name;
        NodePointer = -1;
        CurrentNode = null;
        ExerciseNodes = new List<ExerciseNode>();
    }
    public Exercise(string name, List<ExerciseNode> exerciseNodes)
    {
        Name = name;
        NodePointer = (exerciseNodes.Count == 0) ? -1 : 0;
        CurrentNode = (NodePointer >= 0) ? exerciseNodes[0] : null;
        ExerciseNodes = exerciseNodes;
    }
    public void Add(ExerciseNode node)
    {
        ExerciseNodes.Add(node);
        UpdateCurrentNode();
    }

    public void RemoveById(int id)
    {
        ExerciseNode? exerciseNode = ExerciseNodes.Find(node  => node.Id == id);
        if(exerciseNode is not null) ExerciseNodes.Remove(exerciseNode);
        UpdateCurrentNode();
    }

    public void Next() 
    {
        if (ExerciseNodes.Count == 0 || NodePointer >= ExerciseNodes.Count) return; 
        NodePointer++;
        UpdateCurrentNode();
    }

    public void Previous() 
    {
        if (NodePointer <= 0) return;
        NodePointer--;
        UpdateCurrentNode();
    }

    public void Reset() 
    {
        foreach(ExerciseNode node in ExerciseNodes)
        {
            node.Reps = 0;
            node.Elapsed = 0;
        }
        NodePointer = 0;
        UpdateCurrentNode();
    }

    private void UpdateCurrentNode() 
    {
        if(ExerciseNodes.Count == 0)
        {
            NodePointer = -1;
            CurrentNode = null;
        }
        else if (NodePointer < 0)
        {
            NodePointer = 0;
            CurrentNode = ExerciseNodes[NodePointer];
        }
        else if (NodePointer >= ExerciseNodes.Count)
        {
            NodePointer = ExerciseNodes.Count - 1;
            CurrentNode = ExerciseNodes[NodePointer];
        }
        else
        {
            CurrentNode = ExerciseNodes[NodePointer];
        }
    }
}
