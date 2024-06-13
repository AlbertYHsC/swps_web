using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swps_web.Areas.Identity.Data;
using swps_web.Data;
using swps_web.Models;
using swps_web.Models.ViewModels;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace swps_web.Controllers;

[Authorize]
public class DeviceController : Controller
{
    private readonly ILogger<DeviceController> _logger;
    private readonly swps_dbContext _context;
    private readonly swps_UserManager<swps_webUser> _userManager;
    private readonly IConfiguration _configuration;

    public DeviceController(
        ILogger<DeviceController> logger,
        swps_dbContext context,
        swps_UserManager<swps_webUser> userManager,
        IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(DeviceViewModel? deviceVM)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (ModelState.IsValid)
        {
            var tcpData = new DeviceTCPModel<DeviceViewModel>
            {
                Api = "get_edges",
                Result = 0,
                Data = new()
            };

            try
            {
                var tcpClient = new TcpClient(
                    _configuration["SWPSService:Host"]!,
                    Convert.ToInt16(_configuration["SWPSService:Port"]));

                var tcpJson = JsonSerializer.Serialize(tcpData);
                var tcpBytes = Encoding.UTF8.GetBytes(tcpJson);
                var stream = tcpClient.GetStream();
                stream.Write(tcpBytes, 0, tcpBytes.Length);

                tcpBytes = new byte[2048];
                var tcpNum = stream.Read(tcpBytes, 0, tcpBytes.Length);
                tcpJson = Encoding.UTF8.GetString(tcpBytes, 0, tcpNum);
                tcpData = JsonSerializer.Deserialize<DeviceTCPModel<DeviceViewModel>>(tcpJson);

                tcpClient.Close();
            }
            catch
            {
                throw;
            }

            if ((tcpData != null) && (tcpData.Result == 1))
            {
                deviceVM = tcpData.Data;

                if ((deviceVM != null) && (deviceVM.Clients != null))
                {
                    deviceVM.Clients = deviceVM.Clients.Select(c =>
                    {
                        c.Registered = _context.Device.Any(d => d.DeviceSN == c.DeviceSN);
                        return c;
                    }).ToList();

                    deviceVM.Clients = (from c in deviceVM.Clients
                                        join d in _context.Device on c.DeviceSN equals d.DeviceSN into cd
                                        from d in cd.DefaultIfEmpty()
                                        where ((!c.Registered) || (d.UserId == user.Id))
                                        orderby c.DeviceSN ascending
                                        select c).ToList();
                }
            }
        }
        
        return View(deviceVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Device device, ResetWiFiViewModel resetWiFi)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (device.DetectInterval == 0)
        {
            device.DetectInterval = 10;
            device.PumpStartTime = 0.5M;
            device.SoilMoisture = 26000;
		}
        else if (ModelState.IsValid)
        {
            var tcpData = new DeviceTCPModel<ResetWiFiViewModel>
            {
                Api = "reset_wifi",
                Result = 0,
                Data = resetWiFi
            };

            try
            {
                var tcpClient = new TcpClient(
                    _configuration["SWPSService:Host"]!,
                    Convert.ToInt16(_configuration["SWPSService:Port"]));

                var tcpJson = JsonSerializer.Serialize(tcpData);
                var tcpBytes = Encoding.UTF8.GetBytes(tcpJson);
                var stream = tcpClient.GetStream();
                stream.Write(tcpBytes, 0, tcpBytes.Length);

                tcpBytes = new byte[2048];
                var tcpNum = stream.Read(tcpBytes, 0, tcpBytes.Length);
                tcpJson = Encoding.UTF8.GetString(tcpBytes, 0, tcpNum);
                tcpData = JsonSerializer.Deserialize<DeviceTCPModel<ResetWiFiViewModel>>(tcpJson);

                tcpClient.Close();
            }
            catch
            {
                throw;
            }

            if ((tcpData != null) && (tcpData.Result == 1))
            {
                device.Id = Guid.NewGuid().ToString();
                device.UserId = user.Id;

                _context.Device.Add(device);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("DeviceFailure");
            }
		}

        var deviceVM = new RegisterDeviceViewModel
        {
            Device = device,
            ResetWiFi = resetWiFi
        };
        
        return View(deviceVM);
    }

    [HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult ResetWiFi(ResetWiFiViewModel wifiVM)
    {
        if (ModelState.IsValid)
        {
            var tcpData = new DeviceTCPModel<ResetWiFiViewModel>
            {
                Api = "reset_wifi",
                Result = 0,
                Data = wifiVM
            };

            try
            {
                var tcpClient = new TcpClient(
                    _configuration["SWPSService:Host"]!,
                    Convert.ToInt16(_configuration["SWPSService:Port"]));

                var tcpJson = JsonSerializer.Serialize(tcpData);
                var tcpBytes = Encoding.UTF8.GetBytes(tcpJson);
                var stream = tcpClient.GetStream();
                stream.Write(tcpBytes, 0, tcpBytes.Length);

                tcpBytes = new byte[2048];
                var tcpNum = stream.Read(tcpBytes, 0, tcpBytes.Length);
                tcpJson = Encoding.UTF8.GetString(tcpBytes, 0, tcpNum);
                tcpData = JsonSerializer.Deserialize<DeviceTCPModel<ResetWiFiViewModel>>(tcpJson);

                tcpClient.Close();
            }
            catch
            {
                throw;
            }

            if ((tcpData != null) && (tcpData.Result == 1))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("DeviceFailure");
            }
		}
        
        return View(wifiVM);
    }

    [HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ChangeSettings(Device device)
    {
        var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		var devices = from d in _context.Device
					  where d.UserId == user.Id
					  where d.DeviceSN == device.DeviceSN
					  select d;

		var device_old = await devices.FirstOrDefaultAsync();

        if (device_old != null)
        {
            if (device.DetectInterval == 0)
            {
				device.DetectInterval = device_old.DetectInterval;
				device.PumpStartTime = device_old.PumpStartTime;
				device.SoilMoisture = device_old.SoilMoisture;
			}
            else if (ModelState.IsValid)
            {
                device_old.DetectInterval = device.DetectInterval;
                device_old.PumpStartTime = device.PumpStartTime;
                device_old.SoilMoisture = device.SoilMoisture;
				try
				{
					_context.Device.Update(device_old);
					await _context.SaveChangesAsync();
					return RedirectToAction("Index");
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DeviceExists(device_old.Id))
					{
						return NotFound($"Unable to update device '{device_old.DeviceSN}'.");
					}
					else
					{
						throw;
					}
				}
			}
        }
        else
        {
            return NotFound($"Unable to update unregistered device '{device.DeviceSN}'.");
		}
        
        return View(device);
    }

    public IActionResult DeviceFailure()
    {
        return View();
    }

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private bool DeviceExists(string? id)
    {
        return _context.Device.Any(e => e.Id == id);
    }
}
