using CW_Tasks_app_NetCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW_Tasks_app_NetCore.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {

        }

        public DbSet<CW_Tasks_app_NetCore.Models.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
