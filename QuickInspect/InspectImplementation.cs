using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QuickInspect.NothingToSeeHereMoveAlong;

namespace QuickInspect
{
    public class InspectImplementation<T>
    {
        private readonly List<PropertyInfo> StuffToIgnore = new List<PropertyInfo>();

        private readonly List<OneToOneRelation> oneToOneRelations =
            new List<OneToOneRelation>();

        private readonly List<OneToManyRelation> oneToManyRelations =
            new List<OneToManyRelation>();

        private readonly object left;
        private readonly object right;

        public InspectImplementation(object left, object right)
        {
            this.left = left;
            this.right = right;
            RunReport = (b, m) => { };
        }

        public InspectImplementation<T> Ignore<TProperty>(Expression<Func<T, TProperty>> func)
        {
            StuffToIgnore.Add(func.AsPropertyInfo());
            return this;
        }

        public Action<bool, string> RunReport { get; set; }

        public Func<object, object, bool> Apply { get; set; }

        public bool AreMemberWiseEqual()
        {
            return AreMemberWiseEqual(left, right);
        }

        private bool AreMemberWiseEqual(object leftHand, object rightHand)
        {
            var equal = VerifyPrimitives(leftHand, rightHand);
            equal = equal && ApplyOneToOneRelations(leftHand, rightHand);
            equal = equal && ApplyOneToManyRelations(leftHand, rightHand);
            return equal;
        }

        private bool VerifyPrimitives(object leftHand, object rightHand)
        {
            var equal = true;
            foreach (var propertyInfo in leftHand.GetType().GetProperties(MyBinding.Flags).Where(x => !StuffToIgnore.Contains(x)))
            {
                if (IsAPrimitive(propertyInfo.PropertyType))
                {
                    equal = equal && VerifyEquality(propertyInfo, leftHand, rightHand, RunReport);
                }
            }
            return equal;
        }

        private bool ApplyOneToManyRelations<TLeft, TRight>(TLeft leftHand, TRight rightHand)
        {
            return ApplyOneToManyRelations(leftHand, rightHand, oneToManyRelations);
        }

        private bool ApplyOneToManyRelations<TLeft, TRight>(TLeft leftHand, TRight rightHand, IEnumerable<OneToManyRelation> oneToManies)
        {
            var relations = oneToManies.Where(r => r.One.IsInstanceOfType(leftHand)).ToList();
            return relations.Aggregate(true, (current, relation) => current && ApplyOneToManyRelation(leftHand, rightHand, relation));
        }

        private bool ApplyOneToManyRelation<TLeft, TRight>(TLeft leftHand, TRight rightHand, OneToManyRelation relation)
        {
            var equal = true;
            var lefts = relation.GetChildren(leftHand).ToList();
            var rights = relation.GetChildren(rightHand).ToList();
            var ix = 0;
            if (lefts.Count != rights.Count)
                return false;
            if (IsAPrimitive(relation.Many))
            {
                foreach (var leftMany in lefts)
                {
                    var rightMany = rights.ElementAt(ix);
                    equal = equal && Equals(leftMany, rightMany);
                    ix++;
                }
            }
            else
            {
                foreach (var leftMany in lefts)
                {
                    var rightMany = rights.ElementAt(ix);
                    equal = equal && AreMemberWiseEqual(leftMany, rightMany);
                    ix++;
                }
            }
            return equal;
        }

        private bool ApplyOneToOneRelations<TLeft, TRight>(TLeft leftHand, TRight rightHand)
        {
            return ApplyOneToOneRelations(leftHand, rightHand, oneToOneRelations);
        }

        private bool ApplyOneToOneRelations<TLeft, TRight>(TLeft leftHand, TRight rightHand, IEnumerable<OneToOneRelation> oneToManies)
        {
            var relations = oneToManies.Where(r => r.Left.IsInstanceOfType(leftHand)).ToList();
            return relations.Aggregate(true, (current, relation) => current && ApplyOneToOneRelation(leftHand, rightHand, relation));
        }

        private bool ApplyOneToOneRelation<TLeft, TRight>(TLeft leftHand, TRight rightHand, OneToOneRelation relation)
        {
            var leftValue = relation.GetRight(leftHand);
            var rightValue = relation.GetRight(rightHand);
            return AreMemberWiseEqual(leftValue, rightValue);
        }

        private static bool VerifyEquality(PropertyInfo src, object leftHand, object rightHand, Action<bool, string> runReport)
        {
            var leftValue = src.GetValue(leftHand, null);
            var rightValue = src.GetValue(rightHand, null);

            var equal = Equals(leftValue, rightValue);
            var message = string.Format(
                "{2}{0}.{1}{2}  Expected : {3}.{2}  Actual : {4}.{2}",
                leftHand.GetType().Name,
                src.Name,
                Environment.NewLine,
                leftValue,
                rightValue);

            runReport(equal, message);

            return equal;
        }

        

        private bool IsAPrimitive(Type type)
        {
            if (type.Namespace == null)
                return false;

            if (type.Namespace.StartsWith("System.Collections"))
                return false;

            if (!type.Namespace.StartsWith("System"))
                return false;

            if (type.BaseType == typeof(Array))
                return false;

            return true;
        }

        public InspectImplementation<T> Report(Action<bool, string> report)
        {
            RunReport = report;
            return this;
        }

        public InspectImplementation<T> Inspect<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> expression)
        {
            var compiledExpression = expression.Compile();
            oneToManyRelations.Add(
                new OneToManyRelation
                {
                    One = typeof(TOne),
                    Many = typeof(TMany),
                    GetChildren = x => compiledExpression.Invoke((TOne)(x)).Cast<object>()
                });
            return this;
        }

        public InspectImplementation<T> Inspect<TLeft, TRight>(Expression<Func<TLeft, TRight>> expression)
        {
            var compiledExpression = expression.Compile();
            oneToOneRelations.Add(
                new OneToOneRelation
                {
                    Left = typeof(TLeft),
                    Right = typeof(TRight),
                    GetRight = x => compiledExpression.Invoke((TLeft)(x))
                });
            return this;
        }
    }
}