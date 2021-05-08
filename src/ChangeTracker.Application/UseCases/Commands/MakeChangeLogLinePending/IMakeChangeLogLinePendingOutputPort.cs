﻿using System;

namespace ChangeTracker.Application.UseCases.Commands.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePendingOutputPort
    {
        void WasMadePending(Guid lineId);
        void ChangeLogLineDoesNotExist();
        void ChangeLogLineIsAlreadyPending();
        void VersionAlreadyReleased();
        void VersionClosed();
        void TooManyPendingLines(int maxChangeLogLines);
        void Conflict(string reason);
        void LineWithSameTextAlreadyExists(string text);
    }
}