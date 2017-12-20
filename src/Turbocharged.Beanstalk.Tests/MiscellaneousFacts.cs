using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.Beanstalk.Tests
{
    public class MiscellaneousFacts
    {
        [Fact]
        public void EnsureNoAsyncVoidTests()
        {
            AssertNoAsyncVoidMethods(GetType().GetTypeInfo().Assembly);
        }

        static void AssertNoAsyncVoidMethods(Assembly assembly)
        {
            var messages = assembly
                .GetAsyncVoidMethods()
                .Select(method =>
                    String.Format("'{0}.{1}' is an async void method.",
                        method.DeclaringType.Name,
                        method.Name))
                .ToList();
            Assert.False(messages.Any(),
                "Async void methods found!" + Environment.NewLine + String.Join(Environment.NewLine, messages));
        }
    }
}
