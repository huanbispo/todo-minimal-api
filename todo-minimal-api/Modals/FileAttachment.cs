using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_minimal_api.Modals
{
    public class FileAttachment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        [ForeignKey("Todo")]
        public Guid TodoId { get; set; }
        public Todo ToDo { get; set; }
    }
}
