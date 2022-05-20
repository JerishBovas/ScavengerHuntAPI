﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.API.DTOs;
using ScavengerHunt.API.Library;
using ScavengerHunt.API.Models;
using ScavengerHunt.API.Services;

namespace ScavengerHunt.API.Controllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository locRepo;
        private readonly IUserRepository userRepo;
        private readonly ILogger<LocationController> logger;

        public LocationController(ILocationRepository locRepo, IUserRepository userRepo, ILogger<LocationController> logger)
        {
            this.locRepo = locRepo;
            this.userRepo = userRepo;
            this.logger = logger;
        }

        // GET: api/Location
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            List<Location> locations;
            List<LocationDto> locdto = new();

            locations =  await locRepo.GetAllAsync();

            if(!locations.Any())
            {
                return NoContent();
            }

            foreach(var loc in locations)
            {
                if (loc.IsPrivate)
                {
                    continue;
                }
                LocationDto locationDto = new()
                {
                    Id = loc.Id,
                    IsPrivate = loc.IsPrivate,
                    Name = loc.Name,
                    Description = loc.Description,
                    Address = loc.Address,
                    Coordinate = new CoordinateDto()
                    {
                        Latitude = loc.Coordinate.Latitude,
                        Longitude = loc.Coordinate.Longitude
                    },
                    ImageName = loc.ImageName,
                    Difficulty = loc.Difficulty,
                    Tags = loc.Tags
                };
                locdto.Add(locationDto);
            }

            return Content(JsonConvert.SerializeObject(locdto, Formatting.Indented));
        }

        // GET api/Location/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            Location? loc;
            LocationDto locdto;

            loc = await locRepo.GetAsync(id);
            if(loc == null){return NotFound("Requested location not found");}

            locdto = new()
            {
                Id = loc.Id,
                IsPrivate = loc.IsPrivate,
                Name = loc.Name,
                Description = loc.Description,
                Address = loc.Address,
                Coordinate = new()
                {
                    Latitude = loc.Coordinate.Latitude,
                    Longitude = loc.Coordinate.Longitude
                },
                ImageName = loc.ImageName,
                Difficulty = loc.Difficulty,
                Tags = loc.Tags
            };

            return Content(JsonConvert.SerializeObject(locdto, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        // POST api/Location
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] LocationDto res)
        {
            User? user;
            Location newloc;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user == null){return NotFound("User does not exist");}

            newloc = new()
            {
                IsPrivate = res.IsPrivate,
                Name = res.Name,
                Description = res.Description,
                Address = res.Address,
                UserId = user.Id,
                User = user,
                Coordinate = new Coordinate()
                {
                    Latitude = res.Coordinate.Latitude,
                    Longitude = res.Coordinate.Longitude,
                },
                Rooms = new List<Room>(),
                ImageName = res.ImageName,
                Difficulty = res.Difficulty,
                Ratings = "",
                Tags = res.Tags,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow,
            };

            try
            {
                await locRepo.CreateAsync(newloc);
                await locRepo.SaveChangesAsync();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // PUT api/Location/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] LocationDto res)
        {
            User? user;
            Location? loc;
            Location newloc;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user == null) { return NotFound("User does not exist"); }

            loc = await locRepo.GetAsync(id);
            if (loc == null){ return NotFound("Location not found");}

            if(user.Id != loc.UserId){ return Forbid("You dont have access");}

            newloc = loc with
            {
                IsPrivate = res.IsPrivate,
                Name = res.Name,
                Description = res.Description,
                Address = res.Address,
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
                locRepo.UpdateAsync(newloc);
                await locRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // DELETE api/Location/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            User? user;
            Location? newloc;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user == null) { return NotFound("User does not exist"); }

            newloc = await locRepo.GetAsync(id);
            if (newloc == null) { return NotFound("Location not found"); }

            if (user.Id != newloc.UserId) { return Forbid("You dont have access"); }

            try
            {
                locRepo.DeleteAsync(newloc);
                await locRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
