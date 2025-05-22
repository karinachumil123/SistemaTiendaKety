using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TiendaKeytlin.Server.Models;
using TiendaKeytlin.Server.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaKeytlin.Server.DTOs;

namespace TiendaKeytlin.Server.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService; 

        public AuthService(AppDbContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public string Authenticate(LoginDTO loginDTO)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Correo == loginDTO.Correo && u.Contrasena == loginDTO.Contrasena);

            if (usuario == null)
            {
                return null;
            }

            var rol = _context.Roles.Find(usuario.RolId);
            if (rol == null)
            {
                throw new InvalidOperationException("Rol no encontrado para el usuario");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Correo),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, rol.Nombre)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // NUEVO MÉTODO PARA RECUPERACIÓN DE CONTRASEÑA
        public async Task<bool> EnviarCodigoRecuperacionAsync(string correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                return false;
            }

            var codigo = new Random().Next(100000, 999999).ToString();
            usuario.CodigoRecuperacion = codigo;
            usuario.FechaExpiracionCodigo = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            var emailModel = new EmailModel
            {
                Email = correo,
                Subject = "Código de recuperación de contraseña",
                Message = $"Tu código de recuperación es: {codigo}. Este código expirará en 15 minutos."
            };

            await _emailService.SendEmailAsync(emailModel);
            return true;
        }

        public async Task<bool> VerificarCodigoRecuperacionAsync(string correo, string codigo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null || usuario.CodigoRecuperacion != codigo || usuario.FechaExpiracionCodigo < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == model.Correo && u.CodigoRecuperacion == model.VerificationCode);

            if (usuario == null)
                return false;

            // Aquí puedes aplicar hashing si lo usas
            usuario.Contrasena = model.NewContrasena;
            usuario.CodigoRecuperacion = null; // Borra el código

            await _context.SaveChangesAsync();
            return true;
        }

    }
}