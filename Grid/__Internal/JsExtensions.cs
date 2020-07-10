using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids.__Internal
{
    internal static class JsExtensions
    {
        private class Position
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public void Deconstruct(out double left, out double top) { left = Left; top = Top; }
        }
        private class Dimension
        {
            public double Width { get; set; }
            public double Height { get; set; }
            public void Deconstruct(out double width, out double height) { width = Width; height = Height; }
        }
        public static async ValueTask<(double Left, double Top)> GetPositionAsync(this IJSRuntime js, ElementReference element)
        {
            var (left, top) = await js.InvokeAsync<Position>("eval", $"let e = document.querySelector('[_bl_{element.Id}=\"\"]'); let r = {{ 'Left': e.offsetLeft, 'Top': e.offsetTop }}; r");
            return (left, top);
        }
        public static async ValueTask<(double Width, double Height)> GetDimensionAsync(this IJSRuntime js, ElementReference element)
        {
            var (width, height) = await js.InvokeAsync<Dimension>("eval", $"let e = document.querySelector('[_bl_{element.Id}=\"\"]'); let r = {{ 'Width': e.clientWidth, 'Height': e.clientHeight }}; r");
            return (width, height);
        }
    }
}
