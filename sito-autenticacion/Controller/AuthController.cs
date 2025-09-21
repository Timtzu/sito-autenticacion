using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sito_autenticacion.Model;
using sito_autenticacion.Services;
using System.Security.Claims;
using System.Text;
using sito_autenticacion.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace sito_autenticacion.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "servicios")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;
        public AuthController(AppDbContext context, IConfiguration configuration, PasswordHasher passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegistroDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            var usuario = new Usuario
            {
                Username = dto.Username,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Role = dto.Role // Set role during registration
            };

            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario Registrado Exitosamente.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Usuario Invalida o contraseña");
            Console.WriteLine(user.Role);
            var token = GenerateJwtToken(user);

            // Set the auth_token cookie
            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(2)
            });

            return Ok(new { token });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario updatedUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Modelo no válido: {ModelState}");
            }

            var existingUsuario = await _context.Users.FindAsync(id);
            if (existingUsuario == null)
            {
                return NotFound($"No se encontró el producto con ID {id}");
            }

            // Actualizar los campos del producto existente
            existingUsuario.Username = updatedUsuario.Username;
            existingUsuario.PasswordHash = _passwordHasher.HashPassword(updatedUsuario.PasswordHash);
            existingUsuario.Role = updatedUsuario.Role;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el producto: {ex.Message}");
            }

            return Ok(existingUsuario);
        }

        private string GenerateJwtToken(Usuario user)
        {
            Console.WriteLine("Role: " + user.Role);
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.WriteToken(token);
            if (string.IsNullOrEmpty(jwt) || !jwt.Contains("."))
            {
                throw new Exception("Failed to generate a valid JWT token.");
            }
            return jwt;
        }

        [HttpGet("me")]
        [AllowAnonymous]
        public ActionResult<UserInfo> Me()
        {
            var userInfo = new UserInfo
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Name = User.Identity?.Name
            };

            if (userInfo.IsAuthenticated)
            {
                userInfo.Claims = User.Claims
                    .Select(c => new ClaimDto { Type = c.Type, Value = c.Value })
                    .ToList();
            }

            return Ok(userInfo);
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _context.Users.ToListAsync();
            if (usuarios == null)
            {
                return NotFound("No Existen usuarios");
            }
            return Ok(usuarios);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (id <= 0)
            {
                return BadRequest("ID usuario Invalido");
            }
            return Ok(usuario);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
            {
                return NotFound($"No se encontró el usuario con ID {id}");
            }

            try
            {

                // Eliminado Usuario
                _context.Users.Remove(usuario);

                // Guardar cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el pedido y sus productos relacionados: {ex.Message}");
            }

            return NoContent();
        }
        


    }
}
