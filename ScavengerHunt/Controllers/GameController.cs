using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService gameRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<GameController> logger;
        private readonly IHelperService helpMethod;

        public GameController(IGameService gameRepo, IUserService userRepo, ILogger<GameController> logger, IHelperService help)
        {
            this.gameRepo = gameRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpMethod = help;
        }

        // GET: api/Game
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<GameDto>>> Get()
        {
            List<Game> games;
            List<GameDto> gamedto = new();

            games =  await gameRepo.GetAllAsync();

            if(!games.Any())
            {
                return NoContent();
            }

            foreach(var game in games)
            {
                if (game.IsPrivate)
                {
                    continue;
                }
                GameDto gameDto = new()
                {
                    Id = game.Id,
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
                    Ratings = game.Ratings,
                    Tags = game.Tags
                };
                gamedto.Add(gameDto);
            }

            return gamedto;
        }

        // GET api/Game/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDetailDto>> Get(Guid id)
        {
            Game? game;
            GameDetailDto gamedto;
            List<RoomDto> rooms = new();

            game = await gameRepo.GetAsync(id);
            if(game == null){return NotFound("Requested game not found");}

            if (game.Rooms.Any())
            {
                foreach(var room in game.Rooms)
                {
                    RoomDto roomDto = new()
                    {
                        Name = room.Name,
                        Details = room.Details,
                    };
                    rooms.Add(roomDto);
                }
            }

            gamedto = new()
            {
                Id = game.Id,
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
                Rooms = rooms,
                ImageName = game.ImageName,
                Difficulty = game.Difficulty,
                Ratings = game.Ratings,
                Tags = game.Tags
            };

            return gamedto;
        }

        // POST api/Game
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] GameCreateDto res)
        {
            User? user;
            Game newgame;

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user == null){return NotFound("User does not exist");}

            Coordinate coordinate = new Coordinate()
            {
                Latitude = res.Coordinate.Latitude,
                Longitude = res.Coordinate.Longitude,
            };

            newgame = new(res.IsPrivate, res.Name, res.Description, res.Address, res.Country, user.Id, coordinate, res.ImageName, res.Difficulty, res.Tags);

            try
            {
                await gameRepo.CreateAsync(newgame);
                await gameRepo.SaveChangesAsync();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtAction(nameof(Create), new GameDto()
            {
                Id = newgame.Id,
                IsPrivate = newgame.IsPrivate,
                Name = newgame.Name,
                Description = newgame.Description,
                Address = newgame.Address,
                Country = newgame.Country,
                Coordinate = new CoordinateDto()
                {
                    Latitude = newgame.Coordinate.Latitude,
                    Longitude = newgame.Coordinate.Longitude,
                },
                ImageName = newgame.ImageName,
                Difficulty = newgame.Difficulty,
                Tags = newgame.Tags,
                CreatedDate = newgame.CreatedDate,
                LastUpdated = newgame.LastUpdated
            });
        }

        // PUT api/Game/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] GameCreateDto res)
        {
            User? user;
            Game? game;
            Game newgame;

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User does not exist"); }

            game = await gameRepo.GetAsync(id);
            if (game == null){ return NotFound("Game not found");}

            if(user.Id != game.UserId){ return Forbid("You dont have access");}

            newgame = game with
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

        // DELETE api/Game/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            User? user;
            Game? newgame;

            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user == null) { return NotFound("User does not exist"); }

            newgame = await gameRepo.GetAsync(id);
            if (newgame == null) { return NotFound("Game not found"); }

            if (user.Id != newgame.UserId) { return Forbid("You dont have access"); }

            try
            {
                gameRepo.DeleteAsync(newgame.Id);
                await gameRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
