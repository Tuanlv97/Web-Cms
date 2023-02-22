using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cms.Api
{
    public static class SerilogClass
    {
        public static readonly Serilog.ILogger _log;
        static SerilogClass()
        {
            _log = new LoggerConfiguration().
                    MinimumLevel.Debug().
                    WriteTo.File(@Environment.GetEnvironmentVariable("LocalAppData") + "\\Logs\\Logs1.log").
                    CreateLogger();
        }


    }
}