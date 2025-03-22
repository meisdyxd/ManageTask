using ManageTask.API.Constants;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Contracts.ApiContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.HttpResult;
using ResultSharp.Logging;
using LogLevel = ResultSharp.Logging.LogLevel;

namespace ManageTask.API.Controllers
{
    [Route("api/task")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        readonly ITaskService taskService = taskService;
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.GetAll(new(){ PageNumber = page, PageSize = pageSize }, null, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToPool(RequestTask task)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.AddToPoolAsync(task, HttpContext.Request, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpPost("assignedid")]
        public async Task<IActionResult> AddToUser(RequestTask task, Guid assignedid)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.AddToUserAsync(task, assignedid, HttpContext.Request, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
    }
}
