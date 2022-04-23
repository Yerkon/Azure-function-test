using FunctionApp2.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            //builder.Services.AddSingleton<IDataverserService>((s) => {
            //    return new MyService();
            //});
            builder.Services.AddSingleton<IDataverseService, DataverseService>();


            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
}