﻿using System;

namespace ChangeTracker.Application.UseCases.Labels.AddChangeLogLineLabel
{
    public interface IAddChangeLogLineLabelOutputPort
    {
        void Added(Guid lineId);
        void Conflict(string reason);
        void ChangeLogLineDoesNotExist();
        void RelatedVersionAlreadyReleased();
        void RelatedVersionDeleted();
        void InvalidLabel(string label);
        void MaxLabelsReached(int maxLabels);
    }
}
