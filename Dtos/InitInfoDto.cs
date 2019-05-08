using System.Collections.Generic;

namespace ReactRedux.Dtos {
    public class InitInfoDto {
        public IList<ReadingFilenamesDto> FileNames { get; set; }
        public IList<string> TimePeriods { get; set; }
    }
}