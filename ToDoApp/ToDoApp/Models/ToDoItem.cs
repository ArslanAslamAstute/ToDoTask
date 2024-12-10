namespace ToDoApp.Models
{
    public class ToDoItem
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }   
        public string? Location { get; set; }   
        public int Priority { get; set; } = 3;
        public DateTime? DueDate { get; set; }


    }
}
