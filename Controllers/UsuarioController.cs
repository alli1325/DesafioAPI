using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using DesafioAPI.Data;
using DesafioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DesafioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public UsuarioController(ApplicationDbContext database){
            this.database = database;
        }

        [HttpPost("Registro")]
        public IActionResult Registro([FromBody] UsuarioTemp userTemp){

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var registro = database.Usuarios.ToList();
            
            //Verificando se credenciais já estão em uso
            foreach (var reg in registro){
                if (reg.User == userTemp.User){
                    Response.StatusCode = 400;
                    return new ObjectResult(new {message = "Nome de usuário já cadastrado"});
                }
                if (reg.Email == userTemp.Email){
                    Response.StatusCode = 400;
                    return new ObjectResult(new {message = "E-mail já cadastrado"});
                }
            }
            
            //Validações de campos para registro
            if(userTemp.User.Replace(" ", "").Length <= 3){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Este campo deve conter no mínimo 4 caracteres"});
            }

            if(!regex.IsMatch(userTemp.Email)){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Favor inserir um e-mail válido"});
            }

            if(userTemp.Senha.Replace(" ", "").Length <= 3){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Este campo deve conter no mínimo 4 caracteres"});
            }

            Usuario usuarios = new Usuario();
            usuarios.User = userTemp.User.Trim();
            usuarios.Email = userTemp.Email;
            //Encriptação de senha
            var senha = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userTemp.Senha.Trim()));
            usuarios.Senha = senha.ToString();
            usuarios.Role = "Usuario";

            database.Add(usuarios);
            database.SaveChanges();

            return Ok(new {message = "Usuário cadastrado com sucesso"});
        }   

        [HttpPost("Login")]

        public IActionResult Login ([FromBody] Login credenciais){
            try{
                Usuario usuario = database.Usuarios.First(user => user.User.Equals(credenciais.User));
                var senha = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(credenciais.Senha)).ToString();
                if(usuario.Senha.Equals(senha)){
                    
                    //Chave de segurança
                    string chaveDeSeguranca = "ch4v3_d3_s3gur4nc4_d0_d3s4f10_4p1";
                    var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                    var credencial = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Role, usuario.Role));

                    var JWT = new JwtSecurityToken(
                        issuer: "GFT_Starter_Allison",
                        expires: DateTime.Now.AddHours(1),
                        audience: "GFT_Users",
                        signingCredentials: credencial,
                        claims: claims    
                    );

                    //Enviando email com aviso de acesso
                    SendEmail.Send(usuario.Email, usuario.User);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    
                }else{
                    Response.StatusCode = 401;
                    return new ObjectResult(new {message = "E-mail ou senha inválidos"});
                }
            }catch{
                Response.StatusCode = 401;
                return new ObjectResult(new {message = "E-mail ou senha inválidos"});
            }

        }
    }
}