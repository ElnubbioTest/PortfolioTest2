namespace ToDoList;

public class ToDoItem{
    public string? Title {get; set;}
    public bool IsDone {get; set;}
    public ToDoItem(string? title, bool isDone) {
        Title = title;
        IsDone = isDone;
    }
    public ToDoItem(string? title) {
        Title = title;
        IsDone = false;
    }

    public ToDoItem(){
        Title = "BLANK!";
        IsDone = false;
    }

}