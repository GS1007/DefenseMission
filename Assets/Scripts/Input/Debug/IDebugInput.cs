using System;

public interface IDebugInput
{
    public event Action AngleSetupStickToggled;
    public event Action FireReportOpened;
}
