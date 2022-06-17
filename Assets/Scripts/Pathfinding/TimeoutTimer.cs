using System;
using System.Threading;
using System.Threading.Tasks;

public class TimeoutTimer
{
    private CancellationTokenSource cancellation;
    private readonly int timeout;

    public event Action TimeoutAction;

    public TimeoutTimer(int timeout)
    {
        cancellation = new CancellationTokenSource();
        this.timeout = timeout;
    }

    public void Begin()
    {
        Task.Run(() => TimeoutTask(cancellation.Token), cancellation.Token);
    }

    private async Task TimeoutTask(CancellationToken token)
    {
        await Task.Delay(timeout);

        if (!token.IsCancellationRequested)
        {
            TimeoutAction?.Invoke();
        }

        cancellation.Dispose();
    }

    public void Cancel()
    {
        cancellation.Cancel();
    }
}