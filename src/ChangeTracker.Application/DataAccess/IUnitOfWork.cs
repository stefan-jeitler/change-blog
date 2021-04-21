namespace ChangeTracker.Application.DataAccess
{
    public interface IUnitOfWork
    {
        public void Start();
        public void Commit();
    }
}