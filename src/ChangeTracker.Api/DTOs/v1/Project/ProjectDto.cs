﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.GetProjects;

namespace ChangeTracker.Api.DTOs.v1.Project
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public Guid VersioningSchemeId { get; set; }
        public string VersioningScheme { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; }

        public static ProjectDto FromResponseModel(ProjectsQueryResponseModel m) =>
            new()
            {
                Id = m.Id,
                AccountId = m.AccountId,
                Name = m.Name,
                VersioningSchemeId = m.VersioningSchemeId,
                VersioningScheme = m.VersioningScheme,
                CreatedByUser = m.CreatedByUser,
                CreatedAt = m.CreatedAt
            };
    }
}
