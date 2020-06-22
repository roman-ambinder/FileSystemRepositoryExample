using Roman.Ambinder.Infra.Common.DataTypes;

namespace FileSystemRepositoryExample.Common.Interfaces
{
    public interface IKeyValueValidatorOf<TKey,TValue>
    {
        OperationResult Validate(TKey key);

        OperationResult Validate(TValue key);
    }
}
