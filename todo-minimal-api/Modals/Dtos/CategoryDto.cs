namespace todo_minimal_api.Modals.Dtos
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<TodoDto> Todos { get; set; }
    }
}
