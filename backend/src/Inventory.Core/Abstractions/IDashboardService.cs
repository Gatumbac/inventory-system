using Inventory.Core.Dtos;

namespace Inventory.Core.Abstractions;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync();
}
