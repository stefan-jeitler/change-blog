﻿using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Conflicts;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters
{
    public static class ConflictExtensions
    {
        public static ActionResult ToResponse(this Conflict conflict)
        {
            return conflict switch
            {
                AddOrUpdateChangeLogLineConcurrencyConflict concurrencyConflict => CreateConcurrencyIssueResponse(concurrencyConflict),
                ChangeLogLineDeletedConflict lineDeleteConflict => CreateLineDeletedResponse(lineDeleteConflict),
                ProductClosedConflict closedConflict => CreateProductClosedResponse(closedConflict),
                VersionDeletedConflict versionDeletedConflict => CreateVersionDeletedResponse(versionDeletedConflict),
                VersionReleasedConflict versionReleasedConflict => CreateVersionReleasedResponse(versionReleasedConflict),
                _ => throw new ArgumentOutOfRangeException(nameof(conflict))
            };
        }

        private static ConflictObjectResult CreateVersionReleasedResponse(
            VersionReleasedConflict versionReleasedConflict)
        {
            var versionId = versionReleasedConflict.VersionId;
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            var responseMessage = DefaultResponse.Create("The requested version has already been released.", resourceIds);

            return new ConflictObjectResult(responseMessage);
        }

        private static ConflictObjectResult CreateVersionDeletedResponse(VersionDeletedConflict versionDeletedConflict)
        {
            var versionId = versionDeletedConflict.VersionId;
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            var responseMessage = DefaultResponse.Create("The requested version has already been deleted.",
                resourceIds);

            return new ConflictObjectResult(responseMessage);
        }

        private static ConflictObjectResult CreateProductClosedResponse(ProductClosedConflict closedConflict)
        {
            var productId = closedConflict.ProductId;
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            var responseMessage =
                DefaultResponse.Create("The related product has already been closed.", resourceIds);

            return new ConflictObjectResult(responseMessage);
        }

        private static ConflictObjectResult CreateLineDeletedResponse(ChangeLogLineDeletedConflict lineDeleteConflict)
        {
            var changeLogLineId = lineDeleteConflict.ChangeLogLineId;
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            var responseMessage = DefaultResponse.Create(
                "The requested ChangeLogLine has already been deleted.", resourceIds);

            return new ConflictObjectResult(responseMessage);
        }

        private static ActionResult CreateConcurrencyIssueResponse(
            AddOrUpdateChangeLogLineConcurrencyConflict concurrencyConflict)
        {
            var productId = concurrencyConflict.ProductId;
            var versionId = concurrencyConflict.VersionId;
            var changeLogLineId = concurrencyConflict.ChangeLogLineId;

            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString(),
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            if (versionId.HasValue)
            {
                resourceIds.Add(nameof(versionId), versionId.Value.ToString());
            }

            var responseMessage =
                DefaultResponse.Create("Error while inserting or updating ChangeLogLines. Please try again later", resourceIds);

            return new ConflictObjectResult(responseMessage);
        }
    }
}