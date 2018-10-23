using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using AlbumsReviewRESTApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetArtist(Guid id)
        {
            if (id == null)
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
        public async Task<IActionResult> CreateArtist([FromBody] Artist artist)
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

            _albumsReviewRepository.AddArtist(artist);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception("creating an artist failed on save");
            }

            return CreatedAtRoute("GetArtist", new { id = artist.Id }, artist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArtist(Guid id, [FromBody] Artist artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }


            //  upserting
            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);
            if (artistFromRepo == null)
            {
                artist.Id = id;
                _albumsReviewRepository.AddArtist(artist);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting for artist {id} failed");
                }

                return CreatedAtRoute("GetArtist", new { id = artist.Id }, artist);

            }

            _albumsReviewRepository.UpdateArtistAsync(artist);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"updating for artist {id} failed");
            }

            return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateArtist(Guid id, [FromBody] JsonPatchDocument<Artist> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);

            //  upserting
            if (artistFromRepo == null)
            {
                var artist = new Artist();

                patchDoc.ApplyTo(artist, ModelState);

                TryValidateModel(artist);

                if (!ModelState.IsValid)
                {
                    return new ErrorProcessingEntityObjectResult(ModelState);
                }

                artist.Id = id;

                _albumsReviewRepository.AddArtist(artist);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting for artist {id} failed");
                }

                return CreatedAtRoute("GetArtist", new { id = artist.Id }, artist);

            }

            patchDoc.ApplyTo(artistFromRepo, ModelState);

            TryValidateModel(artistFromRepo);

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"updating for artist {id} failed");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var artistFromRepo = await _albumsReviewRepository.GetArtistAsync(id);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            _albumsReviewRepository.DeleteArtistAsync(artistFromRepo);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting for artist {id} failed");
            }

            return Ok();
        }
    }
}
