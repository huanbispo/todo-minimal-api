namespace todo_minimal_api.Modals.Dtos
{
    public class FileAttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid TodoId { get; set; }
    }
}
