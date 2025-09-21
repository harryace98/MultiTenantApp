using Domain.Models;
using System;
using System.Collections.Generic;

namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    int Id { get; set; }
    string Email { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    ICollection<string> Roles { get; set; }
}
