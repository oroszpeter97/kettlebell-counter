namespace Backend;
public class ExerciseNode : IExerciseNode
{
    public int Id { get; }
    public string Name { get; }
    public ulong Length { get; }
    public ulong Elapsed { get; set; } = 0;
    public int MaxReps { get; }
    public int Reps { get; set; } = 0;

    public ExerciseNode(int id, string name, ulong length, int maxReps)
    {
        Id = id;
        Name = name;
        Length = length;
        MaxReps = maxReps;
    }
}
