public interface IMissile
{
    public ISeeker Seeker { get; }
    public void Launch(bool isCriticalHit);
}
