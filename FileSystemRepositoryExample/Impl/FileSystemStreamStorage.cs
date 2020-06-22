using FileSystemRepositoryExample.Common.Interfaces;
using Roman.Ambinder.Infra.Common.DataTypes;
using System.IO;

namespace FileSystemRepositoryExample.Impl
{
    public class FileSystemStreamStorage<Tkey> :
        IStreamStorageBy<Tkey>
    {
        private readonly IFilePathProviderFor<Tkey> _pathProvider;

        public FileSystemStreamStorage(
           IFilePathProviderFor<Tkey> pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public bool CheckExists(Tkey key)
        {
            var filePath = _pathProvider.GetFilePath(key);
            return File.Exists(filePath);
        }

        public OperationResultOf<Stream> TryCreate(Tkey key)
        {
            if (CheckExists(key))
                return $"{key} already exists"
                     .AsFailedOpResOf<Stream>();

            var filePath = _pathProvider.GetFilePath(key);
            return File.Create(filePath)
                .AsSuccessfullOpRes<Stream>();
        }

        public OperationResult TryDelete(Tkey key)
        {
            var filePath = _pathProvider.GetFilePath(key);
            if (!CheckExists(key))
            {
                return new OperationResult(
                    false,
                    $"{filePath} does not exist");
            }

            File.Delete(filePath);

            return OperationResult.Successful;
        }

        public OperationResultOf<Stream> TryOpenForRead(Tkey key)
        {
            var filePath = _pathProvider.GetFilePath(key);
            if (!CheckExists(key))
            {
                return $"{filePath} does not exist"
                        .AsFailedOpResOf<Stream>(); ;
            }

            return File.Create(filePath)
                .AsSuccessfullOpRes<Stream>();
        }

        public OperationResultOf<Stream> TryUpdate(Tkey key)
        {
            var filePath = _pathProvider.GetFilePath(key);
            return File.OpenWrite(filePath)
                .AsSuccessfullOpRes<Stream>();
        }
    }
}