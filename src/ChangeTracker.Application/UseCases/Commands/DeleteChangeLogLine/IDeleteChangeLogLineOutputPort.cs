using System;

namespace ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine
{
    public interface IDeleteChangeLogLineOutputPort
    {
        public void LineDoesNotExist(Guid changeLogLineId);
        void LineDeleted(Guid lineId);
        void Conflict(string reason);
    }
}