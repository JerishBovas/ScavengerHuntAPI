using Microsoft.AspNetCore.Mvc;

namespace ScavengerHunt.Controllers {
    [Route(".well-known/apple-app-site-association")]
    public class AppleController : Controller {
        private IHostEnvironment _hostingEnvironment;
        private IConfiguration configuration;

        public AppleController(IHostEnvironment environment, IConfiguration configuration) {
            _hostingEnvironment = environment;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            return Content(
                await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.ContentRootPath, configuration["AppleSitePath"])),
                "text/plain"
            );
        }
    }
}