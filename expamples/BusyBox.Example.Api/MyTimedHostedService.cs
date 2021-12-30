using System;
using System.Threading;
using System.Threading.Tasks;
using BusyBox.AspNetCore;

namespace BusyBox.Example.Api
{
    public class MyTimedHostedService : TimedHostedService
    {
        public override TimeSpan Interval => TimeSpan.FromSeconds(20);

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Hello world");
            await Task.Delay(30, cancellationToken);
        }
    }
}
