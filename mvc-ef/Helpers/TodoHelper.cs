using Microsoft.EntityFrameworkCore;
using mvc_ef.Data;
using mvc_ef.DTOs.Todo;
using mvc_ef.Helpers.interfaces;
using mvc_ef.Models;

namespace mvc_ef.Helpers
{
    public class TodoHelper : ITodoHelper
    {
        private readonly MyAppContext _context;

        public TodoHelper(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetAllByUser(int userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TodoItem?> GetById(int id, int userId)
        {
            return await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<bool> Create(TodoDto dto, int userId)
        {
            var todo = new TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = userId
            };

            _context.TodoItems.Add(todo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(int id, TodoDto dto, int userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            todo.Title = dto.Title;
            todo.Description = dto.Description;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleComplete(int id, int userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            todo.IsCompleted = !todo.IsCompleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            _context.TodoItems.Remove(todo);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
