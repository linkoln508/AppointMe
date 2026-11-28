using AppointMe.Api.Data;
using AppointMe.Api.Models;
using AppointMe.Api.Models.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointMe.Api.Controllers;

[ApiController]
[Route("api/businesses/{businessId:guid}/working-hours")]
public class WorkingHoursController : ControllerBase
{
    private readonly IWorkingHoursRepository _workingHours;
    private readonly IBusinessRepository _businesses;
    
    public WorkingHoursController(IWorkingHoursRepository workingHours, IBusinessRepository businesses)
    {
        _workingHours = workingHours;
        _businesses = businesses;
    }

        [AllowAnonymous]   // смотреть расписание может любой, без входа
    [HttpGet]
    public async Task<IActionResult> Get(Guid businessId)
    {
        if(await _businesses.GetByIdAsync(businessId) == null)
            {
                return NotFound();
            }

        var rows = await _workingHours.GetByBusinessIdAsync(businessId);

        var week = new List<WorkingDayDto>();

        for (int i = 0; i < 7; i++)
        {
            var day = (DayOfWeek)i;

            var row = rows.FirstOrDefault(r => r.DayOfWeek == day);

            if (row == null)
            {
                week.Add(new WorkingDayDto { DayOfWeek = day, IsClosed = true });
            }
            else
            {
                week.Add(new WorkingDayDto{ 
                            DayOfWeek = row.DayOfWeek,
                            IsClosed = row.IsClosed,
                            OpenTime = row.OpenTime,
                            CloseTime = row.CloseTime,
                            BreakStart = row.BreakStart,
                            BreakEnd = row.BreakEnd
            });
            }

        }
        return Ok(week);
    }
}
