using System;
using System.Web;

namespace DuesManager.Helpers
{
    public static class PageHelper
    {
        public static void CompleteDataOperation(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            string redirectUrl = context.Request.Url.AbsolutePath + "?hash=" + DateTime.Now.Ticks;

            context.Response.Redirect(redirectUrl, false);
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
