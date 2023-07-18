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
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BookController(IBookRepository bookRepository, ITagRepository tagRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _tagRepository = tagRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        public async Task<IActionResult> GetBooks()
        {
            var books =  await _bookRepository.GetBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(books);            
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBookById(int bookId)
        {
             if (!await _bookRepository.BookExists(bookId))
                return NotFound();

            var book = await _bookRepository.GetBookById(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(book);
        }

        [HttpGet("{bookId}/sugestAuthors")]
        [ProducesResponseType(200, Type = typeof(ICollection<Author>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllAuthorsThatBookByIdDoesntHave(int bookId)
        {
            if (!await _bookRepository.BookExists(bookId))
                return NotFound();

            var authorsThatThisBookDoesntHave = await _bookRepository.GetAllAuthorsThatThisBookDoesntHave(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(authorsThatThisBookDoesntHave);
        }

        [HttpGet("{bookId}/sugestTags")]
        [ProducesResponseType(200, Type = typeof(ICollection<Tag>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllTagsThatBookByIdDoesntHave(int bookId)
        {
            if (!await _bookRepository.BookExists(bookId))
                return NotFound();

            var tagsThatThisBookDoesntHave = await _bookRepository.GetAllTagsThatThisBookDoesntHave(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tagsThatThisBookDoesntHave);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateBook([FromBody] BookDTO book, [FromQuery] List<int> authorIds, [FromQuery] List<int> tagIds)
        {
            if (book is null)
                return BadRequest(ModelState);

            var foundBook = await _bookRepository.GetBookByTitle(book.Title);

            if (foundBook is not null && 
                foundBook.Description == book.Description &&
                foundBook.PublishedOn == book.PublishedOn)
            {
                ModelState.AddModelError("", "Book already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookMap = _mapper.Map<Book>(book);

            bookMap.Authors = new List<Author>();
            bookMap.Tags = new List<Tag>();

            foreach (var tagId in tagIds)
            {
                var tag = await _tagRepository.GetTagById(tagId);
                if (tag is not null)
                    bookMap.Tags.Add(tag);
            }

            foreach (var authorId in authorIds)
            {
                var author = await _authorRepository.GetAuthorById(authorId);
                if (author is not null)
                    bookMap.Authors.Add(author);
            }


            if (!await _bookRepository.Create(bookMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpDelete("{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public async Task<IActionResult> DeleteBook(int bookId)
        {
            if (!await _bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            var bookToDelete = await _bookRepository.GetBookById(bookId);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _bookRepository.Delete(bookToDelete))
            {
                ModelState.AddModelError("", "Something went wrong when deleting books");
            }           

            return NoContent();
        }
    }
}
