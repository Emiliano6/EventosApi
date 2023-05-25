using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventosApi.DTOs;

namespace WebApiAlumnosSeg.Controllers
{
    [ApiController]
    [Route("cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private object editaraccesoDTO;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<Token>> Registrar(CredencialesUsuario credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var result = await userManager.CreateAsync(usuario, credenciales.Contraseña);

            if (result.Succeeded)
            {
                //Se retorna el Jwt (Json Web Token) especifica el formato del token que hay que devolverle a los clientes
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Token>> Login(CredencialesUsuario credencialesUsuario)
        {
            var result = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Contraseña, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }

        }

        [HttpGet("RenovarToken")]
         [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
         public async Task<ActionResult<Token>> Renovar()
         {
             var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
             var email = emailClaim.Value;

             var credenciales = new CredencialesUsuario()
             {
                 Email = email
             };

             return await ConstruirToken(credenciales);

         }
        
         private async Task<Token> ConstruirToken(CredencialesUsuario credencialesUsuario)
        { 

             var claims = new List<Claim>
             {
                 new Claim("email", credencialesUsuario.Email),
             };

             var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
             var claimsDB = await userManager.GetClaimsAsync(usuario);

             claims.AddRange(claimsDB);

             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
             var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

             var expiration = DateTime.UtcNow.AddMinutes(30);

             var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                 expires: expiration, signingCredentials: creds);

             return new Token()
             {
                 NewToken = new JwtSecurityTokenHandler().WriteToken(securityToken),
                 Expiracion = expiration
             };
         }
        
         [HttpPost("HacerAdmin")]
         public async Task<ActionResult> HacerAdmin(EditarAccesoDTO editaraccesoDTO)
         {
             var usuario = await userManager.FindByEmailAsync(editaraccesoDTO.Email);

             await userManager.AddClaimAsync(usuario, new Claim("EsAdmin", "1"));

             return NoContent();
         }

         [HttpPost("RemoverAdmin")]
         public async Task<ActionResult> RemoverAdmin(EditarAccesoDTO editaraccesoDTO)
         {
             var usuario = await userManager.FindByEmailAsync(editaraccesoDTO.Email);

             await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "1"));

             return NoContent();
         }

        [HttpPost("HacerOrganizador")]
        public async Task<ActionResult> HacerOrganizador(EditarAccesoDTO editaraccesoDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editaraccesoDTO.Email);

            await userManager.AddClaimAsync(usuario, new Claim("Esorganizador", "1"));

            return NoContent();
        }

        [HttpPost("RemoverOrganizador")]
        public async Task<ActionResult> RemoverOrganizador(EditarAccesoDTO editaraccesoDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editaraccesoDTO.Email);

            await userManager.RemoveClaimAsync(usuario, new Claim("EsOrganizador", "1"));

            return NoContent();
        }
    }
}