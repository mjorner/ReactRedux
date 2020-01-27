using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
        private readonly ICridentialsValidator CridentialsValidator;
        private readonly INowTokenManager NowTokenManager;

        public DataController(AppConfiguration configuration, IFileReader fileReader,
            IStringCompressor stringCompressor, IFileReadContainerPool fileReadContainerPool,
            ILogger<DataController> logger, ICridentialsValidator cridentialsValidator,
            INowTokenManager nowTokenManager) {
            Configuration = configuration;
            FileReader = fileReader;
            StringCompressor = stringCompressor;
            FileReadContainerPool = fileReadContainerPool;
            Logger = logger;
            CridentialsValidator = cridentialsValidator;
            NowTokenManager = nowTokenManager;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public ConfigurationDto GetAppConfiguration() {
            return new ConfigurationDto() { AppTitle = Configuration.AppTitle, SnapShotFile = Configuration.SnapShotFile, LogFiles = Configuration.LogFiles };
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserDto userDto) {
            bool valid = CridentialsValidator.Verify($"{userDto.Username}:{userDto.Password}");

            if (!valid) {
                return BadRequest("Username or password is incorrect");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, userDto.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new AuthResponse { Token = tokenString, Username = userDto.Username });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetFilenames() {
            List<string> lines = await FileReader.ReadAllLinesAsync(Configuration.WebConfigPath);
            string content = string.Join("", lines);
            List<ReadingFilenamesDto> list = JsonConvert.DeserializeObject<List<ReadingFilenamesDto>>(content);
            return CreatedAtAction(nameof(GetFilenames), new InitInfoDto() { FileNames = list, TimePeriods = TimePeriods.AllTimePeriods });
        }

        [HttpGet("[action]")]
        public async Task<CopmpressedDataDto> ReadGraphData(string fileName, int columnIndex, string timeSpan) {
            if (fileName == null) {
                return new CopmpressedDataDto() { Base64Bytes = StringCompressor.Compress(new ValueReadingDto[0]) };
            }
            FileReadContainer fileReadContainer = FileReadContainerPool.Rent();
            await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{fileName}", fileReadContainer);
            Logger.LogTrace($"Read {fileReadContainer.CurrentLineCount} for {fileName}.");
            int count = StringParser.ParseValueReadings(fileReadContainer, columnIndex, timeSpan);
            Logger.LogTrace($"Parsed {count-1} lines for {fileName} with {timeSpan}.");
            string str = StringCompressor.Compress(fileReadContainer.Values.Take(count - 1));
            FileReadContainerPool.Return(fileReadContainer);
            return new CopmpressedDataDto() { Base64Bytes = str };
        }

        [HttpGet("[action]")]
        public async Task<OutResultDto> ReadOutFile(string filename, string title) {
            List<string> lines = await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{filename}");
            string line = string.Join("", lines);
            return new OutResultDto() { Str = line, Title = title };
        }

        [HttpGet("[action]")]
        public async Task<TxtDto> ReadTextFile(string filename) {
            List<string> lines = await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{filename}");
            string line = string.Join("\n", lines);
            return new TxtDto() { Text = line };
        }

        [HttpGet("[action]")]
        public async Task<TxtDto> ReadSysLog(string filename) {
            List<string> lines = await FileReader.ReadAllLinesAsync($"{Configuration.LogPath}{filename}");
            string line = string.Join("\n", lines);
            return new TxtDto() { Text = line };
        }

        [HttpGet("[action]")]
        public TxtDto GetSnapshotToken() {
            return new TxtDto() { Text = NowTokenManager.GenerateToken() };
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult Image(string token) {
            if (!NowTokenManager.ValidateToken(token)) {
                return BadRequest("Incorrect token");
            }

            var file = $"{Configuration.SnapShotPath}{Configuration.SnapShotFile}";
            return PhysicalFile(file, "image/jpg");
        }
    }
}