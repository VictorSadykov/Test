using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Test.DTO;
using Test.Models;
using Test.Repositories.Abstract;
using Test.Repositories.Real;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tag>))]
        public async Task<IActionResult> GetTags()
        {
            var tag = await _tagRepository.GetTags();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tag);
        }

        [HttpGet("{tagId}")]
        [ProducesResponseType(200, Type = typeof(Tag))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTagById(int tagId)
        {
            if (!await _tagRepository.TagExists(tagId))
                return NotFound();

            var tag = await _tagRepository.GetTagById(tagId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tag);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTag([FromBody] TagDTO tag)
        {
            if (tag is null)
                return BadRequest(ModelState);

            var foundAuthor = await _tagRepository.GetTagByName(tag.Name);

            if (foundAuthor is not null)
            {
                ModelState.AddModelError("", "Author already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tagMap = _mapper.Map<Tag>(tag);

            if (!await _tagRepository.Create(tagMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
    }
}
