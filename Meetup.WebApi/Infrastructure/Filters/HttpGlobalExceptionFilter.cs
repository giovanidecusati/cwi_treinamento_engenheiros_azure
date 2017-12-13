using Meetup.WebApi.Infrastructure.ActionResults;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Meetup.WebApi.Infrastructure.Filters
{
    public partial class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private readonly TelemetryClient _telemetryClient;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, TelemetryClient telemetryClient)
        {
            _env = env;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var json = new JsonErrorResponse()
            {
                Messages = new[] { "An error occurred. Try it again." }
            };

            if (_env.IsDevelopment())
            {
                json.DeveloperMessage = context.Exception;
            }

            _telemetryClient.TrackException(context.Exception);


            context.Result = new InternalServerErrorObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
        }
    }
}
