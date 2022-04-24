using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;

namespace DataverseFunctionTests
{
    [TestClass]
    public class UnitTest1
    {
        public HttpRequest HttpRequestSetup(Dictionary<String, StringValues> query, string body)
        {
            var reqMock = new Mock<HttpRequest>();

            reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);
            return reqMock.Object;
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}