﻿using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public interface IAddPendingLineOutputPort : ILineParserOutput
    {
        void ProductDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void TooManyLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(string text);
    }
}