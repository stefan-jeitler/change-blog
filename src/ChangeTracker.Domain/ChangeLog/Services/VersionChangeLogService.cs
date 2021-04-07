namespace ChangeTracker.Domain.ChangeLog.Services
{
    public class VersionChangeLogService
    {
        public const int MaxChangeLog = 100;
        private readonly ChangeLogInfo _changeLogInfo;

        public VersionChangeLogService(ChangeLogInfo changeLogInfo)
        {
            _changeLogInfo = changeLogInfo;
        }

        public uint RemainingPositionsToAdd => MaxChangeLog - _changeLogInfo.Count;

        public uint NextFreePosition => _changeLogInfo.LastPosition + 1;

        public bool IsPositionAvailable => RemainingPositionsToAdd > 0;
    }
}