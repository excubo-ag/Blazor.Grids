using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids.__Internal
{
    public partial class Header
    {
        [CascadingParameter] public Element Element { get; set; }
        [Parameter] public int? HeadingLevel { get; set; } = 4;
        private bool render_required = true;
        protected override bool ShouldRender()
        {
            if (!render_required)
            {
                render_required = true;
                return false;
            }
            return base.ShouldRender();
        }
        internal async Task MovingAsync(MouseEventArgs e)
        {
            render_required = false;
            if (e.Buttons == 1)
            {
                await MoveElementAsync(e);
            }
            else
            {
                start_position = null;
                Element.Grid.MovingIndicatorOverlay.Release();
            }
        }
        private async Task MouseDownAsync(MouseEventArgs e)
        {
            render_required = false;
            if (e.Buttons == 1)
            {
                await MoveElementAsync(e);
            }
        }
        internal Task MouseUpAsync(MouseEventArgs e)
        {
            render_required = false;
            start_position = null;
            Element.Grid.MovingIndicatorOverlay.Release();
            return Task.CompletedTask;
        }
        private (double X, double Y)? start_position;
        private (double Left, double Top) grid_position;
        private (double Left, double Top) element_position;
        private (double Width, double Height) element_dimension;
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
        private async Task MoveElementAsync(MouseEventArgs e)
        {
            if (start_position == null)
            {
                (grid_position.Left, grid_position.Top) = await js.InvokeAsync<Position>("eval", $"let e = document.querySelector('[_bl_{Element.Grid.Area.Id}=\"\"]'); let r = {{ 'Left': e.offsetLeft, 'Top': e.offsetTop }}; r");
                (element_position.Left, element_position.Top) = await js.InvokeAsync<Position>("eval", $"let e = document.querySelector('[_bl_{Element.Area.Id}=\"\"]'); let r = {{ 'Left': e.offsetLeft, 'Top': e.offsetTop }}; r");
                (element_dimension.Width, element_dimension.Height) = await js.InvokeAsync<Dimension>("eval", $"let e = document.querySelector('[_bl_{Element.Area.Id}=\"\"]'); let r = {{ 'Width': e.clientWidth, 'Height': e.clientHeight }}; r");
                start_position = (X: e.ClientX, Y: e.ClientY); // setting this last assures that all other values are already set, which is important in the else branch.
                Element.Grid.MovingIndicatorOverlay.SetSize(element_dimension.Width, element_dimension.Height);
                Element.Grid.MovingIndicatorOverlay.Set(this);
                Element.Grid.MovingIndicatorOverlay.SetPosition(element_position.Left, element_position.Top);
            }
            else
            {
                var overlay_x = element_position.Left + e.ClientX - start_position.Value.X;
                var overlay_y = element_position.Top + e.ClientY - start_position.Value.Y;
                Element.Grid.MovingIndicatorOverlay.SetPosition(overlay_x, overlay_y);
                if (overlay_x > 2 + element_position.Left + element_dimension.Width / Math.Max(1, Element.ColumnSpan))
                {
                    Element.MoveRight();
                    start_position = null;
                }
                if (overlay_y > 2 + element_position.Top + element_dimension.Height / Math.Max(1, Element.RowSpan))
                {
                    Element.MoveDown();
                    start_position = null;
                }
                if (overlay_x + element_dimension.Width / 2.0 / Math.Max(1, Element.ColumnSpan) < element_position.Left)
                {
                    Element.MoveLeft();
                    start_position = null;
                }
                if (overlay_y + element_dimension.Height / 2.0 / Math.Max(1, Element.RowSpan) < element_position.Top)
                {
                    Element.MoveUp();
                    start_position = null;
                }
            }
        }
        [Inject] private IJSRuntime js { get; set; }
    }
}
