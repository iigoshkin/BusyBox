using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BusyBox.AspNetCore
{
    /// <summary>A timed background task</summary>
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        private CancellationTokenSource? _stoppingCts;

        public abstract TimeSpan Interval { get; }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken token = _stoppingCts.Token;
            int totalMilliseconds = Convert.ToInt32(Interval.TotalMilliseconds);

            while (true)
            {
                await ExecuteAsync(token);

                await Task.Delay(totalMilliseconds, token);
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            if (_stoppingCts != null)
                _stoppingCts.Cancel();

            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            _stoppingCts?.Cancel();
            _stoppingCts?.Dispose();
        }
    }
}
