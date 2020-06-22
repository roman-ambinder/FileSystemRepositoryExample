namespace FileSystemRepositoryExample.Common.Interfaces
{
    public interface IFilePathProviderFor<TKey>
    {
        string GetFilePath(TKey key);
    }
}
