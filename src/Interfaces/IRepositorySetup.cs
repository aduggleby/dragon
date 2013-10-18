namespace Dragon.Interfaces
{
    public interface IRepositorySetup
    {
        void EnsureTableExists<T>() where T : class;
        void DropTableIfExists<T>() where T : class;
        void DropTableIfExists(string name);
    }
}