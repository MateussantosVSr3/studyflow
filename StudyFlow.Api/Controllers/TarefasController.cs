using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Dtos;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/tarefas")]
public class TarefasController(StudyFlowContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<TarefaResponse>>(200)]
    public async Task<ActionResult<List<TarefaResponse>>> Listar([FromQuery] bool? concluida, CancellationToken cancellationToken)
    {
        var query = context.Tarefas.AsNoTracking();
        if (concluida.HasValue) query = query.Where(t => t.Concluida == concluida.Value);
        var tarefas = await query.OrderBy(t => t.Prazo).ThenBy(t => t.Id).ToListAsync(cancellationToken);
        return Ok(tarefas.Select(Representar).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<TarefaResponse>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    public async Task<ActionResult<TarefaResponse>> Buscar(int id, CancellationToken cancellationToken)
    {
        var tarefa = await context.Tarefas.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
        return tarefa is null ? NaoEncontrada(id) : Ok(Representar(tarefa));
    }

    [HttpPost]
    [ProducesResponseType<TarefaResponse>(201)]
    [ProducesResponseType<ValidationProblemDetails>(400)]
    public async Task<ActionResult<TarefaResponse>> Criar(TarefaRequest request, CancellationToken cancellationToken)
    {
        var tarefa = new Tarefa { CriadaEm = DateTime.UtcNow };
        Aplicar(tarefa, request);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Buscar), new { id = tarefa.Id, version = "1" }, Representar(tarefa));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ValidationProblemDetails>(400)]
    [ProducesResponseType<ProblemDetails>(404)]
    public async Task<IActionResult> Atualizar(int id, TarefaRequest request, CancellationToken cancellationToken)
    {
        var tarefa = await context.Tarefas.FindAsync([id], cancellationToken);
        if (tarefa is null) return NaoEncontrada(id);
        Aplicar(tarefa, request);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var tarefa = await context.Tarefas.FindAsync([id], cancellationToken);
        if (tarefa is null) return NaoEncontrada(id);
        context.Tarefas.Remove(tarefa);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private ObjectResult NaoEncontrada(int id) => Problem(statusCode: 404,
        title: "Tarefa não encontrada", detail: $"Não existe tarefa com ID {id}.");
    private static void Aplicar(Tarefa tarefa, TarefaRequest request)
    {
        tarefa.Titulo = request.Titulo.Trim();
        tarefa.Disciplina = request.Disciplina.Trim();
        tarefa.Descricao = request.Descricao?.Trim();
        tarefa.Prazo = request.Prazo;
        tarefa.Concluida = request.Concluida;
        tarefa.AtualizadaEm = DateTime.UtcNow;
    }
    private static TarefaResponse Representar(Tarefa t) => new(t.Id, t.Titulo, t.Disciplina,
        t.Descricao, t.Prazo, t.Concluida, t.CriadaEm, t.AtualizadaEm);
}
