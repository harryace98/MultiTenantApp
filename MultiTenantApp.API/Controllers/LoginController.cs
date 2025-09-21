using Application.Features.Login;
using Application.Features.User.Command;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using MultiTenantApp.API.Extensions;
using MultiTenantApp.API.Infrastructure;
using SharedKernel;

namespace MultiTenantApp.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController(LoginQueryHandler userManagementQuery) : ControllerBase {
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO loginRequest)
            {
            if (!ModelState.IsValid)
                return CustomResults.Problem(Result.Failure(Error.Problem("General.ModelInvalid", ModelState.SerializeModelStateErrors())));

            var result = await userManagementQuery.LoginAsync(loginRequest, new CancellationToken());
            return result.Match(
                value => CustomResults.Success<object>(value),
                CustomResults.Problem
            );
            }
        }
    }