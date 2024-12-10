using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string WeatherApiKey = "f545de35ef8f48c28d2100234240912";

        public ToDoController(ToDoDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

         [HttpPost]
        public async Task<IActionResult> AddToDoItem(ToDoItem toDoItem)
        {
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetToDoItemById), new { id = toDoItem.Id }, toDoItem);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDoItemById(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
                return NotFound();

            if (!string.IsNullOrEmpty(toDoItem.Location))
            {
                var weatherInfo = await GetWeatherInfo(toDoItem.Location);
                return Ok(new { toDoItem, weatherInfo });
            }

            return Ok(toDoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDoItem(int id, ToDoItem updatedItem)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
                return NotFound();

            toDoItem.Title = updatedItem.Title;
            toDoItem.Description = updatedItem.Description;
            toDoItem.Category = updatedItem.Category;
            toDoItem.Priority = updatedItem.Priority;
            toDoItem.Location = updatedItem.Location;
            toDoItem.DueDate = updatedItem.DueDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }


         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
                return NotFound();

            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchToDoItems(string? title, int? priority, DateTime? dueDate)
        {
            var query = _context.ToDoItems.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(item => item.Title.Contains(title));

            if (priority.HasValue)
                query = query.Where(item => item.Priority == priority);

            if (dueDate.HasValue)
                query = query.Where(item => item.DueDate == dueDate);

            return Ok(await query.ToListAsync());
        }

         private async Task<object> GetWeatherInfo(string location)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://api.weatherapi.com/v1/current.json?key={WeatherApiKey}&q={location}");

            if (response.IsSuccessStatusCode)
            {
                var weatherJson = await response.Content.ReadAsStringAsync();
                var weatherData = JsonDocument.Parse(weatherJson);
                var temp = weatherData.RootElement.GetProperty("current").GetProperty("temp_c").GetDecimal();
                var condition = weatherData.RootElement.GetProperty("current").GetProperty("condition").GetProperty("text").GetString();
                return new { Temperature = temp, Condition = condition };
            }

            return null;
        }



    }
}
