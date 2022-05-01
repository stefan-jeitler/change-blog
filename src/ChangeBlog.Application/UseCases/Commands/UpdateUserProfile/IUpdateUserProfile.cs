using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.UpdateUserProfile;

public interface IUpdateUserProfile
{
    Task ExecuteAsync(IUpdateUserProfileOutputPort output,
        UpdateUserProfileRequestModel requestModel);
}