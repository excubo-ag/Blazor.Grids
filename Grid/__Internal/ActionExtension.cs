using System;

namespace Excubo.Blazor.Grids.__Internal
{
    internal static class ActionExtension
    {
        public static void If(this Action action, bool condition)
        {
            if (condition)
            {
                action.Invoke();
            }
        }
    }
}
