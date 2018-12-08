using System.Collections.Generic;

namespace ReactRedux.Dtos {
    public class InitInfoDto {
        public List<ReadingFilenamesDto> FileNames { get; set; }
        public List<string> TimePeriods { get; set; }
    }
}