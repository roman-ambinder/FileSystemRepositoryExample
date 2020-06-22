using Roman.Ambinder.Infra.Common.DataTypes;
using System;

namespace FileSystemRepositoryExample.Common.Interfaces
{
    public interface IRepositoyOf<TValue, TKey>
    {
        OperationResultOf<TValue> TryCreate(TKey key, Action<TValue> init = null);

        OperationResultOf<TValue> TryGet(TKey key);

        OperationResultOf<TValue> TryUpdate(TKey key, Action<TValue> updateAction);

        OperationResultOf<TValue> TryDelete(TKey key);
    }
}
