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
        private async Task MoveElementAsync(MouseEventArgs e)
        {
            if (start_position == null)
            {
                (grid_position.Left, grid_position.Top) = await js.GetPositionAsync(Element.Grid.Area);
                (element_position.Left, element_position.Top) = await js.GetPositionAsync(Element.Area);
                (element_dimension.Width, element_dimension.Height) = await js.GetDimensionAsync(Element.Area);
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
                var column_width = element_dimension.Width / Math.Max(1, Element.ColumnSpan);
                var row_height = element_dimension.Height / Math.Max(1, Element.RowSpan);
                var width_ratio = (e.ClientX - start_position.Value.X) / column_width;
                var height_ratio = (e.ClientY - start_position.Value.Y) / row_height;
                var (height_increase, height_decrease, width_increase, width_decrease) = (width_ratio, height_ratio).GetRequiredChanges(stronger_threshold: 0.9, weaker_threshold: 0.5);
                if (height_increase)
                {
                    Element.MoveDown();
                }
                else if (height_decrease)
                {
                    Element.MoveUp();
                }
                if (width_increase)
                {
                    Element.MoveRight();
                }
                else if (width_decrease)
                {
                    Element.MoveLeft();
                }
                if (height_decrease || height_increase || width_increase || width_decrease)
                {
                    start_position = null;
                }
            }
        }
        [Inject] private IJSRuntime js { get; set; }
    }
}
