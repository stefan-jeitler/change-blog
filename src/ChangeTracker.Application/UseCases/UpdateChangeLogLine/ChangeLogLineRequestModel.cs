﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.UpdateChangeLogLine
{
    public class ChangeLogLineRequestModel
    {
        public ChangeLogLineRequestModel(Guid changeLogLineId, string text, IList<string> labels, IList<string> issues)
        {
            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            ChangeLogLineId = changeLogLineId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public Guid ChangeLogLineId { get; }
        public string Text { get; }
        public IList<string> Labels { get; }
        public IList<string> Issues { get; }
    }
}
