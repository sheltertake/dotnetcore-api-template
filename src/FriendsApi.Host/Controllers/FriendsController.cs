using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FootballContacts.Repositories;
using FriendsApi.Model.Types;
using FriendsApi.Host.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendsApi.Host.Controllers
{
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly ILogger<FriendsController> _logger;

        private readonly IFriendsRepository _friendsRepository;

        public FriendsController(
            IFriendsRepository friendsRepository,
            ILogger<FriendsController> logger)
        {
            _friendsRepository = friendsRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get Friends
        /// </summary>
        /// <remarks>Get Friends</remarks>
        /// <response code="200">Return the list of Friends</response>
        [HttpGet(Routes.Friends)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Friend>>> GetAsync(bool sync)
        {
            if (sync)
                return Ok(_friendsRepository.List());

            var result = await _friendsRepository.ListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get Friend by Id
        /// </summary>
        /// <remarks>Get Friend by Id</remarks>
        /// <response code="200">Return the friend</response>
        /// <response code="404">Friend not found</response>
        [HttpGet(Routes.Friend)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Friend> Get(int id)
        {

            var ret = _friendsRepository.Get(id);
            if (ret != null)
                return Ok(ret);

            return NotFound();
        }

        /// <summary>
        /// Create Friend
        /// </summary>
        /// <remarks>Create an Friend</remarks>
        /// <response code="201">Friend created</response>
        [HttpPost(Routes.Friends)]
        [ProducesResponseType(typeof(Friend), StatusCodes.Status201Created)]
        public ActionResult<Friend> Post([FromBody] Friend friend)
        {
            _logger.LogInformation(nameof(Post));

            var ret = _friendsRepository
                .Create(friend);

            var routeGet = Routes.Friend.Replace("{id}", ret.Id.ToString());

            return Created($"{routeGet}", ret);
        }

        /// <summary>
        /// Update an Friend
        /// </summary>
        /// <remarks>Update an Friend</remarks>
        /// <response code="204">Friend updated</response>
        /// <response code="404">Friend not found</response>
        [HttpPut(Routes.Friend)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(int id, [FromBody] Friend friend)
        {
            friend.Id = id; // quick and dirty solution 
            var updated = _friendsRepository.Update(friend);

            if (updated)
                return NoContent();

            return NotFound();
        }

        /// <summary>
        /// Delete Friend
        /// </summary>
        /// <remarks>Delete an Friend</remarks>
        /// <response code="204">Friend deleted</response>
        /// <response code="404">Friend not found</response>
        [HttpDelete(Routes.Friend)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var ret = _friendsRepository.Delete(id);

            if (ret)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
