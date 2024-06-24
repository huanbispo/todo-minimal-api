using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_minimal_api.Modals
{
    public class Step : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        [ForeignKey("Todo")]
        public Guid TodoId { get; set; }
        public Todo ToDo { get; set; }
    }
}
