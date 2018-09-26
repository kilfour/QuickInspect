namespace QuickInspect
{
    public static class Inspect
    {
        public static InspectImplementation<T> This<T>(T left, T right)
        {
            return new InspectImplementation<T>(left, right);
        }

        public static NavigatorImplementation<T> This<T>(T left)
        {
            return new NavigatorImplementation<T>(left);
        }
    }
}