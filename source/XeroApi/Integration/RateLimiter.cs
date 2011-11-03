using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XeroApi.Integration
{
    class RateLimiter
    {
        public TimeSpan Duration { get; private set; }
        public int Qty { get; private set; }

        List<DateTime> rateLimiter = null;

        /// <summary>
        /// Create an instance of this class that allows x requests over y seconds
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="qty"></param>
        public RateLimiter(TimeSpan duration, int qty)
        {
            this.Duration = duration;
            this.Qty = qty;
            this.rateLimiter = new List<DateTime>(Qty);
        }

        /// <summary>
        /// Don't return from this method until we're back under the limit
        /// </summary>
        public void WaitUntilLimit()
        {
            while (rateLimiter.Count >= Qty)
            {
                while (rateLimiter[0].Add(Duration) > DateTime.UtcNow)
                {
                    Thread.Sleep(1000);
                }
                rateLimiter.RemoveAt(0);
            }
            rateLimiter.Add(DateTime.UtcNow);
        }

        /// <summary>
        /// Check whether we've used up all of the allocation
        /// </summary>
        /// <returns>True if we're over the limit, false if we've got some allocation left</returns>
        public bool CheckLimit()
        {
            return (rateLimiter.Count >= Qty && rateLimiter[0].Add(Duration) > DateTime.UtcNow);
        }
        
    }
}
