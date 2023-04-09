using Grubhub.Models;
using Microsoft.EntityFrameworkCore;

namespace Grubhub.Data
{
    public class GrubhubDBContext:DbContext
    {
        //constutor
        public GrubhubDBContext(DbContextOptions<GrubhubDBContext> options) : base(options) { 

        }
        //create UsersData table in DB reference column from model User
        public DbSet<User> UsersData { get; set; }
    }
}
