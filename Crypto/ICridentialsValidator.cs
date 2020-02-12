namespace ReactRedux.Crypto {
    public interface ICridentialsValidator {
         bool Verify(string username, string password);
    }
}