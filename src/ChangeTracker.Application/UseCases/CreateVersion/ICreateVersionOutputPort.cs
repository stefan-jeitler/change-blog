﻿using System;

namespace ChangeTracker.Application.UseCases.CreateVersion
{
    public interface ICreateVersionOutputPort
    {
        void Created(Guid id);
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void ProjectDoesNotExist();
        void Conflict(string reason);
        void VersionAlreadyExists(string version);
    }
}