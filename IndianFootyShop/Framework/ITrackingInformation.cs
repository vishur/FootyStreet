using System;

namespace Framework
{
    /// <summary>
    /// ITrackingInformation helps to determine the current user name
    /// and the current time to be updated whenever a DML action happens
    /// ont he live/main entity.
    /// </summary>
    public interface ITrackingInformation
    {
        string CurrentUserId { get; }

        DateTime GetCurrentTime();
    }
}