using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Controllers;
using ToDoApp.Models;

namespace ToDoApi.Tests
{
    public class UnitTest
    {
         [Fact]
        public void CanAddToDoItem()
        {
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: "ToDoDb")
                .EnableSensitiveDataLogging()
                .Options;
        
            using (var context = new ToDoDbContext(options))
            {
                var controller = new ToDoController(context, null);
                var newToDo = new ToDoItem
                {
                    Title = "Test ToDo",
                    Priority = 1,
                    Description = "Test Description",
                    Category = "Test Category",
                    Location = "0,0" 
                };
        
                var result = controller.AddToDoItem(newToDo).Result as CreatedAtActionResult;
        
                Assert.NotNull(result);
                Assert.Equal(newToDo.Title, ((ToDoItem)result.Value).Title);
            }
        }

    }
}