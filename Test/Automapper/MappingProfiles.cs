using AutoMapper;
using Test.DTO;
using Test.Models;

namespace Test.Automapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<AuthorDTO, Author>();
            CreateMap<Genre, TagDTO>();
            CreateMap<TagDTO, Genre>();
        }
    }
}
