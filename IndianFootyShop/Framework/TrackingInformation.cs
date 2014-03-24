using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Web;

namespace Framework
{
    public class TrackingInformation : ITrackingInformation
    {
        public virtual string CurrentUserId
        {
            get
            {
                var identityName = string.Empty;
                var context = HttpContext.Current;
                if (context != null && context.User != null && !string.IsNullOrEmpty(context.User.Identity.Name))
                {
                    identityName = context.User.Identity.Name;
                }
                else
                {
                    identityName = String.IsNullOrWhiteSpace(Thread.CurrentPrincipal.Identity.Name)
                                       ? Environment.UserName
                                       : Thread.CurrentPrincipal.Identity.Name;
                }
                return identityName;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual DateTime GetCurrentTime()
        {
            //This is an example of where Microsoft regrets making something a property.
            //return ApplicationTime.GetCurrentTime();
            return System.DateTime.Now;
        }
    }
}