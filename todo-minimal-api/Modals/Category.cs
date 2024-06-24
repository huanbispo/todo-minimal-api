using System.ComponentModel.DataAnnotations;

namespace todo_minimal_api.Modals
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<Todo> Todos { get; set; }
    }
}
