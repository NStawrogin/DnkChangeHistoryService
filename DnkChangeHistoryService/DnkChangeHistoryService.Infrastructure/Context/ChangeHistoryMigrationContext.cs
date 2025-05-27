using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure.Context;

[ExcludeFromCodeCoverage]
public class ChangeHistoryMigrationContext(DbContextOptions<ChangeHistoryMigrationContext> options)
    : ChangeHistoryContext<ChangeHistoryMigrationContext>(options)
{

}

