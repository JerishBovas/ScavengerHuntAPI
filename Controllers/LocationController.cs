﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt_API.DTOs;
using ScavengerHunt_API.Library;
using ScavengerHunt_API.Models;
using ScavengerHunt_API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ScavengerHunt_API.Controllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository locRepo;
        private readonly IUserRepository userRepo;

        public LocationController(ILocationRepository locRepo, IUserRepository userRepo)
        {
            this.locRepo = locRepo;
            this.userRepo = userRepo;
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
            string email;
            User? user;
            Location newloc;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is ""){return BadRequest("User not logged in");}

            user = await userRepo.GetByEmailAsync(email);
            if (user == null){return NotFound("User not found");}

            newloc = new()
            {
                IsPrivate = res.IsPrivate,
                Name = res.Name,
                Description = res.Description,
                Address = res.Address,
                UserId = user.Id,
                User = user,
                Coordinate = new()
                {
                    Latitude = res.Coordinate.Latitude,
                    Longitude = res.Coordinate.Longitude,
                },
                Rooms = new List<Room>(),
                ImageName = res.ImageName,
                Difficulty = res.Difficulty,
                Ratings = "",
                Tags = res.Tags,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
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
            string email;
            User? user;
            Location? loc;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is "") { return BadRequest("User not logged in"); }

            user = await userRepo.GetByEmailAsync(email);
            if (user == null) { return NotFound("User not found"); }

            loc = await locRepo.GetAsync(id);
            if (loc == null){ return NotFound("Location not found");}

            if(user.Id != loc.UserId){ return Forbid("You dont have access");}

            Location newloc = loc with
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
            string email;
            User? user;
            Location? newloc;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is "") { return BadRequest("User not logged in"); }

            user = await userRepo.GetByEmailAsync(email);
            if (user == null) { return NotFound("User not found"); }

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
