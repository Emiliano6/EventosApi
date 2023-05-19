namespace EventosApi.Controllers{
    using EventosApi.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
[Route("api/eventos")]
public class EventosController : ControllerBase{
    private readonly ApplicationDbContext context;
    public EventosController(ApplicationDbContext context){
        this.context = context;
    }
    [HttpGet]
    public async Task <ActionResult<List<Evento>>> Get(){
        return await context.Eventos.ToListAsync();
    }
    [HttpPost]
    public async Task<ActionResult<Evento>> Post(Evento evento){
        context.Add(evento);
        await context.SaveChangesAsync();
        return Ok(evento);
    }

}
}