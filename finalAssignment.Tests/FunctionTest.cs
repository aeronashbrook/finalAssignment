using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using finalAssignment;

namespace finalAssignment.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestItem()
        {
            Item item = new Item();
            item.price = 56960.69;
            Assert.True(item.price > 0);
        }
    }
}
