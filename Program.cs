using AuthApi.Models;
using AuthApi.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDb>(o => o.UseInMemoryDatabase("Users"));
var app = builder.Build();

app.MapGet("/users",async(UserDb db)=>
    await db.Users.ToListAsync());

app.MapGet("/users/{id}", async (int id, UserDb db) =>
    {
        var foundUser = await db.Users.FindAsync(id);
        if(foundUser is null){
            return Results.NotFound();
        }

        return Results.Ok(foundUser);
    });

app.MapPost("/users/signup", async(User user, UserDb db) =>{
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.UserId}", user);
});

app.MapPut("/users/{id}", async (int id, User userInput, UserDb db)=>{
    var user = await db.Users.FindAsync(id);
    if(user is null) {return Results.NotFound();}

    user.Name = userInput.Name;
    user.Email = userInput.Email;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/users/{id}", async(int id, UserDb db) => {
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(user);
    }

    return Results.NotFound();
});

app.MapPost("/users/auth", async(User userInput, UserDb db) =>{
    var email = userInput.Email;
    var pass = userInput.Password;
    try{
        var userByEmail = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

        if(userByEmail is null)
        {
            return Results.BadRequest("Não há usuário cadastrado com esse E-mail!");
        }

        if(userByEmail.Password != pass)
        {
            return Results.BadRequest("A senha está incorreta!");
        }

        return Results.Ok("Usuário logado com sucesso!");

    }catch(Exception e){
        Console.WriteLine(e.Message);
        return Results.NotFound();
    }

});


app.Run();
