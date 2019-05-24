using ReactRedux.Dtos;

namespace ReactRedux.Utilities {
    public sealed class FileReadContainer {
        public int ConstantLineLength { get; }
        public int CurrentLineCount { get; set; }
        public TextLine[] Lines { get; }
        public ValueReadingDto[] Values { get; }
        public FileReadContainer(int lineCount, int lineLength) {
            ConstantLineLength = lineLength;
            Lines = new TextLine[lineCount];
            Values = new ValueReadingDto[lineCount];
        }
    }
}