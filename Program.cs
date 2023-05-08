using AuthApi.Models;
using AuthApi.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDb>(o => o.UseSqlServer("Data Source=localhost;Initial Catalog=auth;Persist Security Info=True;User ID=sa;Password=abcd.1234; Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"));
builder.Services.AddCors();
var app = builder.Build();

app.UseCors((option) => option.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.MapGet("/users",async(UserDb db)=>
    await db.Users.ToListAsync());

app.MapGet("/users/{id}", async (int id, UserDb db) =>
    {
        var foundUser = await db.Users.FindAsync(id);
        if(foundUser is null){
            return Results.NotFound();
        }

        var response = new UserDTO(foundUser.Name, foundUser.Email);

        return Results.Ok(response);
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
    var password = userInput.Password;

    Console.WriteLine(email);
    Console.WriteLine(password);

    try
    {
        var userByEmail = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

        if(userByEmail is null)
        {
            return Results.BadRequest(new { status = false, msg = "Não há usuário cadastrado com esse e-mail"});
        }

        if(userByEmail.Password != password)
        {
            return Results.BadRequest(new {status = false, msg = "A senha está incorreta!" });
        }

        return Results.Ok(new {status = true, msg = "Usuário logado com sucesso!" });

    }catch(Exception e){
        Console.WriteLine(e.Message);
        return Results.NotFound();
    }

});


app.Run();
