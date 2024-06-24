namespace todo_minimal_api.Modals.Dtos
{
    public class TodoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TodoStatus Status { get; set; }
        public string CategoryId { get; set; }
    }

    public class UpdateStatusDto
    {
        public TodoStatus Status { get; set; }
    }
}
