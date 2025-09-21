using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Infrastructure.Authentication
{
    internal sealed class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext()
        {
        
        }
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Email { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
