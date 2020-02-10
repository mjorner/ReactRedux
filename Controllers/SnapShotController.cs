using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactRedux.Crypto;
using ReactRedux.Dtos;
using Microsoft.Extensions.Logging;

namespace ReactRedux.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class SnapShotController : Controller {
        private readonly AppConfiguration Configuration;
        private readonly INowTokenManager NowTokenManager;
        private readonly ILogger Logger;

        public SnapShotController(AppConfiguration configuration, INowTokenManager nowTokenManager,
                                  ILogger<SnapShotController> logger) {
            NowTokenManager = nowTokenManager;
            Configuration = configuration;
            Logger = logger;
        }

        [HttpGet("[action]")]
        public TxtDto GetToken() {
            return new TxtDto() { Text = NowTokenManager.GenerateToken() };
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult GetSnapShot(string token) {
            if (!NowTokenManager.ValidateToken(token)) {
                Logger.LogWarning( $"{token} was not accepted.");
                return BadRequest("Incorrect token");
            }

            string filePath = $"{Configuration.SnapShotPath}{Configuration.SnapShotFile}";
            return PhysicalFile(filePath, "image/jpg");
        }
    }
}