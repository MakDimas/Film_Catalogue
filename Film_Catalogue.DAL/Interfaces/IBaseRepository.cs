namespace Film_Catalogue.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetAll();

        Task Create(T entity);

        Task Update(T entity);

        Task Delete(T entity);
    }
}