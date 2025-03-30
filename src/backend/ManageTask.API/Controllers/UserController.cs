using ManageTask.API.Constants;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Models.Pagination;
using ManageTask.Application.Services;
using ManageTask.Contracts.ApiContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.HttpResult;
using ResultSharp.Logging;

using LogLevel = ResultSharp.Logging.LogLevel;

namespace ManageTask.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(IAccountService accountService) : ControllerBase
    {
        readonly private IAccountService accountService = accountService;

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize, string? name)
        {
            var tokenSource = new CancellationTokenSource();
            var paginationParams = new PaginationParams()
            {
                PageNumber = page,
                PageSize = pageSize
            };
            var sortParams = new SortParams()
            {
                SortKey = "Name",
                IsAscending = true
            };
            var result = await accountService.GetAll(paginationParams, sortParams, name, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        public async Task<IActionResult> Update(RegisterRequest user, Guid id)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await accountService.UpdateAsync(id, user, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [HttpGet("for-test")]
        public async Task<IActionResult> GetAdmin(Guid id)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await accountService.GetAdminAsync(id, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
    }
}
