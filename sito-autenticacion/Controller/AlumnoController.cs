using Microsoft.AspNetCore.Mvc;

namespace sito_autenticacion.Controller
{
    public class AlumnoController
    {
        [Route("api/[Controller]")]
        [ApiController]
        public class EquipoController : ControllerBase
        {
            private readonly IMongoRepository<Equipo> _equipoRepo;

            public EquipoController(IMongoRepository<Equipo> equipoRepo)
            {
                _equipoRepo = equipoRepo;
            }
            // GET all
            [HttpGet]

            public async Task<ActionResult<List<Equipo>>> GetAll()
            {
                var Equipos = await _equipoRepo.GetAllAsync();
                return Ok(Equipos);
            }

            // GET by ID
            [HttpGet("{id}")]
            public async Task<ActionResult> GetById(string id)
            {
                var Equipo = await _equipoRepo.GetByFilterAsync(u => u.Id == id);
                if (Equipo == null) return NotFound();
                return Ok(Equipo);
            }



            [HttpPost]
            public async Task<ActionResult> Create([FromBody] Equipo nuevaEquipo)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _equipoRepo.CreateAsync(nuevaEquipo);

                if (nuevaEquipo.Id == null) return StatusCode(500, "ups no se realizo la insersion ");
                return Ok(nuevaEquipo);
            }

            // PUT (Update)
            [HttpPut("{id}")]
            public async Task<ActionResult> Update(string id, [FromBody] Equipo nuevoEquipo)
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var EquipoActualizar = await GetById(id);
                if (EquipoActualizar == null)
                {
                    return NotFound($"no exite un registro con id {id}");
                }
                nuevoEquipo.Id = id;
                await _equipoRepo.UpdateAsync(id, nuevoEquipo);
                return NoContent();
            }

            // DELETE
            [HttpDelete("{id}")]
            public async Task<ActionResult> Delete(string id)
            {
                var Equipo = await GetById(id);
                if (Equipo == null) return NotFound();
                await _equipoRepo.DeleteAsync(id);
                return NoContent();
            }
        }
    }
}
