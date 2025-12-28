using AutoMapper;

namespace Application.Common.Models
{
    public class LookUpDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public LookUpDto(int id, string title)
        {
            Id = id;
            Title = title;
        }
        public LookUpDto()
        {
        }

    }

    // mapping
    public class MappingProfile : Profile
    {

    }
}
