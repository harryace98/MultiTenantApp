using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTOs;

namespace Application.Features.Login
{
    public class LoginQueryHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IPermissionProvider permissionProvider
        )
    {
        /// <summary>
        /// Authenticates a user with the provided email and password.
        /// </summary>
        /// <param name="loginRequest">The login request containing email and password.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a login response if successful.</returns>
        public async Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.IsActive, cancellationToken);

            if (user == null)
                return Result.Failure<LoginResponseDTO>(Error.NotFound("User.NotFound", "User not found"));

       
            if (!passwordHasher.Verify(loginRequest.Password, user.PasswordHash))
                return Result.Failure<LoginResponseDTO>(Error.Problem("User.InvalidPassword", "Invalid password"));

            //get permission for user and generate token
            var permissions = await permissionProvider.GetUserPermissionsAsync(user.Id, user.TenantId);
            string token = tokenProvider.GenerateJWTToken(user, permissions);

            var loginResponse = new LoginResponseDTO
            {
                Idn = user.Id.ToString(),
                Name = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = token,
            };

            return Result.Success(loginResponse);
        }

        /// <summary>
        /// Retrieves the screen permissions for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a list of user permissions if successful.</returns>
        public async Task<Result<List<UserPermissionsDTO>>> GetUserScreenPermissionsAsync(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0)
                return Result.Failure<List<UserPermissionsDTO>>(Error.Problem("User.InvalidId", "Invalid user ID format"));

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, cancellationToken);

            if (user == null)
                return Result.Failure<List<UserPermissionsDTO>>(Error.NotFound("User.NotFound", "User not found"));

       
            var userPermissions = new UserPermissionsDTO
            {
                ProfileId = 1, 
                Permissions = new List<ScreenPermission>()
            };

            return Result.Success(new List<UserPermissionsDTO> { userPermissions });
        }
    }
}