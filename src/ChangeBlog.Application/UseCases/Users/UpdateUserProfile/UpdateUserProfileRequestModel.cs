using System;

namespace ChangeBlog.Application.UseCases.Users.UpdateUserProfile;

public record UpdateUserProfileRequestModel(Guid UserId, string OlsonOrWindowsId, string Culture);