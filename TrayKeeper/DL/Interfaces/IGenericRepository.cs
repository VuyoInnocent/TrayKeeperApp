namespace TrayKeeper.DL.Interfaces
{
    public interface IGenericRepository <T> where T : class
    {
        Task Init();
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> UpdateAsync(T item);
        Task<int> InsertAsync(T item);
        Task<int> DeleteAsync(T item);
    }
}
