using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CW_Tasks_app_NetCore.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
    }

    public enum TaskStatus
    {
        OPEN,
        IN_PROGRESS,
        DONE
    }

}
