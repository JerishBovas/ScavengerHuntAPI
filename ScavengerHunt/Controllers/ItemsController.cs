﻿using Amazon.Rekognition.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/v1/games/{gameId}/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IGameService gameRepo;
        private readonly IUserService userRepo;
        private readonly ILogger<ItemsController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;
        private readonly IClassificationService classificationService;
        private readonly IMapper mapper;

        public ItemsController(IGameService gameRepo, IUserService userRepo, ILogger<ItemsController> logger, IHelperService help, IBlobService blob, IClassificationService classificationService, IMapper mapper)
        {
            this.gameRepo = gameRepo;
            this.userRepo = userRepo;
            this.logger = logger;
            helpService = help;
            blobService = blob;
            this.classificationService = classificationService;
            this.mapper = mapper;
        }

        // This method takes gameId as parameter
        // And returns a list of items in the game
        // GET: api/Games/{id}/Items
        [Authorize, HttpGet]
        public async Task<ActionResult<List<ItemDto>>> Get(Guid gameId)
        {
            try
            {
                var userId = helpService.GetCurrentUserId(HttpContext) ?? Guid.Empty;
                if(userId == Guid.Empty) {
                    return NotFound(new CustomError("Bad Request", 404, new string[]{"User does not exist"}));
                }
                var game = await gameRepo.GetAsync(gameId, userId);
                if(game == null)
                {
                    return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found or you don't have access."}));
                }
                return mapper.Map<List<ItemDto>>(game.Items);
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // This method takes itemId and gameId as parameter
        // And returns itemDto
        // GET api/Game/5
        [Authorize, HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> Get(Guid id, Guid gameId)
        {
            try
            {
                var userId = helpService.GetCurrentUserId(HttpContext) ?? Guid.Empty;
                if(userId == Guid.Empty) {
                    return NotFound(new CustomError("Bad Request", 404, new string[]{"User does not exist"}));
                }
                var game = await gameRepo.GetAsync(gameId, userId);
                if(game == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found or you don't have access."}));}

                var item = game.Items.FirstOrDefault(x => x.Id == id);
                if(item == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested item not found or you don't have access."}));}

                return mapper.Map<ItemDto>(item);
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // This method takes itemcreatedto, gameId as parameters
        // And returns created item id
        // POST api/Game
        [Authorize, HttpPost]
        public async Task<ActionResult> Create([FromBody] ItemCreateDto res, Guid gameId)
        {
            try
            {
                var user = await helpService.GetCurrentUser(HttpContext);
                if (user == null){return NotFound(new CustomError("Bad Request", 404, new string[]{"User does not exist"}));}

                var game = await gameRepo.GetAsync(gameId, user.Id);
                if(game == null){return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found or you don't have access."}));}

                if(game.IsReadyToPlay){return BadRequest(new CustomError("Not Permitted", 404, new string[]{"Operation not permitted. Enter edit mode first and try again."}));}

                var item = mapper.Map<Item>(res);
                game.Items.Add(item);
                game.LastUpdated = DateTimeOffset.UtcNow;
                game.IsReadyToPlay = false;
                
                await gameRepo.SaveChangesAsync();

                return CreatedAtAction(nameof(Create), new { item.Id});
            }catch(Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        // This method takes imageform as parameter
        // And returns created image url
        [Authorize, HttpPut("image")]
        public async Task<ActionResult<ImageLabels>> UploadAndGetLabels(Guid gameId, [FromForm] ImageForm file)
        {
            Image image = new Image();
            ImageLabels imageLabels = new ImageLabels();

            try
            {
                if(file.ImageFile == null) return BadRequest(new CustomError("Invalid Image", 400, new string[]{"Please upload a valid image"}));

                var user = await helpService.GetCurrentUser(HttpContext);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
                }

                string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                imageLabels.Url = await blobService.UploadImage("items", name, file.ImageFile.OpenReadStream());
                
                using(var ms = new MemoryStream())
                {
                    Stream st = file.ImageFile.OpenReadStream();
                    st.CopyTo(ms);
                    image.Bytes = ms;
                }
                
                DetectLabelsRequest detectlabelsRequest = new DetectLabelsRequest()
                {
                    Image = image,
                    MaxLabels = 3,
                    MinConfidence = 90F
                };

                DetectLabelsResponse response = await classificationService.DetectLabels(detectlabelsRequest);

                imageLabels.Labels = response.Labels;

                return Ok(imageLabels);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }
        
        // This method takes id, gameid as parameter
        // And return ok if delete successful
        // DELETE api/Game/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, Guid gameId)
        {
            try
            {
                var user = await helpService.GetCurrentUser(HttpContext);
                if (user == null){return NotFound(new CustomError("Bad Request", 404, new string[]{"User does not exist"}));}

                var game = await gameRepo.GetAsync(gameId, user.Id);
                if (game == null || game.UserId != user.Id) { return NotFound(new CustomError("Not Found", 404, new string[]{"Requested game not found"})); }

                if(game.IsReadyToPlay){return BadRequest(new CustomError("Not Permitted", 404, new string[]{"Operation not permitted. Enter edit mode first and try again."}));}

                var item = game.Items.FirstOrDefault(x => x.Id == id);
                if(item == null) return Ok();

                game.Items.Remove(item);
                game.LastUpdated = DateTimeOffset.UtcNow;
                await gameRepo.SaveChangesAsync();
                blobService.DeleteImage("items", item.ImageName.Split('/').Last());
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
