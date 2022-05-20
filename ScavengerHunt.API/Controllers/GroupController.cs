﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.API.DTOs;
using ScavengerHunt.API.Library;
using ScavengerHunt.API.Models;
using ScavengerHunt.API.Services;

namespace ScavengerHunt.API.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository groupRepo;
        private readonly IUserRepository userRepo;
        private readonly IScoreLogRepository scoreLogRepo;
        private readonly ILogger<GroupController> logger;

        public GroupController(IGroupRepository groupRepo, IUserRepository userRepo, ILogger<GroupController> logger, IScoreLogRepository scoreLog)
        {
            this.groupRepo = groupRepo;
            this.userRepo = userRepo;
            scoreLogRepo = scoreLog;
            this.logger = logger;
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
                    UniqueId = group.UniqueId,
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
            if (group == null){ return NotFound("Group doesn't exist");}

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
        public async Task<ActionResult> Create([FromBody] GroupCreateDto res)
        {
            User? user;
            Group newgrp;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user == null) { return NotFound("User not found"); }

            newgrp = new()
            {
                UniqueId = Guid.NewGuid(),
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
                CreatedUser = user.Email,
                Members = new List<User> { user },
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
        public async Task<ActionResult> Update(int id, [FromBody] GroupCreateDto res)
        {
            User? user;
            Group? grp;
            Group newgrp;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user == null) { return Unauthorized("User not found"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null){return NotFound("Group doesn't exist");}

            if (grp.CreatedUser != user.Email){return Forbid("Action only allowed by Owner");}

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
            User? user;
            Group? grp;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user is null) { return NotFound("User not found"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null) { return NotFound("Group doesn't exist"); }

            if (grp.CreatedUser != user.Email) { return Forbid("Action only allowed by Owner"); }

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

        //GET /home/userlog
        [Authorize]
        [HttpGet("scores/{id}")]
        public async Task<ActionResult> GetScoreLog(int id)
        {
            User? user;
            Group? grp;
            IEnumerable<ScoreLog> scoreLogList;
            List<ScoreLogDto> scoreloglist = new();

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user is null) { return NotFound("User doesn't exist"); }

            grp = await groupRepo.GetAsync(id);
            if (grp == null) { return NotFound("Group doesn't exist"); }

            if (grp.CreatedUser != user.Email) { return Forbid("Action only allowed by Owner"); }

            scoreLogList = await scoreLogRepo.GetScoreLogsByGroup(grp);

            foreach (var scorelog in scoreLogList)
            {
                ScoreLogDto newdt = new()
                {
                    Id = scorelog.Id,
                    DatePlayed = scorelog.DatePlayed,
                    LocationName = scorelog.LocationName,
                    Score = scorelog.Score,
                };
                scoreloglist.Add(newdt);
            }

            return Content(JsonConvert.SerializeObject(scoreLogList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
