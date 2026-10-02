using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Api.Dtos;

public class TarefaRequest : IValidatableObject
{
    [Required, StringLength(120, MinimumLength = 3)]
    public string Titulo { get; set; } = string.Empty;
    [Required, StringLength(80)]
    public string Disciplina { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? Descricao { get; set; }
    public DateOnly Prazo { get; set; }
    public bool Concluida { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Trim().Length < 3)
            yield return new ValidationResult("O título deve conter pelo menos 3 caracteres úteis.", [nameof(Titulo)]);
        if (string.IsNullOrWhiteSpace(Disciplina))
            yield return new ValidationResult("Informe a disciplina.", [nameof(Disciplina)]);
        if (Prazo == default)
            yield return new ValidationResult("Informe o prazo no formato AAAA-MM-DD.", [nameof(Prazo)]);
    }
}

public record TarefaResponse(int Id, string Titulo, string Disciplina, string? Descricao,
    DateOnly Prazo, bool Concluida, DateTime CriadaEm, DateTime AtualizadaEm);
