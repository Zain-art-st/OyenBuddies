using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class TimeSlot
    {
        public string Time { get; set; }
        public bool IsBooked { get; set; }

        public string Display => IsBooked ? $"{Time} (Booked)" : Time;

        // Static method — generates slots from 9am to 6pm in 30-min intervals
        public static List<TimeSlot> GenerateSlots()
        {
            var slots = new List<TimeSlot>();
            DateTime start = DateTime.Today.AddHours(9);
            DateTime end = DateTime.Today.AddHours(18);

            while (start < end)
            {
                slots.Add(new TimeSlot { Time = start.ToString("hh:mm tt") });
                start = start.AddMinutes(30);
            }
            return slots;
        }
    }
}
