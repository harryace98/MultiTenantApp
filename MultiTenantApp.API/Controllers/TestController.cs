using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantApp.API.Infrastructure;
using SharedKernel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MultiTenantApp.API.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(ITokenProvider tokenProvider, IPermissionProvider permissionProvider) : ControllerBase 
    {
        [HttpGet("GenerateToken")]
        public async Task<IActionResult> GenerateTokenAsync(User user)
        {
            
            var result = tokenProvider.GenerateJWTToken(user, await permissionProvider.GetUserPermissionsAsync(user.Id, user.TenantId));
            return CustomResults.Success<string>(result);
        }

        // GET: api/<TestController>
        [HttpGet]
        [Authorize("view:dashboarddf")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
 }
