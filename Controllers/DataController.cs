using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReactRedux.Crypto;
using ReactRedux.Dtos;
using ReactRedux.Utilities;

namespace ReactRedux.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class DataController : Controller {
        private readonly AppConfiguration Configuration;
        private readonly IFileReader FileReader;
        private readonly IStringCompressor StringCompressor;
        private readonly IFileReadContainerPool FileReadContainerPool;
        private readonly ILogger Logger;

        public DataController(AppConfiguration configuration, IFileReader fileReader,
            IStringCompressor stringCompressor, IFileReadContainerPool fileReadContainerPool,
            ILogger<DataController> logger, ICridentialsValidator cridentialsValidator,
            INowTokenManager nowTokenManager) {
            Configuration = configuration;
            FileReader = fileReader;
            StringCompressor = stringCompressor;
            FileReadContainerPool = fileReadContainerPool;
            Logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public ConfigurationDto GetAppConfiguration() {
            return new ConfigurationDto() { AppTitle = Configuration.AppTitle, SnapShotFile = Configuration.SnapShotFile, LogFiles = Configuration.LogFiles };
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetFilenames() {
            string content = await FileReader.ReadAllTextAsync(Configuration.WebConfigPath);
            List<ReadingFilenamesDto> list = JsonConvert.DeserializeObject<List<ReadingFilenamesDto>>(content);
            return CreatedAtAction(nameof(GetFilenames), new InitInfoDto() { FileNames = list, TimePeriods = TimePeriods.AllTimePeriods });
        }

        [HttpGet("[action]")]
        public async Task<CopmpressedDataDto> ReadGraphData(string fileName, int columnIndex, string timeSpan) {
            if (fileName == null) {
                return new CopmpressedDataDto() { Base64Bytes = StringCompressor.Compress(new ValueReadingDto[0]) };
            }
            FileReadContainer fileReadContainer = FileReadContainerPool.Rent();
            try {
                await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{fileName}", fileReadContainer);
                Logger.LogTrace($"Read {fileReadContainer.CurrentLineCount} for {fileName}.");
                int count = StringParser.ParseValueReadings(fileReadContainer, columnIndex, timeSpan);
                Logger.LogTrace($"Parsed {count-1} lines for {fileName} with {timeSpan}.");
                string str = StringCompressor.Compress(fileReadContainer.Values.Take(count - 1));
                return new CopmpressedDataDto() { Base64Bytes = str };
            } finally {
                FileReadContainerPool.Return(fileReadContainer);
            }
        }

        [HttpGet("[action]")]
        public async Task<OutResultDto> ReadOutFile(string filename, string title) {
            string line = await FileReader.ReadAllTextAsync($"{Configuration.DataPath}{filename}");
            return new OutResultDto() { Str = line, Title = title };
        }

        [HttpGet("[action]")]
        public async Task<TxtDto> ReadTextFile(string filename) {
            string line = await FileReader.ReadAllTextAsync($"{Configuration.DataPath}{filename}");
            return new TxtDto() { Text = line };
        }

        [HttpGet("[action]")]
        public async Task<TxtDto> ReadSysLog(string filename) {
            string line = await FileReader.ReadAllTextAsync($"{Configuration.LogPath}{filename}");
            return new TxtDto() { Text = line };
        }
    }
}