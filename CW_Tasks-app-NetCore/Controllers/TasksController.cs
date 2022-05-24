using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW_Tasks_app_NetCore.Data;
using CW_Tasks_app_NetCore.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW_Tasks_app_NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskDbContext _context;

        public TasksController(TaskDbContext context)
        {
            _context = context;
        }

        // GET api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if(task == null)
            {
                return BadRequest("Task not found.");
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody] CreateTaskDto taskDto)
        {
            var taskContext = _context.Tasks;
            var task = new CW_Tasks_app_NetCore.Models.Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = CW_Tasks_app_NetCore.Models.TaskStatus.OPEN
            };

            await taskContext.AddAsync(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] CW_Tasks_app_NetCore.Models.Task task)
        {
            //_context.Entry(task).State = EntityState.Modified;   Just this works too.
            
            var taskTobeUpdated = await _context.Tasks.FindAsync(task.Id);
            taskTobeUpdated.Title = task.Title;
            taskTobeUpdated.Description = task.Description;
            taskTobeUpdated.Status = task.Status;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskToBeDeleted = await _context.Tasks.FindAsync(id);
            if (taskToBeDeleted == null)
            {
                return NotFound();
            }
            _context.Tasks.Remove(taskToBeDeleted);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
