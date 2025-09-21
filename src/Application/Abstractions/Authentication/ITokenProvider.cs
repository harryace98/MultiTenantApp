using Domain.DTOs;
using Domain.Models;
using System.Collections.Immutable;
namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateJWTToken(User user, ImmutableHashSet<string> permissions);
    IUserContext ValidateToken(string token);
}
