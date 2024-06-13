using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using swps_web.Areas.Identity.Data;
using swps_web.Data;
using swps_web.Models;
using swps_web.Models.ViewModels;

namespace swps_web.Controllers;

[Authorize]
public class RecordController : Controller
{
    private readonly swps_dbContext _context;
    private readonly swps_UserManager<swps_webUser> _userManager;

    public RecordController(
        swps_dbContext context,
        swps_UserManager<swps_webUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: RecordController
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (_context.Device == null)
        {
            return Problem("Entity set 'swps_dbContext.Device' is null.");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var devices = from d in _context.Device
                      where d.UserId == user.Id
                      orderby d.DeviceSN ascending
                      select d.DeviceSN;

        var recordVM = new RecordViewModel
        {
            Records = new List<RecordDevice>(),
            Devices = new SelectList(await devices.Distinct().ToListAsync())
        };

        return View(recordVM);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string deviceSN, DateTime startTime, DateTime endTime)
    {
        if (_context.Record == null || _context.Device == null)
        {
            return Problem("Entity set 'swps_dbContext.Record' or 'swps_dbContext.Device' is null.");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var devices = from d in _context.Device
                      where d.UserId == user.Id
                      orderby d.DeviceSN ascending
                      select d.DeviceSN;

        var records = from r in _context.Record
                      join d in _context.Device on r.DeviceId equals d.Id into rd
                      from d in rd.DefaultIfEmpty()
                      where r.UserId == user.Id
                      where r.DetectTime >= startTime & r.DetectTime <= endTime
                      orderby r.DetectTime descending
                      select new RecordDevice { Record = r, DeviceSN = d.DeviceSN };

        if (!String.IsNullOrEmpty(deviceSN))
        {
            records = records.Where(r => r.DeviceSN == deviceSN);
        }

        var recordVM = new RecordViewModel
        {
            Records = await records.ToListAsync(),
            Devices = new SelectList(await devices.Distinct().ToListAsync())
        };
        
        return View(recordVM);
    }

    // GET: RecordController/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound("No input record id.");
        }

        var record = await _context.Record
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound($"Unable to load record with ID '{id}'");
        }

		var device = await _context.Device
			.FirstOrDefaultAsync(d => d.Id == record.DeviceId);

        var record_device = new RecordDevice
        {
            Record = record,
            DeviceSN = device?.DeviceSN
        };

		return View(record_device);
    }
}
