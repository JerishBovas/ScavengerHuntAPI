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
        private readonly IRepositoryService<Group> groupRepo;
        private readonly IRepositoryService<User> userRepo;
        private readonly ILogger<GroupController> logger;
        private readonly IHelperService helpMethod;

        public GroupController(IRepositoryService<Group> groupRepo, IRepositoryService<User> userRepo, ILogger<GroupController> logger, IHelperService help)
        {
            this.groupRepo = groupRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpMethod = help;
        }

        // GET: api/group
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get()
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
            return Content(JsonConvert.SerializeObject(GroupsDto, Formatting.Indented));
        }

        // GET api/group/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            GroupDetailDto dto;
            Group? group;
            List<UserDto> userdt = new();
            List<ScoreLogDto> scoreLog = new();

            group = await groupRepo.GetAsync(id);
            if (group == null){ return NotFound("Group doesn't exist");}

            if (group.PastWinners.Any())
            {
                foreach (var item in group.PastWinners)
                {
                    ScoreLogDto scoreLogDto = new()
                    {
                        LocationName = item.LocationName,
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

            return Content(JsonConvert.SerializeObject(dto, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        // POST api/group
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] GroupCreateDto res)
        {
            User? user;
            Group newgrp;

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User not found"); }

            newgrp = new()
            {
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
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

            return CreatedAtAction(nameof(Get), res);
        }

        // PUT api/group/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] GroupCreateDto res)
        {
            User? user;
            Group? grp;
            Group newgrp;

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user == null) { return Unauthorized("User not found"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null){return NotFound("Group doesn't exist");}

            if (grp.CreatedUserId != user.Id){return Forbid("Action only allowed by Owner");}

            newgrp = grp with
            {
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description
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
            User? user;
            Group? grp;

            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User not found"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null) { return NotFound("Group doesn't exist"); }

            if (grp.CreatedUserId != user.Id) { return Forbid("Action only allowed by Owner"); }

            try
            {
                groupRepo.DeleteAsync(grp.Id);
                await groupRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        //GET /home/userlog
        [Authorize]
        [HttpGet("scores/{id}")]
        public async Task<ActionResult> GetScoreLog(Guid id)
        {
            User? user;
            Group? grp;
            List<ScoreLogDto> scoreloglist = new();

            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User doesn't exist"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null) { return NotFound("Group doesn't exist"); }

            if (grp.CreatedUserId != user.Id) { return Forbid("Action only allowed by Owner"); }

            foreach (var scorelog in grp.PastWinners)
            {
                ScoreLogDto newdt = new()
                {
                    DatePlayed = scorelog.DatePlayed,
                    LocationName = scorelog.LocationName,
                    Score = scorelog.Score,
                };
                scoreloglist.Add(newdt);
            }

            return Content(JsonConvert.SerializeObject(scoreloglist, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
