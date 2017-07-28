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
        public IActionResult Get()
        {

            using (var context = new MySQLDbContext())
            {
                // Create database
                context.Database.EnsureCreated();

                return Ok(context.Users.ToList());
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (var context = new MySQLDbContext())
            {
                var user = context.Users.Find(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
        }

        [HttpGet("recyclebin")]
        public IActionResult RecycleBin()
        {
            using (var context = new MySQLDbContext())
            {
                var users = context.Users.IgnoreQueryFilters()
                    .Where(user => EF.Property<bool>(user, "IsDeleted") == true);

                return Ok(users.ToList());
            }
        }

        [HttpPost("recover/{id}")]
        public IActionResult Recover(int id)
        {
            using (var context = new MySQLDbContext())
            {
                var deletedUsers = context.Users.IgnoreQueryFilters()
                    .Where(user => user.UserId == id && EF.Property<bool>(user, "IsDeleted") == true);
                if (deletedUsers.Count() == 0)
                    return NotFound();

                foreach (var delUser in deletedUsers)
                {
                    var userEntry = context.ChangeTracker.Entries<User>().First(user => user.Entity == delUser);
                    userEntry.Property("IsDeleted").CurrentValue = false;
                    userEntry.Property("DeleteTime").CurrentValue = null;
                }
                context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPost]
        public IActionResult Post([FromForm]User user)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            using (var context = new MySQLDbContext())
            {
                // Create database
                context.Database.EnsureCreated();

                context.Users.Add(user);
                try
                {
                    context.SaveChanges();
                    return Created($"api/user/{user.UserId}", user);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm]User user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            using (var context = new MySQLDbContext())
            {
                var updateUser = context.Users.Find(id);
                if (updateUser == null)
                    return NotFound();

                updateUser.Name = user.Name;
                context.SaveChanges();
                return Created($"api/user/{user.UserId}", updateUser);
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var context = new MySQLDbContext())
            {
                var user = context.Users.Find(id);
                if (user == null)
                    return NotFound();

                context.Users.Remove(user);
                context.SaveChanges();
                return NoContent();
            }
        }

    }
}