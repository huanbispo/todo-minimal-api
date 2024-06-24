using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_minimal_api.Modals
{
    public class Todo : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TodoStatus Status { get; set; }
        public ICollection<Step> Steps { get; set; }
        public ICollection<FileAttachment> FileAttachments { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public enum TodoStatus
    {
        Todo,
        InProgress,
        Completed
    }
}
