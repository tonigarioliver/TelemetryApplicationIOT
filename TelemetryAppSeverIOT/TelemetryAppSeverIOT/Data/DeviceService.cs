using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System;
using System.Text.Json;
using TelemetryAppSeverIOT.Data;
using TelemetryApiRest.Models.DTO;
using System.Text;

namespace TelemetryAppSeverIOT.Data
{
    public class DeviceService
    {
        public async Task<List<DeviceModel>> GetDevicesAsync()
        {
            using (var client = new HttpClient())
            {
                string endpointUrl = "http://localhost:5012/api/Device/GetAllDevices";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(endpointUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        // Deserializa el contenido de la respuesta a un arreglo de DeviceModel
                        List<DeviceModel> devices = JsonSerializer.Deserialize< List<DeviceModel>>(content);

                        return devices;
                    }
                    else
                    {
                        throw new HttpRequestException($"Error al realizar la solicitud. Código de estado: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task AddDeviceAsync(RegisterNewDeviceDTO newDevice)
        {
            using (var client = new HttpClient())
            {
                string endpointUrl = "http://localhost:5012/api/Device/RegisterNewDevice";

                // Serializa el nuevo dispositivo a JSON
                string jsonDevice = JsonSerializer.Serialize(newDevice);

                // Configura el encabezado de contenido JSON
                var content = new StringContent(jsonDevice, Encoding.UTF8, "application/json");

                // Realiza la solicitud POST
                await client.PostAsync(endpointUrl, content);
            }
        }
        public async Task UpdateDeviceAsync(DeviceModel updateDeviceModel)
        {
            using (var client = new HttpClient())
            {
                string endpointUrl = $"http://localhost:5012/api/Device/UpdateDevice/{updateDeviceModel.SerialNumber}";

                // Serializa el nuevo dispositivo a JSON
                string jsonDevice = JsonSerializer.Serialize(updateDeviceModel);

                // Configura el encabezado de contenido JSON
                var content = new StringContent(jsonDevice, Encoding.UTF8, "application/json");

                // Realiza la solicitud POST
                await client.PutAsync(endpointUrl, content);
            }
        }
        public async Task DeleteDeviceAsync(string serialNumber)
        {
            using (var client = new HttpClient())
            {
                string endpointUrl = $"http://localhost:5012/api/Device/DeleteDevice/{serialNumber}";

                // Realiza la solicitud POST
                await client.DeleteAsync(endpointUrl);
            }
        }
    }
}

