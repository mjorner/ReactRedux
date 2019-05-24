namespace ReactRedux.Utilities {
    public sealed class TextLine {
        public char[] Chars { get; }
        public TextLine(int lineLength) {
            Chars = new char[lineLength];
        }
    }
}