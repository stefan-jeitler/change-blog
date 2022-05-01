using System;

namespace ChangeBlog.Application.UseCases.Commands.UpdateUserProfile;

public record UpdateUserProfileRequestModel(Guid UserId, string OlsonOrWindowsId, string Culture);