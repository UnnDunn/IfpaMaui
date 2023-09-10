using PinballApi.Models.WPPR.v1.Calendar;
using XCalendar.Core.Collections;
using XCalendar.Core.Interfaces;

namespace Ifpa.Models
{
    public class EventDay : BaseObservableModel, ICalendarDay
    {
        public DateTime DateTime { get; set; }
        public ObservableRangeCollection<CalendarDetails> Events { get; } = new ObservableRangeCollection<CalendarDetails>();
        public bool IsSelected { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsInvalid { get; set; }
    }
}
