using Microsoft.EntityFrameworkCore;
using AuthApi.Models;
namespace AuthApi.Persistence;

public class UserDb : DbContext{
    public UserDb(DbContextOptions<UserDb> options): base(options){}
    public DbSet<User> Users => Set<User>();
}