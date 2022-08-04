using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Users.UpdateUserProfile;

public interface IUpdateUserProfile
{
    Task ExecuteAsync(IUpdateUserProfileOutputPort output,
        UpdateUserProfileRequestModel requestModel);
}