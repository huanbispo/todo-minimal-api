using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using todo_minimal_api;
using todo_minimal_api.Modals;
using todo_minimal_api.Modals.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(options =>
{
    options.UseInMemoryDatabase("TodoDb");
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Mapear rotas da API
app.MapGet("/", () => "Hello Brasil!");

// Setup Category Routes

// Get All Categories
app.MapGet("/categories", async (TodoDb db) => await db.Categories.ToListAsync());

// Get Category by Id
app.MapGet("/categories/{id}", async (Guid id, TodoDb db) => await db.Categories.FindAsync(id) is Category category ? Results.Ok(category) : Results.NotFound());

// Create Category
app.MapPost("/categories", async (Category category, TodoDb db) =>
{
    db.Categories.Add(category);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
});

// Update Category
app.MapPut("/categories/{id}", async (Guid id, Category category, TodoDb db) =>
{
    if (id != category.Id)
    {
        return Results.BadRequest("There was a ploblem with the Id selected.");
    }

    db.Entry(category).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete Category
app.MapDelete("/categories/{id}", async (Guid id, TodoDb db) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null)
    {
        return Results.NotFound();
    }

    db.Categories.Remove(category);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// Setup Todo Routes

// Get All Todos
app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToListAsync());

// Get All todos with their respective categories
app.MapGet("/categoriesWithTodos", async (TodoDb db, IMapper mapper) =>
{
    var categories = await db.Categories.Include(c => c.Todos).ToListAsync();
    var categoriesDto = mapper.Map<List<CategoryDto>>(categories);
    return Results.Json(categoriesDto);
});

// Get Todo by Id
app.MapGet("/todos/{id}", async (Guid id, TodoDb db) => await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound());

// Create Todo *A todo must have a valid CategoryId
app.MapPost("/todos", async (TodoDto todoDto, TodoDb db, IMapper mapper) =>
{
    var todo = mapper.Map<Todo>(todoDto);
    todo.Id = Guid.NewGuid(); // Gerar um novo GUID para cada novo todo
    todo.CreatedAt = DateTime.Now;
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

// Update Todo
app.MapPut("/todos/{id}", async (Guid id, Todo todo, TodoDb db) =>
{
    if (id != todo.Id)
    {
        return Results.BadRequest("There was a ploblem with the Id selected.");
    }

    db.Entry(todo).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// update todo status
app.MapPut("/todos/{id}/status", async (Guid id, [FromBody] UpdateStatusDto statusDto, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Status = statusDto.Status;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete Todo
app.MapDelete("/todos/{id}", async (Guid id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Setup Step Routes
app.MapPost("/todos/{todoId}/steps", async (Guid todoId, Step step, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(todoId);

    if (todo is null) return Results.NotFound();

    step.TodoId = todoId;
    db.Steps.Add(step);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todoId}/steps/{step.Id}", step);
});

app.MapPut("/steps/{id}", async (Guid id, Step inputStep, TodoDb db) =>
{
    var step = await db.Steps.FindAsync(id);

    if (step is null) return Results.NotFound();

    step.Description = inputStep.Description;
    step.IsCompleted = inputStep.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/steps/{id}", async (Guid id, TodoDb db) =>
{
    var step = await db.Steps.FindAsync(id);

    if (step is null) return Results.NotFound();

    db.Steps.Remove(step);
    await db.SaveChangesAsync();
    return Results.Ok(step);
});

// Rotas para FileAttachment
app.MapGet("/todos/{todoId}/fileattachments", async (Guid todoId, TodoDb db) =>
{
    var todo = await db.Todos.Include(t => t.FileAttachments).FirstOrDefaultAsync(t => t.Id == todoId);

    if (todo == null) return Results.NotFound();

    var fileAttachments = todo.FileAttachments.Select(fa => new FileAttachmentDto
    {
        Id = fa.Id,
        FileName = fa.FileName,
        FilePath = fa.FilePath,
        TodoId = fa.TodoId
    }).ToList();

    return Results.Ok(fileAttachments);
});

app.MapGet("/todos/fileattachments/{attachmentId}", async (int attachmentId, TodoDb db) =>
{
    var attachment = await db.FileAttachments.FindAsync(attachmentId);

    if (attachment == null)
        return Results.NotFound();

    var filePath = attachment.FilePath;

    if (!System.IO.File.Exists(filePath))
    {
        return Results.NotFound("File not found.");
    }

    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
    return Results.File(fileBytes, "application/octet-stream", attachment.FileName);
});




app.MapPost("/todos/{todoId}/fileattachments", async (Guid todoId, HttpRequest request, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(todoId);

    if (todo is null) return Results.NotFound();

    var formCollection = await request.ReadFormAsync();
    var file = formCollection.Files.FirstOrDefault();

    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("Invalid file data.");
    }

    var uploadsFolderPath = Path.Combine("Uploads", todoId.ToString());
    Directory.CreateDirectory(uploadsFolderPath);

    var filePath = Path.Combine(uploadsFolderPath, file.FileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var fileAttachment = new FileAttachment
    {
        FileName = file.FileName,
        FilePath = filePath,
        TodoId = todoId
    };

    db.FileAttachments.Add(fileAttachment);
    await db.SaveChangesAsync();

    var fileAttachmentDto = new FileAttachmentDto
    {
        Id = fileAttachment.Id,
        FileName = fileAttachment.FileName,
        FilePath = fileAttachment.FilePath,
        TodoId = fileAttachment.TodoId
    };

    return Results.Created($"/todos/{todoId}/fileattachments/{fileAttachment.Id}", fileAttachmentDto);
});


app.MapDelete("/fileattachments/{id}", async (int id, TodoDb db) =>
{
    var fileAttachment = await db.FileAttachments.FindAsync(id);

    if (fileAttachment is null) return Results.NotFound();

    db.FileAttachments.Remove(fileAttachment);
    await db.SaveChangesAsync();
    return Results.Ok(fileAttachment);
});

app.Run();
