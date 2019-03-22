using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModLibrary
{
    public static class Delayed
    {
        public static void TriggerAfterDelay(fakeAction action, TimeSpan time)
        {
            WaitThenCallClass.Instance.AddCallback(action, time);
        }

        public static void TriggerAfterDelay(fakeAction action, float seconds)
        {
            int intSeconds = (int)Math.Floor(seconds);
            int miliseconds = (int)((seconds - intSeconds)*1000);
            int hours = intSeconds / 60;
            intSeconds = intSeconds % 60;

            WaitThenCallClass.Instance.AddCallback(action, new TimeSpan(0, hours, intSeconds, miliseconds));
        }
    }
}
