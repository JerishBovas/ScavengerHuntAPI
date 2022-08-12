using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService teamRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<TeamsController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;

        public TeamsController(ITeamService teamRepo, IUserService userRepo, ILogger<TeamsController> logger, IHelperService help, IBlobService blob)
        {
            this.teamRepo = teamRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
        }

        // GET: api/team
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<TeamDto>>> Get()
        {
            List<TeamDto> TeamsDto = new();

            var Teams = await teamRepo.GetAllAsync();

            if (!Teams.Any())
            {
                return TeamsDto;
            }

            foreach (var team in Teams)
            {
                if (!team.IsOpen)
                {
                    continue;
                }
                TeamDto grpDto = new()
                {
                    Id = team.id,
                    IsOpen = team.IsOpen,
                    Title = team.Title,
                    Description = team.Description
                };
                TeamsDto.Add(grpDto);
            }
            return TeamsDto;
        }

        // GET api/team/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDetailDto>> Get(Guid id)
        {
            List<GameScoreDto> scoreLog = new();

            var team = await teamRepo.GetByIdAsync(id);
            if (team == null){ return NotFound("Team doesn't exist");}

            if (team.PastWinners.Any())
            {
                foreach (var item in team.PastWinners)
                {
                    GameScoreDto scoreLogDto = new()
                    {
                        id = item.id,
                        GameId = item.GameId,
                        GameName = item.GameName,
                        NoOfItems = item.NoOfItems,
                        ItemsFound = item.ItemsFound,
                        Score = item.Score,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime

                    };
                    scoreLog.Add(scoreLogDto);
                }
            }

            TeamDetailDto dto = new()
            {
                Id = team.id,
                IsOpen = team.IsOpen,
                Title = team.Title,
                Description = team.Description,
                Members = team.Members,
                PastWinners = scoreLog,
            };

            return dto;
        }

        // POST api/team
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TeamCreateDto res)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User not found"); }

            Team newgrp = new()
            {
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
                TeamIcon = res.TeamIcon,
                CreatedUserId = user.id,
                Members = new List<Guid> { user.id},
                PastWinners = new List<GameScore>(),
            };

            try
            {
                await teamRepo.CreateAsync(newgrp);
                await teamRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtAction(nameof(Get), new {newgrp.id});
        }

        // PUT api/team/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] TeamCreateDto res)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return Unauthorized("User not found"); }

            var grp = await teamRepo.GetAsync(id, user.id);
            if (grp == null){return NotFound("Team doesn't exist");}

            Team newgrp = grp with
            {
                IsOpen = res.IsOpen,
                Title = res.Title,
                Description = res.Description,
                TeamIcon = res.TeamIcon
            };

            try
            {
                teamRepo.UpdateAsync(newgrp);
                await teamRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("image")]
        public async Task<ActionResult> UploadImage([FromForm] FileModel file)
        {
            if(file.ImageFile == null) return BadRequest();

            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null)
            {
                return NotFound(
                    new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
            }

            try
            {
                string date = DateTime.Now.ToBinary().ToString();
                string name = user.id.ToString() + date;
                string url = await blobService.SaveImage("teams", file.ImageFile, name);
                return Created(url, new {ImagePath = url});
            }
            catch (Exception e)
            {
                logger.LogInformation("Possible Storage Error", e);
                return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
            }
        }

        // DELETE api/team/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User not found"); }

            try
            {
                teamRepo.DeleteAsync(id, user.id);
                await teamRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
