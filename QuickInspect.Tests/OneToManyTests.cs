using System.Collections.Generic;
using Xunit;

namespace QuickInspect.Tests
{
    public class OneToManyTests
    {
        [Fact]
        public void AreMemberWiseEqual()
        {
            var thingOne =new Something{MySomethingElse = new List<SomethingElse>{new SomethingElse{MyProp = 42}}};
            var thingTwo = new Something { MySomethingElse = new List<SomethingElse> { new SomethingElse { MyProp = 42 } } };
            var inspector =
                Inspect
                    .This(thingOne, thingTwo)
                    .Inspect<Something, SomethingElse>(s => s.MySomethingElse);
            Assert.True(inspector.AreMemberWiseEqual());
            thingTwo.MySomethingElse[0].MyProp = 43;
            Assert.False(inspector.AreMemberWiseEqual());
        }

        public class Something
        {
            public List<SomethingElse> MySomethingElse { get; set; }
            public Something()
            {
                MySomethingElse = new List<SomethingElse>();
            }
        }

        public class SomethingElse { public int MyProp { get; set; } }
    }
}