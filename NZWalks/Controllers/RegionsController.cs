using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NZWalks.CustomActionFilter;
using NZWalks.Data;
using NZWalks.Models.Domains;
using NZWalks.Models.DTO;
using NZWalks.Repositories;
using System.Text.Json;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext,
                                 IRegionRepository regionRepository,
                                 IMapper mapper,
                                 ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        //GET ALL REGIONS
        [HttpGet]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                //throw new Exception("this is a custom exception");
                //get data from database-domain model
                var regionsDomain = await regionRepository.GetAllAsync();

                logger.LogInformation($"Finished GetAllRegions request with data:{JsonSerializer.Serialize(regionsDomain)}");
                //return dtos
                return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
            
            
        }

        //GET REGION BY ID
        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
           
            //get data from database-domain model
            var regionsDomain = await regionRepository.GetByIdAsync(id);
            if (regionsDomain == null)
            {
                return NotFound();
            }
            //return dto
            return Ok(mapper.Map<RegionDto>(regionsDomain));
        }
        //POST TO CREATE NEW REGION
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {

                //Map the dto from client to domain model using mapper
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                //use domain model to create region

                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                //use mapper to again map domail model to dto
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { Id = regionDomainModel.Id }, regionDto);

        }


        //PUT TO UPDATE REGION
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
                //convert updaterequest dto to domain model
                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                //convert domain model to dto
                return Ok(mapper.Map<RegionDto>(regionDomainModel));
            
        }

        //DELETE TO DELETE A REGION
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {

            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if(regionDomainModel == null)
            {
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDto);
        }
    }
}
