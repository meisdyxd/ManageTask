using ManageTask.API.Constants;
using ManageTask.API.Mappers;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Services;
using ManageTask.Contracts.ApiContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.Extensions.FunctionalExtensions.Sync;
using ResultSharp.HttpResult;
using ResultSharp.Logging;
using LogLevel = ResultSharp.Logging.LogLevel;

namespace ManageTask.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var tokenSource = new CancellationTokenSource();
            var result = _accountService.RegisterAsync(request, HttpContext.Response, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return (await result)
                .Map(u => u.Map())
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [HttpGet("profile")]
        public async Task<IActionResult> Get()
        {
            var tokenSource = new CancellationTokenSource();
            var result = _accountService.GetCurrentUserAsync(HttpContext.Request, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return (await result)
                .Map(u => u.Map())
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var tokenSource = new CancellationTokenSource();
            var result = _accountService.LogoutAsync(HttpContext.Request, HttpContext.Response, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return (await result).LogErrorMessages(logLevel: LogLevel.Warning).ToResponse();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var tokenSource = new CancellationTokenSource();
            var result = _accountService.LoginAsync(request, HttpContext.Response, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            Response.Headers.Append("Access-Control-Expose-Headers", "Authorization, Refresh-Token");
            return (await result).LogErrorMessages(logLevel: LogLevel.Warning).ToResponse();
        }
        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var tokenSource = new CancellationTokenSource();
            var result = _accountService.RefreshToken(HttpContext.Request, HttpContext.Response, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return (await result).LogErrorMessages(logLevel: LogLevel.Warning).ToResponse();
        }
    }
}
