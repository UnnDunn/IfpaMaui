using System.Collections.ObjectModel;
using System.Diagnostics;
using PinballApi.Models.WPPR.v1.Calendar;
using Ifpa.Models;
using PinballApi;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Extensions.Logging;
using XCalendar.Core.Models;
using XCalendar.Core.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using XCalendar.Core.Extensions;
using XCalendar.Core.Enums;

namespace Ifpa.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        public ObservableCollectionRange<CalendarDetails> CalendarDetails { get; set; }

        public ObservableRangeCollection<CalendarDetails> SelectedEvents { get; } = new ObservableRangeCollection<CalendarDetails>();

        public Calendar<EventDay> EventCalendar { get; set; } = new Calendar<EventDay>()
        {
            SelectedDates = new ObservableRangeCollection<DateTime>(),
            SelectionAction = SelectionAction.Replace,
            SelectionType = SelectionType.Single
        };

        public ICommand ChangeDateSelectionCommand { get; set; }

        public ICommand NavigateCalendarCommand { get; set; }


        public ObservableCollection<Pin> Pins { get; set; }

        public List<Color> Colors { get; } = new List<Color>() { Microsoft.Maui.Graphics.Colors.Red, Microsoft.Maui.Graphics.Colors.Orange, Microsoft.Maui.Graphics.Colors.Yellow, Color.FromArgb("#00A000"), Microsoft.Maui.Graphics.Colors.Blue, Color.FromArgb("#8010E0") };

        public Command LoadItemsCommand { get; set; }

        public CalendarViewModel(PinballRankingApiV1 pinballRankingApiV1, PinballRankingApiV2 pinballRankingApiV2, ILogger<CalendarViewModel> logger) : base(pinballRankingApiV1, pinballRankingApiV2, logger)
        {
            Title = "Calendar";
            CalendarDetails = new ObservableCollectionRange<CalendarDetails>();
            Pins = new ObservableCollection<Pin>();


            ChangeDateSelectionCommand = new Command<DateTime>(ChangeDateSelection);
            NavigateCalendarCommand = new Command<int>(NavigateCalendar);

            EventCalendar.SelectedDates.CollectionChanged += SelectedDates_CollectionChanged;
            EventCalendar.DaysUpdated += EventCalendar_DaysUpdated;
        }

        public async Task ExecuteLoadItemsCommand(string address, int distance)
        {
            IsBusy = true;

            try
            {
                var sw = Stopwatch.StartNew();
                CalendarDetails.Clear();
                //InlineCalendarItems.Clear();

                logger.LogDebug("Cleared collections in {0}", sw.ElapsedMilliseconds);

                var items = await PinballRankingApi.GetCalendarSearch(address, distance, DistanceUnit.Miles);

                logger.LogDebug("Api call completed at {0}", sw.ElapsedMilliseconds);

                if (items.Calendar.Any())
                {
                    CalendarDetails.AddRange(items.Calendar.OrderBy(n => n.EndDate));

                    //Limit calendar to 100 future items. otherwise this page chugs
                    foreach (var detail in CalendarDetails)
                    {
                        LoadEventOntoMap(detail);
                    }

                    foreach (var day in EventCalendar.Days)
                    {
                        day.Events.ReplaceRange(items.Calendar.Where(x => x.StartDate.Date == day.DateTime.Date));
                    }
                }

                logger.LogDebug("Collections loaded at {0}", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading calendar items");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadEventOntoMap(CalendarDetails detail)
        {
            var location = new Location(detail.Latitude, detail.Longitude);

            //check for duplicate pins at this location. Don't add another pin to the same place.
            if (Pins.Any(n => n.Location == location) == false)
            {
                var pin = new Pin();

                pin.Location = location;
                pin.Label = detail.TournamentName;
                pin.Address = detail.Address1 + " " + detail.City + ", " + detail.State;
                pin.Type = PinType.Generic;
                pin.MarkerId = detail.CalendarId.ToString();

                Pins.Add(pin);
            }
        }

        private void EventCalendar_DaysUpdated(object sender, EventArgs e)
        {
            SelectedEvents.Clear();
        }

        public void ChangeDateSelection(DateTime dateTime)
        {
            EventCalendar?.ChangeDateSelection(dateTime);
        }

        private void SelectedDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {            
            SelectedEvents.ReplaceRange(CalendarDetails.Where(x => EventCalendar.SelectedDates.Any(y => x.StartDate.Date == y.Date)).OrderByDescending(x => x.StartDate));
        }

        public void NavigateCalendar(int amount)
        {
            if (EventCalendar.NavigatedDate.TryAddMonths(amount, out DateTime targetDate))
            {
                EventCalendar.Navigate(targetDate - EventCalendar.NavigatedDate);
            }
            else
            {
                EventCalendar.Navigate(amount > 0 ? TimeSpan.MaxValue : TimeSpan.MinValue);
            }
        }
    }
}