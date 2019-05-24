namespace ReactRedux.Utilities {
    internal sealed class BlockingFileReadContainerPool : BlockingContainerPool<FileReadContainer>, IFileReadContainerPool {
        public BlockingFileReadContainerPool(int concurrencyCount, int lineCount, int lineLength) : base() {
            for (int i = 0; i < concurrencyCount; i++) {
                Return(new FileReadContainer(lineCount, lineLength));
            }
        }

        public override void Return(FileReadContainer item) {
            item.CurrentLineCount = 0;
            base.Return(item);
        }
    }
}