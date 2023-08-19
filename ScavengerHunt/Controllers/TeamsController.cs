using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService teamRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<TeamsController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;

        public TeamsController(ITeamService teamRepo, IUserService userRepo, ILogger<TeamsController> logger, IHelperService help, IBlobService blob, IMapper mapper)
        {
            this.teamRepo = teamRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
            this.mapper = mapper;
        }

        // GET: api/team
        [Authorize, HttpGet]
        public async Task<ActionResult<List<TeamDto>>> Get([FromQuery]string category = "all", [FromQuery]int index = 0, [FromQuery]int count = 10)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }
                var teams = await teamRepo.GetAllAsync();
                teams.RemoveAll(g => g.IsOpen == false);
                
                if(category == "user")
                {
                    teams.RemoveAll(g => g.AdminId != userId);
                }
                else if(category == "other")
                {
                    teams.RemoveAll(g => g.AdminId == userId);
                }

                if(index >= teams.Count) return new List<TeamDto>();
                var shortTeams = teams.GetRange(index, count > teams.Count - index ? teams.Count - index : count);

                return mapper.Map<List<TeamDto>>(shortTeams);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // GET api/team/5
        [Authorize, HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> Get(string id)
        {
            try
            {
                var team = await teamRepo.GetByIdAsync(id);
                if (team == null){ return NotFound(new CustomError("Not Found", 404, new string[]{"Team doesn't exist"}));}

                return mapper.Map<TeamDto>(team);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // POST api/team
        [Authorize, HttpPost]
        public async Task<ActionResult> Create([FromBody] TeamCreateDto res)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null) { return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"})); }

                Team newgrp = mapper.Map<Team>(res);
                newgrp.AdminId = user.Id;
                newgrp.Members.Add(user.Id.ToString());
                user.Teams.Add(newgrp.Id.ToString());

                await teamRepo.CreateAsync(newgrp);
                await teamRepo.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new {newgrp.Id});
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // PUT api/team/5
        [Authorize, HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] TeamCreateDto res)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);

                var grp = await teamRepo.GetAsync(id, userId);
                if (grp == null){return NotFound(new CustomError("Login Error", 404, new string[]{"Team doesn't exist"}));}

                grp = mapper.Map<TeamCreateDto, Team>(res, grp);
                await teamRepo.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        [Authorize, HttpPut("{id}/image")]
        public async Task<ActionResult> UploadImage(string id, [FromForm] ImageForm file)
        {
            try
            {
                if(file.ImageFile == null) return BadRequest(new CustomError("Invalid Image", 400, new string[]{"Please upload a valid image"}));

                string userId = helpService.GetCurrentUserId(this.User);

                string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                string url = await blobService.UploadImage("teams", name, file.ImageFile.OpenReadStream());

                var grp = await teamRepo.GetAsync(id, userId);
                if (grp == null){return NotFound(new CustomError("Login Error", 404, new string[]{"Team doesn't exist"}));}

                grp.TeamIcon = url;
                await teamRepo.SaveChangesAsync();
                return Created(url, new {ImagePath = url});
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // DELETE api/team/5
        [Authorize, HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                var team = await teamRepo.GetAsync(id, user.Id);
                if (team is null) { return NotFound(new CustomError("Not Found", 404, new string[]{"Team doesn't exist."})); }

                string imageName = team.TeamIcon.Split('/').Last();

                teamRepo.DeleteAsync(team);
                user.Teams.Remove(team.Id.ToString());
                await teamRepo.SaveChangesAsync();
                blobService.DeleteImage("teams", imageName);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }
    }
}
