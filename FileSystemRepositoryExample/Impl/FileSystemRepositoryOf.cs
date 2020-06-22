using FileSystemRepositoryExample.Common.Interfaces;
using Roman.Ambinder.Infra.Common.DataTypes;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace FileSystemRepositoryExample.Impl
{

    public class FileSystemRepositoryOf<TValue, TKey> :
        IRepositoyOf<TValue, TKey>
        where TValue : new()
    {
        private readonly IStreamStorageBy<TKey> _streamStore;
        private readonly ISerializerOf<TValue> _serializer;
        private readonly IKeyValueValidatorOf<TKey, TValue> _keyValueValidator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamStore"></param>
        /// <param name="serializer"></param>
        /// <param name="keyValueValidator"></param>
        public FileSystemRepositoryOf(
            IStreamStorageBy<TKey> streamStore,
            ISerializerOf<TValue> serializer,
            IKeyValueValidatorOf<TKey, TValue> keyValueValidator = null)
        {
            _streamStore = streamStore;
            _serializer = serializer;
            _keyValueValidator = keyValueValidator;
        }

        public OperationResultOf<TValue> TryCreate(TKey key,
            Action<TValue> init = null)
        {
            //Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Ensure key is unique
            if (_streamStore.CheckExists(key))
                return $"{key} already exists in store"
                    .AsFailedOpResOf<TValue>();

            // Create new value and validate
            var newValue = new TValue();
            init?.Invoke(newValue);
            validateOpRes = _keyValueValidator.Validate(newValue);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Create stream for new value
            var createStreamOpRes = _streamStore.TryCreate(key);
            if (!createStreamOpRes)
                return new OperationResultOf<TValue>();

            // Serialize value and write to stream store
            return TrySerializeAndReturnValue(
              value: newValue,
              destinationStream: createStreamOpRes);
        }

        public OperationResultOf<TValue> TryDelete(TKey key)
        {
            // Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Try get existing value
            var getOpRes = TryGet(key);
            if (!getOpRes)
                return getOpRes;

            // Try delete existing value
            var deleteOpRes = _streamStore.TryDelete(key);
            if (!deleteOpRes)
                return deleteOpRes.ErrorMessage.AsFailedOpResOf<TValue>();

            return getOpRes;
        }

        public OperationResultOf<TValue> TryGet(TKey key)
        {
            // Validate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Get existing value stream
            var getStreamFromReadOpRes = _streamStore.TryOpenForRead(key);
            if (!getStreamFromReadOpRes)
                return getStreamFromReadOpRes.ErrorMessage
                    .AsFailedOpResOf<TValue>();

            return TryDeserializeValue(getStreamFromReadOpRes);
        }

        public OperationResultOf<TValue> TryUpdate(TKey key,
            Action<TValue> updateAction)
        {
            // Valdiate key
            var validateOpRes = _keyValueValidator.Validate(key);
            if (!validateOpRes)
                return validateOpRes.AsFailedOpResOf<TValue>();

            // Try Get existing value by key
            var getExistingValueOpRes = TryGet(key);
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
            return TrySerializeAndReturnValue(
                value: getExistingValueOpRes,
                destinationStream: getUpdateStreamOpRes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private OperationResultOf<TValue> TrySerializeAndReturnValue(
           TValue value,
           Stream destinationStream)
        {
            using (destinationStream)
            {
                var serializeOpRes = _serializer.TrySerialize(
                    value,
                    destinationStream);

                if (!serializeOpRes)
                    return serializeOpRes.AsFailedOpResOf<TValue>(); ;
            }

            return value.AsSuccessfullOpRes();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private OperationResultOf<TValue> TryDeserializeValue(
            Stream stream)
        {
            TValue value;
            using (stream)
            {
                var desrializeOpRes = _serializer.TryDeserialize(stream);
                value = desrializeOpRes.Value;
                if (!desrializeOpRes)
                    return desrializeOpRes.ErrorMessage
                        .AsFailedOpResOf<TValue>();
            }

            return value.AsSuccessfullOpRes();
        }
    }
}
