using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    static class Disposable
    {
        public static IDisposable Create(Action dispose)
        {
            return new AnonymousDisposable(dispose);
        }

        public static IDisposable Empty
        {
            get { return EmptyDisposable.Value; }
        }

        static Lazy<IDisposable> EmptyDisposable = new Lazy<IDisposable>(() => new AnonymousDisposable(() => { }));

        class AnonymousDisposable : IDisposable
        {
            volatile Action dispose;
            public AnonymousDisposable(Action dispose)
            {
                this.dispose = dispose;
            }

            public void Dispose()
            {
#pragma warning disable 0420
                var action = Interlocked.Exchange(ref dispose, null);
#pragma warning restore 0420
                if (action != null)
                {
                    action();
                }
            }
        }
    }
}
