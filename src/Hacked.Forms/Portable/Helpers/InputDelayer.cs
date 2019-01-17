using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hacked.Forms.Portable.Helpers
{
    /// <summary>
    /// Prevent too many actions when user is typing in search terms. This helper will only use input after a pause period
    /// </summary>
    public class InputDelayer
    {
        private DelayerTimer _delayerTimer;

        private readonly Action _action;

        public InputDelayer(Action action, int dueTime, int period)
        {
            _action = action;

            _delayerTimer = new DelayerTimer(TimerCallback, null, dueTime, period);
        }

        private void TimerCallback(object state)
        {
            _action.Invoke();
        }
    }

    public sealed class DelayerTimer : CancellationTokenSource
    {
        internal DelayerTimer(Action<object> callback, object state, int millisecondsDueTime, int millisecondsPeriod, bool waitForCallbackBeforeNextPeriod = false)
        {
            //Contract.Assert(period == -1, "This stub implementation only supports dueTime.");

            Task.Delay(millisecondsDueTime, Token).ContinueWith(async (t, s) =>
            {
                var tuple = (Tuple<Action<object>, object>) s;

                while (!IsCancellationRequested)
                {
                    if (waitForCallbackBeforeNextPeriod)
                    {
                        tuple.Item1(tuple.Item2);
                    }
                    else
                    {
                        await Task.Run(() => tuple.Item1(tuple.Item2));
                    }

                    await Task.Delay(millisecondsPeriod, Token).ConfigureAwait(false);
                }

            }, Tuple.Create(callback, state), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Cancel();

            base.Dispose(disposing);
        }
    }
    
}
