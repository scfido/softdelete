using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SoftDelete
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        [HttpGet]
        public IEnumerable<User> Get()
        {
            using (var context = new MySQLDbContext())
            {
                // Create database
                context.Database.EnsureCreated();

                return context.Users.ToList();
            }
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            using (var context = new MySQLDbContext())
            {
                var user = context.Users.Find(id);
                if (user == null)
                {
                    this.Response.StatusCode = 404;
                    return null;
                }
                else
                {
                    return user;
                }
            }
        }

        [HttpGet("recyclebin")]
        public IEnumerable<User> RecycleBin()
        {
            using (var context = new MySQLDbContext())
            {
                var users = context.Users.IgnoreQueryFilters()
                    .Where(user => EF.Property<bool>(user, "IsDeleted") == true);

                return users.ToList();
            }
        }

        [HttpPost("recover/{id}")]
        public void Recover(int id)
        {
            using (var context = new MySQLDbContext())
            {
                var deletedUsers = context.Users.IgnoreQueryFilters()
                    .Where(user => user.UserId == id && EF.Property<bool>(user, "IsDeleted") == true);
                if (deletedUsers.Count() == 0)
                {
                    this.Response.StatusCode = 404;
                }
                else
                {
                    foreach (var delUser in deletedUsers)
                    {
                        var userEntry = context.ChangeTracker.Entries<User>().First(user => user.Entity == delUser);
                        userEntry.Property("IsDeleted").CurrentValue = false;
                        userEntry.Property("DeleteTime").CurrentValue = null;
                    }
                    context.SaveChanges();
                }
            }
        }

        [HttpPost]
        public void Post([FromForm]User user)
        {
            if (user == null || user.Name == null)
            {
                Response.StatusCode = 400;
                return;
            }

            using (var context = new MySQLDbContext())
            {
                // Create database
                context.Database.EnsureCreated();

                context.Users.Add(user);
                try
                {
                    context.SaveChanges();
                }
                catch (System.Exception ex)
                {

                    throw ex;
                }
            }

        }

        [HttpPut("{id}")]
        public void Put(int id, string name)
        {
            using (var context = new MySQLDbContext())
            {
                var user = context.Users.Find(id);
                if (user == null)
                    this.Response.StatusCode = 404;
                else
                {
                    user.Name = name;
                    context.SaveChanges();
                }
            }

        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (var context = new MySQLDbContext())
            {
                var user = context.Users.Find(id);
                if (user == null)
                    this.Response.StatusCode = 404;
                else
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                }
            }
        }

    }
}