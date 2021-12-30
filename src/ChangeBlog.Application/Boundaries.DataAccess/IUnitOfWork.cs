namespace ChangeBlog.Application.Boundaries.DataAccess;

public interface IUnitOfWork
{
    public void Start();
    public void Commit();
}