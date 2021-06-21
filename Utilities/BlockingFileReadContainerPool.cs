using Microsoft.Extensions.Logging;
namespace ReactRedux.Utilities {
    internal sealed class BlockingFileReadContainerPool : BlockingContainerPool<FileReadContainer>, IFileReadContainerPool {
        public BlockingFileReadContainerPool(AppConfiguration appConfiguration, ILogger<BlockingFileReadContainerPool> logger) : base(logger) {
            for (int i = 0; i < appConfiguration.GraphConcurrencyCount; i++) {
                Return(new FileReadContainer(appConfiguration.GraphLineCount, appConfiguration.GraphLineLength));
            }
        }

        public override void Return(FileReadContainer item) {
            item.CurrentLineCount = 0;
            base.Return(item);
        }
    }
}