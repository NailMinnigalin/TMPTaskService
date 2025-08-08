using System.ComponentModel.DataAnnotations;

namespace TMPTaskService.Data.Models
{
	public class Task
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public required string Name { get; set; }

		public string? Description { get; set; }
	}
}
