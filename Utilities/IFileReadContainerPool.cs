namespace ReactRedux.Utilities {
    public interface IFileReadContainerPool {
        void Return(FileReadContainer item);
        FileReadContainer Rent();
    }
}