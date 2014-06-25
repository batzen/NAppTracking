using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAppTracking.Client.Tests
{
    using System.Net;
    using System.Net.Http;
    using NUnit.Framework;

    [TestFixture]
    public class TrackingResultTests
    {
        [Test]
        public void TestCombine()
        {

        }

        [Test]
        public void TestToStringWithSuccess()
        {
            var trackingResult = new TrackingResult();

            var toString = trackingResult.ToString();
            Assert.That(toString, Is.Not.Null);

            Assert.That(toString, Is.StringContaining("Success: True"));

            Assert.That(toString, Is.Not.StringContaining("Exception"));

            Assert.That(toString, Is.Not.StringContaining("Response"));
        }

        [Test]
        public void TestToStringWithOneException()
        {
            var trackingResult = new TrackingResult(new Exception("Test-Exception"));

            var toString = trackingResult.ToString();
            Assert.That(toString, Is.Not.Null);

            Assert.That(toString, Is.StringContaining("Success: False"));

            Assert.That(toString, Is.StringContaining(@"Message: Test-Exception
Details: System.Exception: Test-Exception"));

            Assert.That(toString, Is.Not.StringContaining("Response"));
        }

        [Test]
        public void TestToStringWithOneSuccessResponse()
        {
            var trackingResult = new TrackingResult(new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new StringContent("Response-Text") });

            var toString = trackingResult.ToString();
            Assert.That(toString, Is.Not.Null);

            Assert.That(toString, Is.StringContaining("Success: True"));

            Assert.That(toString, Is.Not.StringContaining("Exception"));

            Assert.That(toString, Is.StringContaining(@"StatusCode: Accepted
Response: Response-Text"));
        }
    }
}