using Xunit;

namespace QuickInspect.Tests
{
    public class OneToOneTests
    {
        [Fact]
        public void CompositeTests()
        {
            var thingOne = new Something{MyProp = 42, MySomethingElse = new SomethingElse{MyProp = 42}};
            var thingTwo = new Something { MyProp = 42, MySomethingElse = new SomethingElse { MyProp = 42 } };
            var inspector = 
                Inspect
                    .This(thingOne, thingTwo)
                    .Inspect<Something, SomethingElse>(s => s.MySomethingElse);
            Assert.True(inspector.AreMemberWiseEqual());
            thingTwo.MySomethingElse.MyProp = 43;
            Assert.False(inspector.AreMemberWiseEqual());
        }

        public class Something
        {
            public SomethingElse MySomethingElse { get; set; }
            public int MyProp { get; set; }
        }

        public class SomethingElse { public int MyProp { get; set; } }
    }
}