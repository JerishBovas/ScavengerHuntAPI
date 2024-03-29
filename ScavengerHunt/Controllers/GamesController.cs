﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService gameRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<GamesController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;

        public GamesController(IGameService gameRepo, IUserService userRepo, ILogger<GamesController> logger, IHelperService help, IBlobService blob, IMapper mapper)
        {
            this.gameRepo = gameRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
            this.mapper = mapper;
        }

        // GET: api/Game
        [Authorize, HttpGet]
        public async Task<ActionResult<List<GameDto>>> Get([FromQuery]string category = "all", [FromQuery]int index = 0, [FromQuery]int count = 10)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var games = await gameRepo.GetAllAsync();
                games.RemoveAll(g => g.IsPrivate == true && g.UserId != userId);

                if(category == "user")
                {
                    games.RemoveAll(g => g.UserId != userId);
                }
                else if(category == "other")
                {
                    games.RemoveAll(g => g.UserId == userId);
                }

                if(index >= games.Count) return new List<GameDto>();
                var shortGames = games.GetRange(index, count > games.Count - index ? games.Count - index : count);

                return mapper.Map<List<GameDto>>(shortGames);
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // GET api/Game/5
        [Authorize, HttpGet("{id}")]
        public async Task<ActionResult<GameDetailDto>> Get(string id, [FromQuery] string userId)
        {
            try
            {
                string currentUserId = helpService.GetCurrentUserId(this.User);
                var game = await gameRepo.GetAsync(id, userId);
                if(game == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found."}));}
                if(game.IsPrivate && game.UserId != currentUserId){return NotFound(new CustomError("Not Found", 404, new string[]{"You don't have access to the game."}));}

                var gameDto = mapper.Map<GameDetailDto>(game);
                gameDto.IsUser = currentUserId == gameDto.UserId ? true : false;
                return gameDto;
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // POST api/Game
        [Authorize, HttpPost]
        public async Task<ActionResult> Create([FromBody] GameCreateDto res)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                Game newGame = mapper.Map<Game>(res);
                newGame.UserId = user.Id;
                user.Games.Add(newGame.Id.ToString());

                await gameRepo.CreateAsync(newGame);
                await gameRepo.SaveChangesAsync();

                return CreatedAtAction(nameof(Create), new { newGame.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[] { "An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it." }));
            }
        }

        // POST api/Game
        [Authorize, HttpPost]
        public async Task<ActionResult> Create()
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                var form = await Request.ReadFormAsync();
                var imageFile = form.Files.GetFile("image");
                var json = form["json"].ToString();

                if(imageFile == null) return BadRequest(new CustomError("Invalid Image", 400, new string[]{"Please upload a valid image"}));
                string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                string url = await blobService.UploadImage("games", name, imageFile.OpenReadStream());

                // Deserialize JSON object
                if(string.IsNullOrEmpty(json)) return BadRequest(new CustomError("Invalid Image", 400, new string[]{"Please upload a valid image"}));
                var game = JsonConvert.DeserializeObject<Game>(json);
                Game newGame = mapper.Map<Game>(game);
                newGame.UserId = user.Id;
                newGame.ImageName = url;
                user.Games.Add(newGame.Id.ToString());

                await gameRepo.CreateAsync(newGame);
                await gameRepo.SaveChangesAsync();

                return Created(nameof(Create), mapper.Map<GameDto>(newGame));
            }catch(Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // PUT api/Game/mode
        [Authorize, HttpPost("{id}/mode")]
        public async Task<ActionResult> ChangeMode(string id, [FromQuery] bool edit)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                var game = await gameRepo.GetAsync(id, user.Id);
                if(game == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found"}));}
                
                game.LastUpdated = DateTimeOffset.UtcNow;
                game.IsReadyToPlay = !edit;
                await gameRepo.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // PUT api/Game/5
        [Authorize, HttpPut("{id}")]
        public async Task<ActionResult> Update(string id)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                var form = await Request.ReadFormAsync();
                var imageFile = form.Files.GetFile("image");
                var json = form["json"].ToString();

                var game = await gameRepo.GetAsync(id, user.Id);
                if(game == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found"}));}

                var res = JsonConvert.DeserializeObject<GameCreateDto>(json);
                mapper.Map<GameCreateDto, Game>(res, game);

                if(imageFile != null)
                {
                    string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                    string url = await blobService.UploadImage("games", name, imageFile.OpenReadStream());
                    game.ImageName = url;
                }
                
                game.LastUpdated = DateTimeOffset.UtcNow;
                game.IsReadyToPlay = false;
                await gameRepo.SaveChangesAsync();

                return Ok(mapper.Map<GameDto>(game));
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
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                var game = await gameRepo.GetAsync(id, user.Id);
                if(game == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found"}));}

                if(game.IsReadyToPlay){return BadRequest(new CustomError("Not Permitted", 404, new string[]{"Operation not permitted. Enter edit mode first and try again."}));}

                string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                string url = await blobService.UploadImage("games", name, file.ImageFile.OpenReadStream());
                
                game.ImageName = url;
                game.LastUpdated = DateTimeOffset.UtcNow;
                await gameRepo.SaveChangesAsync();
                return Created(url, new {ImagePath = url});
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // DELETE api/Game/5
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

                var game = await gameRepo.GetAsync(id, user.Id);
                if (game == null || game.UserId != user.Id) { return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found"})); }

                gameRepo.Delete(game);
                user.Games.Remove(game.Id.ToString());
                await gameRepo.SaveChangesAsync();

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
