using Application.Abstractions.Data;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserEntity = Domain.Models.User;

namespace Application.Features.User.Query
{
    public class UserQueryHandler(
        IApplicationDbContext context
        )
    {
        public async Task<Result<UserDTO>> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0)
                return Result.Failure<UserDTO>(Error.NotFound("User.NotFound", "User not found"));

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, cancellationToken);

            if (user == null)
                return Result.Failure<UserDTO>(Error.NotFound("User.NotFound", "User not found"));
            // userId = el id del usuario autenticado

            var userData = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                    {
                    user = new
                        {
                        id = u.Id,
                        username = u.FirstName + " " + u.LastName,
                        },
                    roles = u.UserRoles
                        .Select(ur => ur.Role.Name)
                        .ToList(),

                    rawPermissions = u.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Select(rp => new { rp.Permission.Screen, rp.Permission.Action })
                        .ToList()
                    })
                .FirstOrDefaultAsync();

            // Ahora hacemos el GroupBy en memoria
            var result = new
                {
                userData.user,
                userData.roles,
                permissions = userData.rawPermissions
                    .GroupBy(p => p.Screen)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.Action).Distinct().ToList()
                    )
                };

            return Result.Success(UserDTO.MapToDTO(user));
        }

        public async Task<Result<List<UserDTO>>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await context.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .ToListAsync(cancellationToken);

            var userDTOs = users.Select(UserDTO.MapToDTO).ToList();

            return Result.Success(userDTOs);
        }
    }
}