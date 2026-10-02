namespace StudyFlow.Api.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateOnly Prazo { get; set; }
    public bool Concluida { get; set; }
    public DateTime CriadaEm { get; set; }
    public DateTime AtualizadaEm { get; set; }
}
