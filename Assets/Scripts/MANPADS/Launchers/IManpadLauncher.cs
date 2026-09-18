using UnityEngine;

public interface IManpadLauncher
{
    public ISeeker Seeker { get; }
    public void Launch(bool isCriticalHit);
}
