namespace InnoClinic.Appointments.API.Application.Options
{
    public class WorkingHoursOptions
    {
        public required TimeOnly Start { get; set; }

        public required TimeOnly End { get; set; }
    }
}
