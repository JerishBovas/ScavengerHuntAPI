using Microsoft.AspNetCore.Mvc;

namespace ScavengerHunt.Controllers {
    [Route(".well-known/apple-app-site-association")]
    public class AppleController : Controller {
        private IHostEnvironment _hostingEnvironment;

        public AppleController(IHostEnvironment environment) {
            _hostingEnvironment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            return Content(
                await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.ContentRootPath, ".well-known/apple-app-site-association")),
                "text/plain"
            );
        }
    }
}