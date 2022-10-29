using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Accounts.CreateAccount;

public interface ICreateAccount
{
    Task ExecuteAsync(ICreateAccountOutputPort output, CreateAccountRequestModel requestModel);
}