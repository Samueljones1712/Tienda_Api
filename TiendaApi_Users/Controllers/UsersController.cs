using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TiendaApi_Users.Data;
using TiendaApi_Users.Models;
using TiendaApi_Users.Services;

namespace TiendaApi_Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador, Empleado")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;
        private readonly Encrypt _encrypt;

        public UsersController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _encrypt = new Encrypt();
            _configuration = configuration;

        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound(); // No se encontró el usuario a actualizar
            }

            existingUser.email = updatedUser.email;
            existingUser.fullName = updatedUser.fullName;

            if (_encrypt.VerifyPassword(updatedUser.password, existingUser.password) == false)//Si la contrasena cambio
            {
                existingUser.password = _encrypt.EncryptPassword(updatedUser.password);
            }

            // Marcar la entidad como modificada para que los cambios se reflejen en la base de datos
            _context.Entry(existingUser).State = EntityState.Modified;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(existingUser); // Retorna el usuario actualizado
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Users>> PostUsers(Users user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'DataContext.Users'  is null.");
          }

            user.password = _encrypt.EncryptPassword(user.password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = user.idUsers }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersExists(int id)
        {
            return (_context.Users?.Any(e => e.idUsers == id)).GetValueOrDefault();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/Login")]
        public async Task<ActionResult<Users>> login(string username, string password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(e => e.username == username);

            if (user == null)
            {
                return NotFound();
            }

            if (_encrypt.VerifyPassword(password,user.password))
            {
                //    var tokenString = Generate(user);

                var authClaims = new List<Claim>
            {
                new Claim("idUser", user.idUsers.ToString()),
                new Claim(ClaimTypes.Role, user.rol),
            };

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            else
            {
                return Problem("Contraseña Incorrecta");
            }
        }


      

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
