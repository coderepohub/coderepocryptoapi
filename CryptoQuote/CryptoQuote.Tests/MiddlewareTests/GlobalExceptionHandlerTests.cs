using CryptoQuote.API.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace CryptoQuote.Tests.MiddlewareTests
{
    public class GlobalExceptionHandlerTests
    {
        [Fact]
        public async Task InvokeAsync_Should_Handle_Exception_And_Write_Response()
        {
            // Arrange
            var middleware = new GlobalExceptionHandler(next: (innerHttpContext) =>
            {
                throw new Exception("Test the global exception");
            });

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            context.Response.ContentType.Should().Be("application/json");

            var expectedResponse = new
            {
                error = new
                {
                    message = "An error occurred while processing your request.",
                    details = "Test the global exception"
                }
            };

            //Taking the body seek to begnining to read
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var actualResponse = await reader.ReadToEndAsync();

            actualResponse.Should().Be(JsonConvert.SerializeObject(expectedResponse));
        }

    }
}
