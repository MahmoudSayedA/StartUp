using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.CleanUpJobs;
public static class RunScheduleJobsExtension
{
    public static void RunScheduleJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var refreshTokenCleanUp = scope.ServiceProvider.GetRequiredService<RefreshTokenCleanUp>();

        //fire background job here
        refreshTokenCleanUp.WeeklyCleanUpRefreshToken();
    }
}
