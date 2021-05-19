﻿using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models
{
    public class VersionChangeLogLineRequestModelRequestModel : IChangeLogLineRequestModel
    {
        public VersionChangeLogLineRequestModelRequestModel(Guid productId,
            string version,
            string text,
            List<string> labels,
            List<string> issues)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public Guid ProductId { get; }
        public string Version { get; }
        public string Text { get; }
        public List<string> Labels { get; }
        public List<string> Issues { get; }
    }
}