using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace PlayerQuests.Helpers;

public class Location
{
    public uint TerritoryType = 0;
    public uint WorldId = 0;
    public string? TerritoryName => Services.DataManager.GetExcelSheet<TerritoryType>()?.GetRow(TerritoryType)?.PlaceName.Value?.Name.ToString();
    public bool ValidTerritory => TerritoryName != null;
    public string? WorldName => Services.DataManager.GetExcelSheet<World>()?.GetRow(WorldId)?.Name.ToString();
    public string? DatacenterName => Services.DataManager.GetExcelSheet<World>()?.GetRow(WorldId)?.DataCenter.Value?.Name.ToString();
    public bool ValidWorld => WorldName != null;

    public bool IsHousing = false;
    public bool IsApartment = false;
    public bool IsYard = false;
    public ushort Ward;
    public ushort Plot;
    public ushort ApartmentWing;
    public ushort Apartment;

    public static unsafe Location? GetLocation()
    {
        var territory = Services.ClientState.TerritoryType;
        if (territory == 0)
        {
            return null;
        }

        var world = Services.ClientState.LocalPlayer?.CurrentWorld.Id ?? 0;
        if (world == 0)
        {
            return null;
        }

        var location = new Location
        {
            TerritoryType = territory,
            WorldId = world,
        };

        var housingManager = HousingManager.Instance();
        var currentPlot = housingManager->GetCurrentPlot();
        if (currentPlot < -1)
        {
            location.ApartmentWing = (ushort)((unchecked((byte)currentPlot) & ~0x80) + 1);
            location.Apartment = (ushort)housingManager->GetCurrentRoom();
            if (location.Apartment != 0)
            {
                location.IsHousing = true;
                location.IsApartment = true;
            }
        }
        else if (currentPlot > 0)
        {
            location.IsHousing = true;
            location.Plot = (ushort)(housingManager->GetCurrentPlot() + 1);
            location.IsYard = housingManager->GetCurrentHouseId() == -1;
        }

        var ward = housingManager->GetCurrentWard();
        if (ward >= 0)
        {
            location.Ward = (ushort)(ward + 1);
        }

        return location;
    }
}
