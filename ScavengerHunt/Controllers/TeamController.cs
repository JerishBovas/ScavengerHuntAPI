using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService teamRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<TeamController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;

        public TeamController(ITeamService teamRepo, IUserService userRepo, ILogger<TeamController> logger, IHelperService help, IBlobService blob, IMapper mapper)
        {
            this.teamRepo = teamRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
            this.mapper = mapper;
        }

        // GET: api/team
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<TeamDto>>> Get()
        {
            try
            {
                var teams = await teamRepo.GetAllAsync();
                teams.RemoveAll(g => g.IsOpen == false);
                
                return mapper.Map<List<TeamDto>>(teams);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // GET api/team/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> Get(Guid id)
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TeamCreateDto res)
        {
            try
            {
                var user = await helpService.GetCurrentUser(HttpContext);
                if (user == null) { return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"})); }

                Team newgrp = new()
                {
                    AdminId = user.Id,
                    IsOpen = res.IsOpen,
                    Title = res.Title,
                    Description = res.Description,
                    TeamIcon = res.TeamIcon,
                };

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
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] TeamCreateDto res)
        {
            try
            {
                var user = await helpService.GetCurrentUser(HttpContext);
                if (user == null) { return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"})); }

                var grp = await teamRepo.GetAsync(user.Id, id);
                if (grp == null){return NotFound(new CustomError("Login Error", 404, new string[]{"Team doesn't exist"}));}

                Team newgrp = grp with
                {
                    IsOpen = res.IsOpen,
                    Title = res.Title,
                    Description = res.Description,
                    TeamIcon = res.TeamIcon
                };

                teamRepo.UpdateAsync(newgrp);
                await teamRepo.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        [Authorize]
        [HttpPut("image")]
        public async Task<ActionResult> UploadImage([FromForm] ImageForm file)
        {
            try
            {
                if(file.ImageFile == null) return BadRequest(new CustomError("Invalid Image", 400, new string[]{"Please upload a valid image"}));

                var user = await helpService.GetCurrentUser(HttpContext);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
                }

                string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                string url = await blobService.UploadImage("teams", name, file.ImageFile.OpenReadStream());
                return Created(url, new {ImagePath = url});
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // DELETE api/team/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var user = await helpService.GetCurrentUser(HttpContext);
                if (user is null) { return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"})); }

                var team = await teamRepo.GetByIdAsync(id);
                if (team is null || team.AdminId != user.Id) { return NotFound(new CustomError("Not Found", 404, new string[]{"Team doesn't exist."})); }

                teamRepo.DeleteAsync(team);
                await teamRepo.SaveChangesAsync();

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
