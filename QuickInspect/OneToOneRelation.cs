using System;

namespace QuickInspect
{
    public class OneToOneRelation
    {
        public Type Left { get; set; }
        public Type Right { get; set; }
        public Func<object, object> CreateRight;
        public Func<object, object> GetRight { get; set; }
    }

}
