using Graphitie.Models;

namespace Graphitie.Extensions;

public static class GraphitieExtensions
{
    public static Device WithManagedDevice(this Device device, Microsoft.Graph.ManagedDevice? managedDevice)
    {
        device.IsEncrypted = managedDevice?.IsEncrypted;
        device.FreeStorageSpaceInBytes = managedDevice?.FreeStorageSpaceInBytes;
        device.TotalStorageSpaceInBytes = managedDevice?.TotalStorageSpaceInBytes;
        device.LastSyncDateTime = managedDevice?.LastSyncDateTime;
        device.Imei = managedDevice?.Imei;
        device.ManagedDeviceId = managedDevice?.Id;
        device.SerialNumber = managedDevice?.SerialNumber;

        return device;
    }

}