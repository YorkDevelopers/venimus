using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Services;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class ActiveGroupsController : ControllerBase
    {
        private readonly Mongo.GroupStore _groupStore;

        private readonly IMapper _mapper;
        private readonly URLBuilder _urlBuilder;

        public ActiveGroupsController(Mongo.GroupStore groupStore, IMapper mapper, URLBuilder urlBuilder)
        {
            _groupStore = groupStore;
            _mapper = mapper;
            _urlBuilder = urlBuilder;
        }

        /// <summary>
        ///     Allows you request the list of active groups
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /public/Groups
        ///
        /// </remarks>
        /// <returns>Array of ListActiveGroups view models</returns>
        /// <response code="200">Success</response>
        [Route("public/Groups")]
        [ResponseCache(Duration = 300)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListActiveGroups[]>> Get()
        {
            var groups = await _groupStore.GetActiveGroups().ConfigureAwait(false);

            var server = $"{Request.Scheme}://{Request.Host}";

            var viewModels = groups.Select(grp => new ListActiveGroups
            {
                Description = grp.Description,
                Name = grp.Name,
                Slug = grp.Slug,
                StrapLine = grp.StrapLine,
                Logo = _urlBuilder.BuildGroupLogoURL(grp.Slug),
            }).ToArray();

            return viewModels;
        }
    }
}