using System;

namespace Meetup.FunctionApp.Infrastructure
{
    public static class FunctionHelper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
