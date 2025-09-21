using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserEntity = Domain.Models.User;

namespace Application.Features.User.Command
{
    public class UserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher
        )
    {
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user create command containing user details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a string user ID if successful.</returns>
        public async Task<Result<string>> CreateUserAsync(UserCreateCommand user, CancellationToken cancellationToken)
        {
            if (user == null)
                return Result.Failure<string>(Error.Problem("User.Null", "User cannot be null"));

            if (string.IsNullOrWhiteSpace(user.Idn))
                return Result.Failure<string>(Error.Problem("User.InvalidId", "User ID is required"));

  
            var existingUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);

            if (existingUser != null)
                return Result.Failure<string>(Error.Conflict("User.EmailExists", "User with this email already exists"));

            var newUser = new UserEntity
            {
                Idn = user.Idn,
                FirstName = user.Name,
                Email = user.Email,
                PasswordHash = passwordHasher.Hash(user.Password), 
                IsActive = user.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(newUser.Id.ToString());
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="user">The user update command with updated details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the updated user details if successful.</returns>
        public async Task<Result<UserDTO>> UpdateUserAsync(int userId, UserUpdateCommand user, CancellationToken cancellationToken)
        {
            if (user == null)
                return Result.Failure<UserDTO>(Error.Problem("User.Null", "User cannot be null"));

            if (userId <= 0)
                return Result.Failure<UserDTO>(Error.Problem("User.InvalidId", "Invalid user ID format"));

            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (existingUser == null)
                return Result.Failure<UserDTO>(Error.NotFound("User.NotFound", "User not found"));

            existingUser.Idn = user.Idn;
            existingUser.FirstName = user.Name;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(UserDTO.MapToDTO(existingUser));
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object indicating success or failure.</returns>
        public async Task<Result> DeleteUserAsync(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0)
                return Result.Failure(Error.Problem("User.InvalidId", "Invalid user ID format"));

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return Result.Success();

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}