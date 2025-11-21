using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookingService.Business.Services.External;

public class PermissionService : IPermissionService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PermissionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> CanViewVehicleCalendarAsync(int userId, int vehicleId, string accessToken)
    {
        // Bước 1: Gọi VehicleService để lấy GroupId của xe
        // Giả định VehicleService chạy ở localhost:5010 (Cấu hình cứng hoặc lấy từ AppSettings)
        var vehicleClient = _httpClientFactory.CreateClient();
        vehicleClient.BaseAddress = new Uri("http://localhost:5010");
        vehicleClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var vehicleRes =
            await vehicleClient.GetFromJsonAsync<ApiResultExternal<VehicleDtoExternal>>($"api/Vehicles/{vehicleId}");

        if (vehicleRes == null || !vehicleRes.Success || vehicleRes.Data == null)
            // Không tìm thấy xe hoặc lỗi
            return false;

        var groupId = vehicleRes.Data.CoOwnerGroupId;

        // Bước 2: Gọi ContractGroupService để lấy danh sách nhóm của User
        // Giả định ContractGroupService chạy ở localhost:5196
        var groupClient = _httpClientFactory.CreateClient();
        groupClient.BaseAddress = new Uri("http://localhost:5196");
        groupClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var groupRes = await groupClient.GetFromJsonAsync<ApiResultExternal<List<GroupDtoExternal>>>("api/Groups/my");

        if (groupRes == null || !groupRes.Success || groupRes.Data == null) return false;

        // Bước 3: Kiểm tra xem GroupId của xe có nằm trong danh sách nhóm của user không
        return groupRes.Data.Any(g => g.CoOwnerGroupId == groupId);
    }

    public async Task<bool> IsUserInGroupAsync(int userId, int groupId, string accessToken)
    {
        var groupClient = _httpClientFactory.CreateClient();
        // Cấu hình cứng URL của ContractGroupService (Port 5196)
        groupClient.BaseAddress = new Uri("http://localhost:5196");
        groupClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            // Gọi API lấy danh sách nhóm của User ("My Groups")
            var groupRes =
                await groupClient.GetFromJsonAsync<ApiResultExternal<List<GroupDtoExternal>>>("api/Groups/my");

            if (groupRes == null || !groupRes.Success || groupRes.Data == null) return false;

            // Check xem groupId có nằm trong danh sách nhóm của user không
            return groupRes.Data.Any(g => g.CoOwnerGroupId == groupId);
        }
        catch
        {
            // Log error nếu cần
            return false;
        }
    }
}

// --- Các Class DTO dùng tạm để hứng dữ liệu JSON ---
public class ApiResultExternal<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
}

public class VehicleDtoExternal
{
    public int CoOwnerGroupId { get; set; }
}

public class GroupDtoExternal
{
    public int CoOwnerGroupId { get; set; }
}