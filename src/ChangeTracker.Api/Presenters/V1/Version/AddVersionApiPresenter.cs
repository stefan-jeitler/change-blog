using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class AddVersionApiPresenter : BasePresenter, IAddVersionOutputPort
    {
        private readonly HttpContext _httpContext;

        public AddVersionApiPresenter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void Created(Guid id)
        {
            var location = _httpContext.CreateLinkTo($"api/v1/versions/{id}");
            Response = new CreatedResult(location, DefaultResponse.Create("Version added.", id));
        }

        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid format '{version}'."));
        }

        public void VersionDoesNotMatchScheme(string version)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"Version does not match your product's versioning scheme. Version '{version}'"));
        }

        public void ProductDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found."));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void VersionAlreadyExists(string version)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create($"Version '{version}' already exists."));
        }

        public void ProductClosed()
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("The related product has already been closed."));
        }

        public void InvalidVersionName(string versionName)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"The name of the version is invalid. Name {versionName}"));
        }
    }
}