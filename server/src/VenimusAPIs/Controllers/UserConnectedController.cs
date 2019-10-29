using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class UserConnectedController : ControllerBase
    {
        private readonly Services.Mongo _mongo;

        private readonly IMapper _mapper;

        private readonly IHttpClientFactory _httpClientFactory;

        public UserConnectedController(Services.Mongo mongo, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _mongo = mongo;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        ///     Called once the user has connected to the frontend.  This ensures that they exist in the database.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/connected
        ///     {
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the user</returns>
        /// <response code="201">Success</response>
        [Route("api/Users/Connected")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post()
        {
            var client = _httpClientFactory.CreateClient("Auth0");

            // sub
            var id = (User.Identity as System.Security.Claims.ClaimsIdentity).Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            var accessToken = User.FindFirst("access_token")?.Value;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = await client.GetStringAsync("/userinfo");
            var profile = JsonSerializer.Deserialize<UserProfile>(json);

            var newUser = new Models.User
            {
                Identities = new string[] { id },
                EmailAddress = profile.Email,
            };

            await _mongo.StoreUser(newUser);

            return NoContent();
        }

        private class UserProfile
        {
            [System.Text.Json.Serialization.JsonPropertyName("email")]
            public string Email { get; set; }
        }
    }
}