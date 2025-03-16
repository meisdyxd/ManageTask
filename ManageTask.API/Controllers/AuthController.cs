using ManageTask.API.Constants;
using ManageTask.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var tokenSource = new CancellationTokenSource();
            var result = _authService.RegisterAsync(request, HttpContent.Response, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return await result;
        }
    }
}
