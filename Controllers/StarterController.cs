using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DesafioAPI.Data;
using DesafioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StarterController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;
        public static IWebHostEnvironment _webHostEnviroment;

        public StarterController(ApplicationDbContext database, IWebHostEnvironment webHostEnviroment){
            this.database = database;
            _webHostEnviroment = webHostEnviroment;
            HATEOAS = new HATEOAS.HATEOAS($"localhost:5001/api/Starter");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("DELETE_CATEGORIA", "DELETE");
        }

        [HttpGet]
        public IActionResult Get(){
            var starters = database.Starters.Include(f => f.Categoria).ToList();
            List<StarterContainer> startersHATEOAS = new List<StarterContainer>();
            foreach(var sts in starters){
                StarterContainer starterHATEOAS = new StarterContainer();
                starterHATEOAS.starter = sts;
                starterHATEOAS.links = HATEOAS.GetActions(sts.Id.ToString());

                startersHATEOAS.Add(starterHATEOAS);
            }

            return Ok(startersHATEOAS);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){
            try{
                var starter = database.Starters.Include(f => f.Categoria).First(c => c.Id == id);
                StarterContainer starterHATEOAS = new StarterContainer();
                starterHATEOAS.starter = starter;
                starterHATEOAS.links = HATEOAS.GetActions(starter.Id.ToString());
                
                return Ok(starterHATEOAS);

            }catch{ 
                Response.StatusCode = 404;
                return new ObjectResult(new {message = "Id Inválido"});
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] StarterPost startPost){
            // Validações
            //Nome
            if(startPost.Nome.Replace(" ", "").Length <= 3){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Nome muito curto, deve conter no mínimo 4 caracteres"});
            }

            //CPF
            //Regex only numbers
            Regex regex = new Regex("[0-9]");
            if (startPost.Cpf.Length != 11){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Cpf deve conter 11 digitos"});
            }

            if (!regex.IsMatch(startPost.Cpf)){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Cpf deve conter somente números"});
            }

            //Letras
            //Regex only letters
            regex = new Regex( @"^[a-zA-Z]+$");

             if (!regex.IsMatch(startPost.Letra)){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Este campo deve conter somente letras"});
            }

            if (startPost.Letra.Length != 4){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Letras devem conter 4 caracteres"});
            }
            
            //Email
            //Regex email
            regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (!regex.IsMatch(startPost.Email)){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Digite um e-mail válido"});
            }

            //CategoriaId
            var listaCat = database.Categorias.ToList();
            var validador = false;

            if (listaCat == null){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Favor cadastrar uma Categoria antes de cadastrar um Starter"});
            }

            foreach(var cat in listaCat){
                if(cat.Id == startPost.CategoriaId){
                    validador = true;
                }
            }

            if(validador == false){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Id de categoria inválido!"});
            }

            

            Starter start = new Starter();
            start.Nome = startPost.Nome.Trim();
            start.Cpf = startPost.Cpf;
            start.Letra = startPost.Letra;
            start.Email = startPost.Email;
            start.Foto = "padrao.png";
            
            start.Categoria = database.Categorias.First(c => c.Id == startPost.CategoriaId);

            database.Starters.Add(start);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(new {message = "Starter criado com sucesso"});
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id){
            try{
                var starter = database.Starters.First(c => c.Id == id);
                database.Starters.Remove(starter);
                database.SaveChanges();
                return Ok(new {message = "Starter deletado com sucesso"});

            }catch{ 
                Response.StatusCode = 404;
                return new ObjectResult(new {message = "Id Inválido"});
            }
        }

        [Route("uploadImage")]
        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<string> UploadImage(int id, [FromForm] FileUpload fileUpload){
            try{
                var starter = database.Starters.First(s => s.Id == id);

                if (fileUpload.Foto.Length < 0){
                    Response.StatusCode = 400;
                    return ("Upload de imagem inválido");
                }

                string path = _webHostEnviroment.WebRootPath + "\\uploads\\";
                
                if(starter.Foto != "padrao.png"){
                    System.IO.File.Delete(path+starter.Foto);
                }

                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                await using (FileStream fileStream = System.IO.File.Create(path+fileUpload.Foto.FileName)){
                    fileUpload.Foto.CopyTo(fileStream);
                    fileStream.Flush();
                }

                starter.Foto = fileUpload.Foto.FileName;
                database.SaveChanges();

                return ("Upload realizado com sucesso.");

            }catch{
                return ("Id Inválido");
            }
        }

        [Route("getImage/{id}")]
        [HttpGet]
        public IActionResult GetImage(int id){
            try{
                var starter = database.Starters.First(c => c.Id == id);
                string path = _webHostEnviroment.WebRootPath + "\\uploads\\";
                    if(System.IO.File.Exists(path+starter.Foto)){
                        byte[] b = System.IO.File.ReadAllBytes(path+starter.Foto);
                        return File(b, "image/png");
                    }

                    return Ok();
            }catch{
                Response.StatusCode = 404;
                return new ObjectResult(new {message = "Id Inválido"});
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Put([FromBody] StarterTemp start){
            if(start.Id <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Id inválido"});
            }

            try{
                var s = database.Starters.First(s => s.Id == start.Id);
                var c = database.Categorias.ToList();
                Regex regex = new Regex("^[0-9]+$");

                //Edição
                s.Nome = start.Nome.Replace(" ", "") != null ? start.Nome.Trim() : s.Nome;
                s.Cpf = regex.IsMatch(start.Cpf) && start.Cpf.Length == 11 ? start.Cpf : s.Cpf;

                regex = new Regex( @"^[a-zA-Z]+$");

                s.Letra = regex.IsMatch(start.Letra) && start.Letra.Length == 4 ? start.Letra : s.Letra;

                regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

                s.Email = regex.IsMatch(start.Email) ? start.Email : s.Email;

                if(start.CategoriaId <= 0 || start.CategoriaId > c.Count){
                    s.Categoria.Id = s.Categoria.Id;
                    s.Categoria.Tecnologia = s.Categoria.Tecnologia;
                    s.Categoria.Nome = s.Categoria.Nome;
                }else{
                    s.Categoria = database.Categorias.First(f => f.Id == start.CategoriaId);
                }

                database.SaveChanges();
                return Ok(new {message = "Starter editado com sucesso"});

            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Starter não encontrado"});
            }
        }

        [Route("asc")]
        [HttpGet]
        public IActionResult Asc(){
            var starters = database.Starters.Include(c => c.Categoria).ToList().OrderBy(s => s.Nome);
            List<StarterContainer> startersHATEOAS = new List<StarterContainer>();
            foreach(var sts in starters){
                StarterContainer starterHATEOAS = new StarterContainer();
                starterHATEOAS.starter = sts;
                starterHATEOAS.links = HATEOAS.GetActions(sts.Id.ToString());

                startersHATEOAS.Add(starterHATEOAS);
            }

            return Ok(startersHATEOAS);
            
        }

        [Route("desc")]
        [HttpGet]
        public IActionResult Desc(){
            var starters = database.Starters.Include(c => c.Categoria).ToList().OrderByDescending(s => s.Nome);
            List<StarterContainer> startersHATEOAS = new List<StarterContainer>();
            foreach(var sts in starters){
                StarterContainer starterHATEOAS = new StarterContainer();
                starterHATEOAS.starter = sts;
                starterHATEOAS.links = HATEOAS.GetActions(sts.Id.ToString());

                startersHATEOAS.Add(starterHATEOAS);
            }

            return Ok(startersHATEOAS);
        }

        [Route("nome/{nome}")]
        [HttpGet]
        public IActionResult Nome(string nome){
            try{
                var starter = database.Starters.Include(c => c.Categoria).First(c => c.Nome == nome.Trim());
                StarterContainer starterHATEOAS = new StarterContainer();
                starterHATEOAS.starter = starter;
                starterHATEOAS.links = HATEOAS.GetActions(starter.Id.ToString());

                return Ok(starterHATEOAS);


            }catch{ 
                Response.StatusCode = 404;
                return new ObjectResult(new {message = "Nome Inválido"});
            }
        }
    }
}