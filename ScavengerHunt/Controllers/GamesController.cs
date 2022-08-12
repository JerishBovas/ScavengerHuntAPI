using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService gameRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<GamesController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;

        public GamesController(IGameService gameRepo, IUserService userRepo, ILogger<GamesController> logger, IHelperService help, IBlobService blob)
        {
            this.gameRepo = gameRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
        }

        // GET: api/Game
        [Authorize, HttpGet]
        public async Task<ActionResult<List<GameDto>>> Get()
        {
            List<Game> games;
            List<GameDto> gamedto = new();

            games =  await gameRepo.GetAllAsync();

            if(!games.Any())
            {
                return gamedto;
            }

            foreach(var game in games)
            {
                if (game.IsPrivate)
                {
                    continue;
                }
                GameDto gameDto = new()
                {
                    Id = game.id,
                    IsPrivate = game.IsPrivate,
                    Name = game.Name,
                    Description = game.Description,
                    Address = game.Address,
                    Country = game.Country,
                    Coordinate = new CoordinateDto()
                    {
                        Latitude = game.Coordinate.Latitude,
                        Longitude = game.Coordinate.Longitude
                    },
                    ImageName = game.ImageName,
                    Difficulty = game.Difficulty,
                    Ratings = game.Ratings.Count > 0 ? game.Ratings.Sum()/game.Ratings.Count : 0,
                    Tags = game.Tags
                };
                gamedto.Add(gameDto);
            }

            return gamedto;
        }

        // GET api/Game/5
        [Authorize, HttpGet("{id}")]
        public async Task<ActionResult<GameDetailDto>> Get(Guid id)
        {
            GameDetailDto gamedto;
            List<ItemDto> items = new();

            var game = await gameRepo.GetByIdAsync(id);
            if(game == null){return NotFound("Requested game not found");}

            if (game.Items.Any())
            {
                foreach(var item in game.Items)
                {
                    ItemDto itemDto = new()
                    {
                        Id = item.id,
                        Name = item.Name,
                        Description = item.Description,
                        ImageName = item.ImageName,
                    };
                    items.Add(itemDto);
                }
            }

            gamedto = new()
            {
                Id = game.id,
                IsPrivate = game.IsPrivate,
                Name = game.Name,
                Description = game.Description,
                Address = game.Address,
                Country = game.Country,
                Coordinate = new()
                {
                    Latitude = game.Coordinate.Latitude,
                    Longitude = game.Coordinate.Longitude
                },
                Items = items,
                ImageName = game.ImageName,
                Difficulty = game.Difficulty,
                Ratings = game.Ratings.Count > 0 ? game.Ratings.Sum()/game.Ratings.Count : 0,
                Tags = game.Tags
            };

            return gamedto;
        }

        // POST api/Game
        [Authorize, HttpPost]
        public async Task<ActionResult> Create([FromBody] GameCreateDto res)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null){return NotFound("User does not exist");}

            Coordinate coordinate = new()
            {
                Latitude = res.Coordinate.Latitude,
                Longitude = res.Coordinate.Longitude,
            };

            Game newgame = new()
            {
                IsPrivate = res.IsPrivate,
                Name = res.Name,
                Description = res.Description,
                Address = res.Address,
                Country = res.Country,
                UserId = user.id,
                Coordinate = coordinate,
                ImageName = res.ImageName,
                Difficulty = res.Difficulty,
                Tags = res.Tags
            };

            try
            {
                await gameRepo.CreateAsync(newgame);
                await gameRepo.SaveChangesAsync();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtAction(nameof(Create), new { newgame.id});
        }

        // PUT api/Game/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] GameCreateDto res)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User does not exist"); }

            var game = await gameRepo.GetAsync(id, user.id);
            if (game == null){ return NotFound("Game not found");}

            var newgame = game with
            {
                IsPrivate = res.IsPrivate,
                Name = res.Name,
                Description = res.Description,
                Address = res.Address,
                Country = res.Country,
                Coordinate = new()
                {
                    Latitude = res.Coordinate.Latitude,
                    Longitude = res.Coordinate.Longitude,
                },
                ImageName = res.ImageName,
                Difficulty = res.Difficulty,
                Tags = res.Tags,
                LastUpdated = DateTime.Now
            };

            try
            {
                gameRepo.UpdateAsync(newgame);
                await gameRepo.SaveChangesAsync();
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
                string url = await blobService.SaveImage("games", file.ImageFile, name);
                return Created(url, new {ImagePath = url});
            }
            catch (Exception e)
            {
                logger.LogInformation("Possible Storage Error", e);
                return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
            }
        }

        // DELETE api/Game/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User does not exist"); }

            try
            {
                gameRepo.DeleteAsync(id, user.id);
                await gameRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    
        // POST api/Game
        [Authorize, HttpPost("{id}/items")]
        public async Task<ActionResult> CreateItem(Guid id, [FromBody] ItemCreateDto res)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null){return NotFound("User does not exist");}

            var game = await gameRepo.GetAsync(id, user.id);
            if(game == null){ return NotFound("Game does not exist!");}

            var item = new Item
            {
                Name = res.Name,
                Description = res.Description,
                ImageName = res.ImageName
            };
            game.Items.Add(item);

            try
            {
                gameRepo.UpdateAsync(game);
                await gameRepo.SaveChangesAsync();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // PUT api/Game/5
        [Authorize, HttpPut("{id}/items/{itemId}")]
        public async Task<ActionResult> UpdateItem(Guid id, Guid itemId, [FromBody] ItemCreateDto res)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User does not exist"); }

            var game = await gameRepo.GetAsync(id, user.id);
            if (game == null){ return NotFound("Game not found");}

            var item = game.Items.Where(x => x.id == itemId).FirstOrDefault();
            if (item == null){ return NotFound("Item not found");}

            item.Name = res.Name;
            item.Description = res.Description;
            item.ImageName = res.ImageName;

            try
            {
                gameRepo.UpdateAsync(game);
                await gameRepo.SaveChangesAsync();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // DELETE api/Game/5
        [Authorize]
        [HttpDelete("{id}/items/{itemId}")]
        public async Task<ActionResult> DeleteItem(Guid id, Guid itemId)
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User does not exist"); }

            var game = await gameRepo.GetAsync(id, user.id);
            if(game == null){ return NotFound("Game does not exist!");}

            var item = game.Items.Where(x => x.id == itemId).FirstOrDefault();
            if (item == null){ return NotFound("Item not found");}

            game.Items.Remove(item);

            try
            {
                gameRepo.UpdateAsync(game);
                await gameRepo.SaveChangesAsync();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

    }
}
