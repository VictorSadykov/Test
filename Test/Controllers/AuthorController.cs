using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc;
using Test.DTO;
using Test.Models;
using Test.Repositories.Abstract;
using Test.Repositories.Real;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public AuthorController(IAuthorRepository authorRepository, IMapper mapper, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Author>))]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _authorRepository.GetAuthors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(authors);
        }

        [HttpGet("{authorId}")]
        [ProducesResponseType(200, Type = typeof(Author))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuthorById(int authorId)
        {
            if (!await _authorRepository.AuthorExists(authorId))
                return NotFound();

            var book = await _authorRepository.GetAuthorById(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(book);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDTO author)
        {
            if (author is null)
                return BadRequest(ModelState);

            var foundAuthor = await _authorRepository.GetAuthorByName(author.Name);

            if (foundAuthor is not null) 
            { 
                ModelState.AddModelError("", "Author already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorMap = _mapper.Map<Author>(author);

            if (!await _authorRepository.Create(authorMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpDelete("{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            if (!await _authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksToDelete = (await _authorRepository.GetAllBooksAuthorWrote(authorId)).ToList();
            var authorToDelete = await _authorRepository.GetAuthorById(authorId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!await _bookRepository.DeleteBooks(booksToDelete))
            {
                ModelState.AddModelError("", "Something went wrong when deleting books");
            }

            if (!await _authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", "Something went wrong when deleting authors");
            }

            return NoContent();
        }
    }
}
