using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REMOEngine
{
    public static class REMOTimer
    {
        private static Dictionary<Action, int> timers = new Dictionary<Action, int>();
        /// <summary>
        /// Activate this function in update routine. It'll automatically match a stopwatch for the action,
        /// and conduct a stopwatch action.
        /// </summary>
        /// <param name="ResetTime"></param>
        /// <param name="TargetAction"></param>
        public static void StopWatch(int ResetTime, Action TargetAction)
        {
            if (!timers.ContainsKey(TargetAction))
            {
                timers.Add(TargetAction, ResetTime);//Add timer for the action.
            }
            else
            {
                if (timers[TargetAction] > 0)
                    timers[TargetAction]--;
                else
                {
                    timers[TargetAction] = ResetTime;
                    TargetAction();
                }

            }
        }


    }
}
