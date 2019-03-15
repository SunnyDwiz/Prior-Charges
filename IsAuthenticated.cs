using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceNowAppTool.Common
{
    public class IsAuthenticatedAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            var isAuthorized = base.AuthorizeCore(httpContext);

            if (!isAuthorized)
            {
                return false; //User not Authorized
            }

            else
            {
                //Check your conditions here
                return true;
            }

        }
    }

}