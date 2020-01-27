namespace ReactRedux.Crypto {
    public interface INowTokenManager {
        string GenerateToken();
        bool ValidateToken(string token);
    }
}