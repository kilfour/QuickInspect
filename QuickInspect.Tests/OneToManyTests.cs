using System.Collections.Generic;
using Xunit;

namespace QuickInspect.Tests
{
    public class OneToManyTests
    {
        [Fact]
        public void AreMemberWiseEqualOneChildEachTrue()
        {
            var thingOne =new Something{MySomethingElse = new List<SomethingElse>{new SomethingElse{MyProp = 42}}};
            var thingTwo = new Something { MySomethingElse = new List<SomethingElse> { new SomethingElse { MyProp = 42 } } };
            var inspector =
                Inspect
                    .This(thingOne, thingTwo)
                    .Inspect<Something, SomethingElse>(s => s.MySomethingElse);
            Assert.True(inspector.AreMemberWiseEqual());
        }

        [Fact]
        public void AreMemberWiseEqualOneChildEachFalse()
        {
            var thingOne = new Something { MySomethingElse = new List<SomethingElse> { new SomethingElse { MyProp = 42 } } };
            var thingTwo = new Something { MySomethingElse = new List<SomethingElse> { new SomethingElse { MyProp = 43 } } };
            var inspector =
                Inspect
                    .This(thingOne, thingTwo)
                    .Inspect<Something, SomethingElse>(s => s.MySomethingElse);

            Assert.False(inspector.AreMemberWiseEqual());
        }

        [Fact]
        public void AreMemberWiseEqualDifferentNumberOfChildrenFalse()
        {
            var thingOne = new Something { MySomethingElse = new List<SomethingElse> { new SomethingElse(), new SomethingElse() } };
            var thingTwo = new Something { MySomethingElse = new List<SomethingElse> { new SomethingElse () } };
            var inspector =
                Inspect
                    .This(thingOne, thingTwo)
                    .Inspect<Something, SomethingElse>(s => s.MySomethingElse);

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