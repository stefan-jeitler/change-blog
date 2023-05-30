namespace ChangeBlog.Application.Boundaries.DataAccess;

public interface IBusinessTransaction
{
    public void Start();
    public void Commit();
}