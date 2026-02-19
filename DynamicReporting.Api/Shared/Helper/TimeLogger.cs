namespace DynamicReporting.Api.Shared.Helper
{
    public static class TimeLogger
    {
        public static T Time<T>(Func<T> func, string label)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return func();
            }
            finally
            {
                sw.Stop();
                Log.Error("{Label} took {Elapsed} ms", label, sw.ElapsedMilliseconds);
            }
        }

        public static void Time(Action action, string label)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                sw.Stop();
                Log.Error("{Label} took {Elapsed} ms", label, sw.ElapsedMilliseconds);
            }
        }

        public static async Task<T> TimeAsync<T>(Func<Task<T>> func, string label)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return await func();
            }
            finally
            {
                sw.Stop();
                Log.Error("{Label} took {Elapsed} ms", label, sw.ElapsedMilliseconds);
            }
        }

        public static async Task TimeAsync(Func<Task> func, string label)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await func();
            }
            finally
            {
                sw.Stop();
                Log.Error("{Label} took {Elapsed} ms", label, sw.ElapsedMilliseconds);
            }
        }
    }
}
