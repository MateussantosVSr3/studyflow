using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Data;

public class StudyFlowContext(DbContextOptions<StudyFlowContext> options) : DbContext(options)
{
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var tarefa = modelBuilder.Entity<Tarefa>();
        tarefa.ToTable("Tarefas");
        tarefa.HasKey(t => t.Id);
        tarefa.Property(t => t.Titulo).HasMaxLength(120).IsRequired();
        tarefa.Property(t => t.Disciplina).HasMaxLength(80).IsRequired();
        tarefa.Property(t => t.Descricao).HasMaxLength(1000);
        tarefa.HasIndex(t => t.Prazo);
    }
}
