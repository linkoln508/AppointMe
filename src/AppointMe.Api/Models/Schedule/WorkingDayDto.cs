namespace AppointMe.Api.Models.Schedule;

public class WorkingDayDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsClosed { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public TimeOnly? BreakStart { get; set; }
    public TimeOnly? BreakEnd { get; set; }
}