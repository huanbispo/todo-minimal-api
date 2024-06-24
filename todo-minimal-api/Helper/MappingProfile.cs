using todo_minimal_api.Modals;
using todo_minimal_api.Modals.Dtos;
using AutoMapper;

namespace todo_minimal_api.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Todo, TodoDto>();
            CreateMap<TodoDto, Todo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Mapeamento para Category e CategoryDto
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Todos, opt => opt.MapFrom(src => src.Todos));
        }
    }
}
