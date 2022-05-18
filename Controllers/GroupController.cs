using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt_API.DTOs;
using ScavengerHunt_API.Library;
using ScavengerHunt_API.Models;
using ScavengerHunt_API.Services;

namespace ScavengerHunt_API.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository groupRepo;
        private readonly IUserRepository userRepo;

        public GroupController(IGroupRepository groupRepo, IUserRepository userRepo)
        {
            this.groupRepo = groupRepo;
            this.userRepo = userRepo;
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
                    Description = group.Description,
                    CreatedUser = group.CreatedUser,
                };
                GroupsDto.Add(grpDto);
            }
            return Content(JsonConvert.SerializeObject(GroupsDto, Formatting.Indented));
        }

        // GET api/group/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            GroupDto dto;
            Group? group;
            List<UserDto> userdt = new();
            List<ScoreLogDto> scoreLog = new();

            group = await groupRepo.GetAsync(id);
            if (group == null){ return NotFound();}

            if (group.Members.Any())
            {
                foreach (var item in group.Members)
                {
                    UserDto ust = new()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Email = item.Email,
                    };
                    userdt.Add(ust);
                }
            }
            if (group.PastWinners.Any())
            {
                foreach (var item in group.PastWinners)
                {
                    ScoreLogDto scoreLogDto = new()
                    {
                        LocationName = item.LocationName,
                        DatePlayed = item.DatePlayed,
                        Score = item.Score,
                        Id = item.Id,

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
                CreatedUser = group.CreatedUser,
                Members = userdt,
                PastWinners = scoreLog,
            };

            return Content(JsonConvert.SerializeObject(dto, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        // POST api/group
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] GroupDto res)
        {
            string email;
            User? user;
            Group newgrp;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is ""){return BadRequest("User not logged in");}

            user = await userRepo.GetByEmailAsync(email);
            if (user == null){return NotFound("User not found");}

            newgrp = new()
            {
                UniqueId = Guid.NewGuid(),
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
                CreatedUser = email,
                Members = new List<User> { user},
                PastWinners = new List<ScoreLog>()
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

            return Ok();
        }

        // PUT api/group/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] GroupDto res)
        {
            string email;
            User? user;
            Group? grp;
            Group newgrp;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is "") { return BadRequest("User not logged in"); }

            user = await userRepo.GetByEmailAsync(email);
            if (user == null) { return NotFound("User not found"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null){return NotFound("Group not found");}

            if (grp.CreatedUser != email){return Forbid("Action only allowed by Owner");}

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
        public async Task<ActionResult> Delete(int id)
        {
            string email;
            Group? grp;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is "") { return BadRequest("User not logged in"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null) { return NotFound("Group not found"); }

            if (grp.CreatedUser != email) { return Forbid("Action only allowed by Owner"); }

            try
            {
                groupRepo.DeleteAsync(grp);
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
