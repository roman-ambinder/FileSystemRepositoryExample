using Roman.Ambinder.Infra.Common.DataTypes;
using System.IO;

namespace FileSystemRepositoryExample.Common.Interfaces
{
    public interface IStreamStorageBy<TKey>
    {
        bool CheckExists(TKey key);

        OperationResultOf<Stream> TryOpenForRead(TKey key);

        OperationResultOf<Stream> TryCreate(TKey key);

        OperationResultOf<Stream> TryUpdate(TKey key);

        OperationResult TryDelete(TKey key);
    }
}