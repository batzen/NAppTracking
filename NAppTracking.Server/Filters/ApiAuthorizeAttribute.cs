namespace NAppTracking.Server.Filters
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Mvc;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Helpers;
    using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

    public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        #region Overrides of AuthorizeAttribute

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var apiKey = ApiKeyHelper.GetApiKey(actionContext.Request);;

            var db = DependencyResolver.Current.GetService<IEntitiesContext>();

            var trackingApplication = db.TrackingApplications.FirstOrDefault(x => x.ApiKey == apiKey);

            if (trackingApplication != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Processes requests that fail authorization.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response = response;
        }

        #endregion
    }
}