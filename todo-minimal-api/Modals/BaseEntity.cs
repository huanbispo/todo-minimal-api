using System.ComponentModel.DataAnnotations;

namespace todo_minimal_api.Modals
{
    public class BaseEntity
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
