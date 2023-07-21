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
        private readonly IGenreRepository _genreRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BookController(IBookRepository bookRepository, IGenreRepository tagRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _genreRepository = tagRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        public async Task<IActionResult> GetBooks([FromQuery] int? authorId, [FromQuery] int? genreId, [FromQuery] int? limit, [FromQuery] int? page)
        {
            List<Book> books = new List<Book>();

            

            if (genreId is null && authorId is null)
            {
                books = (await _bookRepository.GetBooks()).ToList();
                
            }
            else if (genreId is null && authorId is not null)
            {
                books = (await _bookRepository.GetBooks())
                    .Where(x => x.AuthorId == authorId)                    
                    .ToList();
            }
            else if (genreId is not null && authorId is null)
            {
                books = (await _bookRepository.GetBooks())
                    .Where(x => x.GenreId == genreId)
                    .ToList();
            }
            else if (genreId is not null && authorId is not null)
            {
                books = (await _bookRepository.GetBooks())
                    .Where(x => x.GenreId == genreId
                        && x.AuthorId == authorId)
                    .ToList();
            }

            var booksLimitedAndPaged = _bookRepository.GetBooksWithLimitAndPage(books, limit, page);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(booksLimitedAndPaged);            
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

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateBook([FromBody] BookDTO book, [FromQuery] int authorId, [FromQuery] int genreId)
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

            bookMap.AuthorId = authorId;
            bookMap.GenreId = genreId;
            


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

        [HttpPut("{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCategory(int bookId, [FromBody] BookDTO book, [FromQuery] int authorId, [FromQuery] int genreId)
        {
            if (book is null)
                return BadRequest(ModelState);

            if (bookId != book.Id)
                return BadRequest(ModelState);

            if (!await _bookRepository.BookExists(bookId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var bookMap = _mapper.Map<Book>(book);

            bookMap.AuthorId = authorId;
            bookMap.GenreId = genreId;

            if (!await _bookRepository.Update(bookMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating the book");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
