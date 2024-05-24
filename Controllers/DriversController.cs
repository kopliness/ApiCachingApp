using ApiCachingApp.Data;
using ApiCachingApp.Models;
using ApiCachingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCachingApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DriversController: ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly ApiDbContext _context;

    public DriversController(ICacheService cacheService, ApiDbContext context)
    {
        _cacheService = cacheService;
        _context = context;
    }
    
    [HttpGet("GetDrivers")]
    public async Task<IActionResult> GetDrivers()
    {
        var cacheDrivers = _cacheService.GetData<IEnumerable<Driver>>("drivers");
        if (cacheDrivers !=null && cacheDrivers.Count() > 0) 
            return Ok(cacheDrivers);

        var drivers = await _context.Drivers.ToListAsync();

        _cacheService.SetData("drivers", drivers, DateTimeOffset.Now.AddMinutes(2));
        return Ok(drivers);
    }
    [HttpPost("AddDriver")]
    [AllowAnonymous]
    public async Task<IActionResult> AddDriver(Driver driver)
    {
        if (driver == null)
            return BadRequest();
        await _context.Drivers.AddAsync(driver);
        await _context.SaveChangesAsync();
        
        _cacheService.RemoveData("drivers");
        return Ok(driver);
    }
}