using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SoftDelete
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        MySQLDbContext dbContext;

        public UserController(MySQLDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Create database
            dbContext.Database.EnsureCreated();

            return Ok(dbContext.Users.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = dbContext.Users.Find(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("recyclebin")]
        public IActionResult RecycleBin()
        {
            var users = dbContext.Users.IgnoreQueryFilters()
                .Where(user => EF.Property<bool>(user, "IsDeleted") == true);

            return Ok(users.ToList());
        }

        [HttpPost("recover/{id}")]
        public IActionResult Recover(int id)
        {
            var deletedUsers = dbContext.Users.IgnoreQueryFilters()
                .Where(user => user.UserId == id && EF.Property<bool>(user, "IsDeleted") == true);
            if (deletedUsers.Count() == 0)
                return NotFound();

            foreach (var delUser in deletedUsers)
            {
                var userEntry = dbContext.ChangeTracker.Entries<User>().First(user => user.Entity == delUser);
                userEntry.Property("IsDeleted").CurrentValue = false;
                userEntry.Property("DeleteTime").CurrentValue = null;
            }
            dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        public IActionResult Post([FromForm]User user)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            // Create database
            dbContext.Database.EnsureCreated();

            dbContext.Users.Add(user);
            try
            {
                dbContext.SaveChanges();
                return Created($"api/user/{user.UserId}", user);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm]User user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var updateUser = dbContext.Users.Find(id);
            if (updateUser == null)
                return NotFound();

            updateUser.Name = user.Name;
            dbContext.SaveChanges();
            return Created($"api/user/{user.UserId}", updateUser);

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = dbContext.Users.Find(id);
            if (user == null)
                return NotFound();

            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
            return NoContent();
        }

    }
}