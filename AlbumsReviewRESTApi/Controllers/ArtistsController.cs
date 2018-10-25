using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using AlbumsReviewRESTApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Models;
using AutoMapper;

namespace AlbumsReviewRESTApi.Controllers
{
    [Route("api/artists")]
    public class ArtistsController : Controller
    {
        private IAlbumsReviewRepository _albumsReviewRepository;

        public ArtistsController(IAlbumsReviewRepository albumsReviewRepository)
        {
            _albumsReviewRepository = albumsReviewRepository;
        }


        [HttpGet]
        [ArtistResultFilter]
        public async Task<IActionResult> GetArtists(ArtistsRequestParameters artistsRequestParameters)
        {
            //  ártistsrequestparamters is for:

            //  TODO: add support for OrderBy

            //  TODO: make sure requested fields for data shaping exist

            //  TODO: add pagination

            var artistEntities = await _albumsReviewRepository.GetArtistsAsync(artistsRequestParameters);
            return Ok(artistEntities);
        }


        [HttpGet("{id}", Name = "GetArtist")]
        [ArtistResultFilter]
        public async Task<IActionResult> GetArtist([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            return Ok(artistFromRepo);
        }

        [HttpPost]
        [ArtistResultFilter]
        public async Task<IActionResult> CreateArtist([FromBody] ArtistForCreationDto artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }

            TryValidateModel(artist);


            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            var artistEntity = Mapper.Map<Artist>(artist);

            _albumsReviewRepository.AddArtist(artistEntity);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception("creating an artist failed on save");
            }

            return CreatedAtRoute("GetArtist", new { id = artist.Id }, artistEntity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArtist([FromRoute] Guid id, [FromBody] ArtistForUpdateDto artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }

            if (id == Guid.Empty)
            {
                return BadRequest();
            }

          
            
            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);
            //  upserting
            if (artistFromRepo == null)
            {
                var artistEntity = Mapper.Map<Artist>(artist);
                artistEntity.Id = id;
                _albumsReviewRepository.AddArtist(artistEntity);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting for artist {id} failed");
                }

                var artistToReturn = Mapper.Map<ArtistDto>(artistEntity);

                return CreatedAtRoute("GetArtist", new { id = artistToReturn.Id }, artistToReturn);

            }

            Mapper.Map(artist, artistFromRepo);
            _albumsReviewRepository.UpdateArtist(artistFromRepo);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"updating for artist {id} failed");
            }

            return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateArtist([FromRoute] Guid id, [FromBody] JsonPatchDocument<ArtistForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);

            //  upserting
            if (artistFromRepo == null)
            {
                var artistForUpdateDto = new ArtistForUpdateDto();

                patchDoc.ApplyTo(artistForUpdateDto, ModelState);

                TryValidateModel(artistForUpdateDto);

                if (!ModelState.IsValid)
                {
                    return new ErrorProcessingEntityObjectResult(ModelState);
                }

                var artistToAdd = Mapper.Map<Artist>(artistForUpdateDto);
                 artistToAdd.Id = id;

                _albumsReviewRepository.AddArtist(artistToAdd);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting for artist {id} failed");
                }

                var artistToReturn = Mapper.Map<ArtistDto>(artistToAdd);

                return CreatedAtRoute("GetArtist", new { id = artistToReturn.Id }, artistToReturn);

            }

            var artistToPatch = Mapper.Map<ArtistForUpdateDto>(artistFromRepo);

            patchDoc.ApplyTo(artistToPatch, ModelState);

            TryValidateModel(artistToPatch);

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            Mapper.Map(artistToPatch, artistFromRepo);

            _albumsReviewRepository.UpdateArtist(artistFromRepo);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"updating for artist {id} failed");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            _albumsReviewRepository.DeleteArtist(artistFromRepo);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting for artist {id} failed");
            }

            return Ok();
        }
    }
}
