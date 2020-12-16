using System;
using System.Threading.Tasks;

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
        public static async Task If(this Func<Task> task, bool condition)
        {
            if (condition)
            {
                await task.Invoke();
            }
        }
    }
}