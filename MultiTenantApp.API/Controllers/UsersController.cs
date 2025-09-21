using Microsoft.AspNetCore.Mvc;
using MultiTenantApp.API.Extensions;
using SharedKernel;
using Application.Features.User.Command;
using Application.Features.User.Query;
using MultiTenantApp.API.Infrastructure;
using Application.Features.Login;

namespace MultiTenantApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        UserCommandHandler userCommand,
        UserQueryHandler userQuery,
        LoginCommandHandler userManagementCommand,
        LoginQueryHandler userManagementQuery
        ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var result = await userQuery.GetAllUsersAsync(new CancellationToken());
            return result.Match(
                value => CustomResults.Success<object>(value),
                CustomResults.Problem
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var result = await userQuery.GetUserByIdAsync(id, new CancellationToken());
            return result.Match(
                value => CustomResults.Success<object>(value),
                CustomResults.Problem
            );
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync([FromBody] UserCreateCommand user)
        {
            if (!ModelState.IsValid)
                return CustomResults.Problem(Result.Failure(Error.Problem("General.ModelInvalid", ModelState.SerializeModelStateErrors())));

            var result = await userCommand.CreateUserAsync(user, new CancellationToken());
            return result.Match(
                value => CustomResults.Success(title: "User.Created",
                result: value, status: StatusCodes.Status201Created),
                CustomResults.Problem
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAsync(int id, [FromBody] UserUpdateCommand user)
        {
            if (!ModelState.IsValid)
                return CustomResults.Problem(Result.Failure(Error.Problem("General.ModelInvalid", ModelState.SerializeModelStateErrors())));

            var result = await userCommand.UpdateUserAsync(id, user, new CancellationToken());
            return result.Match(
                value => CustomResults.Success(value, title: "User.Updated"),
                CustomResults.Problem
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var result = await userCommand.DeleteUserAsync(id, new CancellationToken());
            return result.Match(
                () => CustomResults.Success<object>(result: null, title: "User.Deleted", status: StatusCodes.Status202Accepted),
                CustomResults.Problem
            );
        }

        [HttpPost("{id}/ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody] ChangePasswordCommand passwordCommand)
        {
            if (!ModelState.IsValid)
                return CustomResults.Problem(Result.Failure(Error.Problem("General.ModelInvalid", ModelState.SerializeModelStateErrors())));

            var result = await userManagementCommand.ChangePassword(id, passwordCommand, new CancellationToken());
            return result.Match(
                value => CustomResults.Success<object>(value),
                CustomResults.Problem
            );
        }

        [HttpGet("{id}/Permissions")]
        public async Task<IActionResult> GetUserScreenPermissionsAsync(int id)
        {
            var result = await userManagementQuery.GetUserScreenPermissionsAsync(id, new CancellationToken());
            return result.Match(
                value => CustomResults.Success<object>(value),
                CustomResults.Problem
            );
        }
    }
}
