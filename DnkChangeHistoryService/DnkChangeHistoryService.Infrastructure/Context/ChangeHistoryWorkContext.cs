using Microsoft.EntityFrameworkCore;

namespace DnkChangeHistoryService.Infrastructure.Context;

public class ChangeHistoryWorkContext(DbContextOptions<ChangeHistoryWorkContext> options)
    : ChangeHistoryContext<ChangeHistoryWorkContext>(options)
{

}