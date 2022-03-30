using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(IcreCreamRatingAPI_WithEntitySQL.Startup))]

namespace IcreCreamRatingAPI_WithEntitySQL
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string SqlConnection = Environment.GetEnvironmentVariable("ConnectionString");
            builder.Services.AddDbContext<IceCreamRatingContext>(
                options => options.UseSqlServer(SqlConnection));
        }
    }
}