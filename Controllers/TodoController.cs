using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/v1/todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        /// <summary>
        /// Get all todos
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all todos")]
        public async Task<IActionResult> GetTodos()
        {
            var todos = await _todoRepository.GetAllTodosAsync(null);
            return Ok(todos);
        }

        /// <summary>
        /// Query all todos
        /// </summary>
        [HttpPost("query")]
        [SwaggerOperation(Summary = "Query all todos")]
        public async Task<IActionResult> QueryTodos([FromBody] Filter<Todo> filter)
        {
            var todos = await _todoRepository.GetAllTodosAsync(filter);
            return Ok(todos);
        }

        /// <summary>
        /// Get a specific todo by ID
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a todo by ID")]
        [SwaggerResponse(200, "Todo found", typeof(Todo))]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        /// <summary>
        /// Create a new todo
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new todo")]
        [SwaggerResponse(201, "Todo created", typeof(Todo))]
        public async Task<IActionResult> CreateTodo([FromBody] Todo todo)
        {
            await _todoRepository.AddTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
        }

        /// <summary>
        /// Update an existing todo
        /// </summary>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing todo")]
        [SwaggerResponse(204, "Todo updated")]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo todo)
        {
            if (id != todo.Id) return BadRequest();
            await _todoRepository.UpdateTodoAsync(todo);
            return NoContent();
        }

        /// <summary>
        /// Delete a todo
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a todo")]
        [SwaggerResponse(204, "Todo deleted")]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            await _todoRepository.DeleteTodoAsync(id);
            return NoContent();
        }
    }
}
