using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models
{
    public class CompleteVersionRequestModel
    {
        public CompleteVersionRequestModel(Guid productId, string version, List<ChangeLogLineRequestModel> lines,
            bool releaseImmediately = false)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            ReleaseImmediately = releaseImmediately;
        }

        public Guid ProductId { get; }
        public string Version { get; }
        public List<ChangeLogLineRequestModel> Lines { get; }
        public bool ReleaseImmediately { get; }
    }
}