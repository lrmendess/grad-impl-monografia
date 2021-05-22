using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Text;

namespace SCAP.Configurations
{
    public static class KissLogConfig
    {
        public static IOptionsBuilder ConfigureKissLog(this IOptionsBuilder options)
        {
            options.Options.AppendExceptionDetails((Exception ex) =>
            {
                StringBuilder sb = new StringBuilder();

                if (ex is System.NullReferenceException nullRefException)
                {
                    sb.AppendLine("Important: check for null references");
                }

                return sb.ToString();
            });

            options.InternalLog = (message) =>
            {
                Debug.WriteLine(message);
            };

            return options;
        }

        public static IOptionsBuilder RegisterKissLogListeners(this IOptionsBuilder options, IConfiguration configuration)
        {
            options.Listeners.Add(new RequestLogsApiListener(
                new Application(configuration["KissLog:OrganizationId"], configuration["KissLog:ApplicationId"]))
            {
                ApiUrl = configuration["KissLog:ApiUrl"]
            });

            return options;
        }
    }
}
