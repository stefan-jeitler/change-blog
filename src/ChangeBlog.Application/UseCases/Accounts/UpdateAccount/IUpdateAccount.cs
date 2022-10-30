using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Accounts.UpdateAccount;

public interface IUpdateAccount
{
    Task ExecuteAsync(IUpdateAccountOutputPort output, UpdateAccountRequestModel requestModel);
}