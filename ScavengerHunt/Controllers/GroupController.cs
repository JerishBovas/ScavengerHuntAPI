using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService groupRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<GroupController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;

        public GroupController(IGroupService groupRepo, IUserService userRepo, ILogger<GroupController> logger, IHelperService help, IBlobService blob)
        {
            this.groupRepo = groupRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
        }

        // GET: api/group
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> Get()
        {
            List<Group> Groups;
            List<GroupDto> GroupsDto = new();

            Groups = await groupRepo.GetAllAsync();

            if (!Groups.Any())
            {
                return NoContent();
            }

            foreach (var group in Groups)
            {
                if (!group.IsOpen)
                {
                    continue;
                }
                GroupDto grpDto = new()
                {
                    Id = group.Id,
                    IsOpen = group.IsOpen,
                    Title = group.Title,
                    Description = group.Description
                };
                GroupsDto.Add(grpDto);
            }
            return GroupsDto;
        }

        // GET api/group/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDetailDto>> Get(Guid id)
        {
            GroupDetailDto dto;
            Group? group;
            List<UserDto> userdt = new();
            List<ScoreLogDto> scoreLog = new();

            group = await groupRepo.GetByIdAsync(id);
            if (group == null){ return NotFound("Group doesn't exist");}

            if (group.PastWinners.Any())
            {
                foreach (var item in group.PastWinners)
                {
                    ScoreLogDto scoreLogDto = new()
                    {
                        GameName = item.GameName,
                        DatePlayed = item.DatePlayed,
                        Score = item.Score

                    };
                    scoreLog.Add(scoreLogDto);
                }
            }

            dto = new()
            {
                Id = group.Id,
                IsOpen = group.IsOpen,
                Title = group.Title,
                Description = group.Description,
                Members = group.Members,
                PastWinners = scoreLog,
            };

            return dto;
        }

        // POST api/group
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] GroupCreateDto res)
        {
            Group newgrp;

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User not found"); }

            newgrp = new()
            {
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
                GroupIcon = res.GroupIcon,
                CreatedUserId = user.Id,
                Members = new List<Guid> { user.Id},
                PastWinners = new List<ScoreLog>(),
            };

            try
            {
                await groupRepo.CreateAsync(newgrp);
                await groupRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtAction(nameof(Get), new {Id = newgrp.Id});
        }

        // PUT api/group/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] GroupCreateDto res)
        {
            Group newgrp;

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return Unauthorized("User not found"); }

            var grp = await groupRepo.GetAsync(id, user.Id);
            if (grp == null){return NotFound("Group doesn't exist");}

            newgrp = grp with
            {
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
                GroupIcon = res.GroupIcon
            };

            try
            {
                groupRepo.UpdateAsync(newgrp);
                await groupRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // DELETE api/group/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User not found"); }

            try
            {
                groupRepo.DeleteAsync(id, user.Id);
                await groupRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
