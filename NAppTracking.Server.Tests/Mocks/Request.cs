namespace NAppTracking.Server.Tests.Mocks
{
    using System.Web;

    public class MockHttpContext : HttpContextBase
    {
        public MockHttpRequest request = new MockHttpRequest();
        public MockHttpResponse response = new MockHttpResponse();

        public override HttpRequestBase Request
        { get { return this.request; } }

        public override HttpResponseBase Response
        { get { return this.response; } }
    }

    public class MockHttpRequest : HttpRequestBase
    {
        public bool isAuthenticated = false;

        /// <summary>
        /// When overridden in a derived class, gets a value that indicates whether the request has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the request has been authenticated; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">Always.</exception>
        public override bool IsAuthenticated
        {
            get { return isAuthenticated; }
        }
    }

    public class MockHttpResponse : HttpResponseBase
    {
        // override whatever bits you want (eg cookies)
    }
}