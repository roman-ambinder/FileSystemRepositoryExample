using Roman.Ambinder.Infra.Common.DataTypes;
using System.IO;
using System.Threading.Tasks;

namespace FileSystemRepositoryExample.Common.Interfaces
{
    public interface ISerializerOf<TValue>
        where TValue : new()
    {
        OperationResult TrySerialize(TValue value, Stream destinationStream);

        Task<OperationResult> TrySerializeAsync(TValue value, Stream destinationStream);

        OperationResultOf<TValue> TryDeserialize(Stream destinationStream);

        Task<OperationResultOf<TValue>> TryDeserializeAsync(Stream destinationStream);
    }
}