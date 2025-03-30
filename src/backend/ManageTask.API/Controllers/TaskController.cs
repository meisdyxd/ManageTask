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
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize, int? status)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var result = await taskService.GetAll(new(){ PageNumber = page, PageSize = pageSize },
                new() { SortKey = "Title", IsAscending = true },
                status,
                tokenSource.Token);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(int page, int pageSize, int? status)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.GetCurrentAsync(HttpContext.Request,
                new() { PageNumber = page, PageSize = pageSize }, 
                new() { SortKey = "Title", IsAscending = true}, 
                status,
                tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpGet("created")]
        public async Task<IActionResult> GetCreated(int page, int pageSize, int? status)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.GetCreatedAsync(HttpContext.Request,
                new() { PageNumber = page, PageSize = pageSize },
                new() { SortKey = "Title", IsAscending = true },
                status,
                tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpGet("pool")]
        public async Task<IActionResult> GetPool(int page, int pageSize, int? status)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var result = await taskService.GetFromPool(
                new() { PageNumber = page, PageSize = pageSize },
                new() { SortKey = "Title", IsAscending = true },
                status,
                tokenSource.Token);

            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("to-pool")]
        public async Task<IActionResult> AddToPool(RequestTask task)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.AddToPoolAsync(task, HttpContext.Request, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("to-user")]
        public async Task<IActionResult> AddToUser(RequestTask task)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.AddToUserAsync(task, HttpContext.Request, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(Guid taskId)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.ChangeStatusAsync(taskId, HttpContext.Request, Domain.StatusTask.Cancelled, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("return")]
        public async Task<IActionResult> Return(Guid taskId)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.ChangeStatusAsync(taskId, HttpContext.Request, Domain.StatusTask.InProcess, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        public async Task<IActionResult> Update(Guid taskId, RequestTask requestTask)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.UpdateAsync(requestTask, taskId, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut(Guid taskId)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.ChangeStatusAsync(taskId, HttpContext.Request, Domain.StatusTask.OnReview, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpPost("complete")]
        public async Task<IActionResult> Complete(Guid taskId)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.ChangeStatusAsync(taskId, HttpContext.Request, Domain.StatusTask.Success, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
        [Authorize]
        [HttpPost("take")]
        public async Task<IActionResult> Take(Guid taskId)
        {
            var tokenSource = new CancellationTokenSource();
            var result = await taskService.TakeAsync(taskId, HttpContext.Request, tokenSource.Token);
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return result
                .LogErrorMessages(logLevel: LogLevel.Warning)
                .ToResponse();
        }
    }
}
