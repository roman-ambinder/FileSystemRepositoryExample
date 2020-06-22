using FileSystemRepositoryExample.Common.Interfaces;
using Roman.Ambinder.Infra.Common.DataTypes;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FileSystemRepositoryExample.Impl
{
    public class AsyncFileSystemRepositoryOf<TValue, TKey> :
        IAsyncRepositoyOf<TValue, TKey>
         where TValue : new()
    {
        private readonly IStreamStorageBy<TKey> _streamStore;
        private readonly ISerializerOf<TValue> _serializer;
        private readonly IKeyValueValidatorOf<TKey, TValue> _keyValueValidator;

        public AsyncFileSystemRepositoryOf(IStreamStorageBy<TKey> streamStore,
            ISerializerOf<TValue> serializer,
            IKeyValueValidatorOf<TKey, TValue> keyValueValidator)
        {
            _streamStore = streamStore;
            _serializer = serializer;
            _keyValueValidator = keyValueValidator;
        }

        public Task<OperationResultOf<TValue>> TryCreateAsync(TKey key,
            Action<TValue> init = null)
        {
            //Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return Task.FromResult(validateOpRes.AsFailedOpResOf<TValue>());

            // Ensure key is unique
            if (_streamStore.CheckExists(key))
                return Task.FromResult(
                    $"{key} already exists in store".AsFailedOpResOf<TValue>());

            // Create new value and validate
            var newValue = new TValue();
            init?.Invoke(newValue);
            validateOpRes = _keyValueValidator.Validate(newValue);
            if (!validateOpRes)
                return Task.FromResult(validateOpRes.AsFailedOpResOf<TValue>());

            // Create stream for new value
            var createStreamOpRes = _streamStore.TryCreate(key);
            if (!createStreamOpRes)
                return Task.FromResult(new OperationResultOf<TValue>());

            return TrySerializeAndReturnValueAsync(newValue,
                createStreamOpRes);
        }

        public async Task<OperationResultOf<TValue>> TryDeleteAsync(TKey key)
        {
            //Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Try get existing value
            var getOpRes = await TryGetAsync(key).ConfigureAwait(false);
            if (!getOpRes)
                return getOpRes;

            // Try delete existing value
            var deleteOpRes = _streamStore.TryDelete(key);
            if (!deleteOpRes)
                return deleteOpRes.AsFailedOpResOf<TValue>();

            return getOpRes;
        }

        public Task<OperationResultOf<TValue>> TryGetAsync(TKey key)
        {
            //Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return Task.FromResult(validateOpRes.AsFailedOpResOf<TValue>());

            // Get existing value stream
            var getStreamFromReadOpRes = _streamStore.TryOpenForRead(key);
            if (!getStreamFromReadOpRes)
                return Task.FromResult(
                    getStreamFromReadOpRes.ErrorMessage
                    .AsFailedOpResOf<TValue>());

            return TryDeserializeValueAsync(getStreamFromReadOpRes);
        }

        public async Task<OperationResultOf<TValue>> TryUpdateAsync(TKey key,
            Action<TValue> updateAction)
        {
            //Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Try Get existing value by key
            var getExistingValueOpRes = await TryGetAsync(key).ConfigureAwait(false);
            if (!getExistingValueOpRes)
                return getExistingValueOpRes;

            // Update value and validate update value
            updateAction(getExistingValueOpRes.Value);
            validateOpRes = _keyValueValidator.Validate(getExistingValueOpRes.Value);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Get existing values stream for update
            var getUpdateStreamOpRes = _streamStore.TryUpdate(key);
            if (!getUpdateStreamOpRes)
                return getUpdateStreamOpRes.ErrorMessage
                    .AsFailedOpResOf<TValue>();

            // Serialize value and write to stream store
            return await TrySerializeAndReturnValueAsync(
                value: getExistingValueOpRes,
                destinationStream: getUpdateStreamOpRes)
                .ConfigureAwait(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<OperationResultOf<TValue>> TrySerializeAndReturnValueAsync(
        TValue value,
        Stream destinationStream)
        {
            using (destinationStream)
            {
                var serializeOpRes = await _serializer.TrySerializeAsync(
                    value,
                    destinationStream)
                    .ConfigureAwait(false);

                if (!serializeOpRes)
                    return serializeOpRes.AsFailedOpResOf<TValue>(); ;
            }

            return value.AsSuccessfullOpRes();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<OperationResultOf<TValue>> TryDeserializeValueAsync(
         Stream stream)
        {
            TValue value;
            using (stream)
            {
                var desrializeOpRes = await _serializer
                    .TryDeserializeAsync(stream)
                    .ConfigureAwait(false);

                value = desrializeOpRes.Value;
                if (!desrializeOpRes)
                    return desrializeOpRes.ErrorMessage.AsFailedOpResOf<TValue>();
            }

            return value.AsSuccessfullOpRes();
        }
    }
}