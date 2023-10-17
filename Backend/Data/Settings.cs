namespace Backend.Data;
public struct Settings
{
    public bool IsSoundOn { get; }
    public Timing Timing { get; }

    public Settings(bool isSoundOn, Timing timing)
    {
        IsSoundOn = isSoundOn;
        Timing = timing;
    }
}