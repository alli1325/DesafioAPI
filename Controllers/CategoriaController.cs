using System.Collections.Generic;
using System.Linq;
using DesafioAPI.Data;
using DesafioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {

        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public CategoriaController(ApplicationDbContext database){
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS($"localhost:5001/api/Categoria");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("DELETE_CATEGORIA", "DELETE");
        }

        [HttpGet]
        public IActionResult Get(){

            var categorias = database.Categorias.ToList();
            List<CategoriaContainer> categoriasHATEOAS = new List<CategoriaContainer>();
            foreach(var cat in categorias){
                CategoriaContainer categoriaHATEOAS = new CategoriaContainer();
                categoriaHATEOAS.categoria = cat;
                categoriaHATEOAS.links = HATEOAS.GetActions(cat.Id.ToString());

                categoriasHATEOAS.Add(categoriaHATEOAS);
            }

            return Ok(categoriasHATEOAS);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){
            try{
                var categoria = database.Categorias.First(c => c.Id == id);
                CategoriaContainer categoriaHATEOAS = new CategoriaContainer();
                categoriaHATEOAS.categoria = categoria;

                categoriaHATEOAS.links = HATEOAS.GetActions(categoria.Id.ToString());

                return Ok(categoriaHATEOAS);

            }catch{ 
                Response.StatusCode = 404;
                return new ObjectResult(new {message = "Id Inválido"});
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] CategoriaTemp cTemp){

            if(cTemp.Nome.Replace(" ", "").Length <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Campo tecnologia não está preenchido"});
            }

            if(cTemp.Nome.Replace(" ", "").Length <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Campo nome não está preenchido"});
            }

            Categoria cat = new Categoria();
            cat.Tecnologia = cTemp.Tecnologia;
            cat.Nome = cTemp.Nome;

            database.Categorias.Add(cat);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(new {message = "Categoria criada com sucesso"});
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id){
            try{
                var categoria = database.Categorias.First(c => c.Id == id);
                database.Categorias.Remove(categoria);
                database.SaveChanges();
                return Ok(new {message = "Categoria deletada com sucesso"});

            }catch{ 
                Response.StatusCode = 404;
                return new ObjectResult(new {message = "Id Inválido"});
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Put([FromBody] Categoria cat){
            if(cat.Id <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Id inválido"});
            }

            try{
                var c = database.Categorias.First(c => c.Id == cat.Id);

                //Edição
                c.Tecnologia = cat.Tecnologia.Replace(" ", "") != null ? cat.Tecnologia.Trim() : c.Tecnologia;
                c.Nome = cat.Nome.Replace(" ", "") != null ? cat.Nome.Trim() : c.Nome;

                database.SaveChanges();
                return Ok(new {message = "Alterado com sucesso"});

            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new {message = "Categoria não encontrada"});
            }
        }
    }
}