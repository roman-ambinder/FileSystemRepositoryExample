using Roman.Ambinder.Infra.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace FileSystemRepositoryExample.Common.Interfaces
{
    public interface IAsyncRepositoyOf<TValue, TKey>
    {
        Task<OperationResultOf<TValue>> TryCreateAsync(TKey key, Action<TValue> init = null);

        Task<OperationResultOf<TValue>> TryGetAsync(TKey key);

        Task<OperationResultOf<TValue>> TryUpdateAsync(TKey key, Action<TValue> updateAction);

        Task<OperationResultOf<TValue>> TryDeleteAsync(TKey key);
    }
}