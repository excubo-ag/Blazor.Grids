using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids.__Internal
{
    public partial class ResizeHandle
    {
        [CascadingParameter] public Element Element { get; set; }
        private bool render_required = true;
        private async Task MouseDownAsync(MouseEventArgs e)
        {
            render_required = false;
            if (e.Buttons == 1)
            {
                await ResizeAsync(e);
            }
        }
        internal Task MouseUpAsync(MouseEventArgs e)
        {
            render_required = false;
            start_position = null;
            Element.Grid.MovingIndicatorOverlay.Release();
            return Task.CompletedTask;
        }
        internal async Task MouseMoveAsync(MouseEventArgs e)
        {
            render_required = false;
            if (e.Buttons == 1)
            {
                await ResizeAsync(e);
            }
            else
            {
                start_position = null;
                Element.Grid.MovingIndicatorOverlay.Release();
            }
        }
        private (double X, double Y)? start_position;
        private (double Left, double Top) grid_position;
        private (double Left, double Top) element_position;
        private (double Width, double Height) element_dimension;
        private async Task ResizeAsync(MouseEventArgs e)
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
                Resize(e);
            }
        }
        private void Resize(MouseEventArgs e)
        {
            var overlay_x = element_dimension.Width + e.ClientX - start_position.Value.X;
            var overlay_y = element_dimension.Height + e.ClientY - start_position.Value.Y;
            Element.Grid.MovingIndicatorOverlay.SetSize(overlay_x, overlay_y);
            var column_width = element_dimension.Width / Math.Max(1, Element.ColumnSpan);
            var row_height = element_dimension.Height / Math.Max(1, Element.RowSpan);
            var h_ratio = (overlay_x - element_dimension.Width) / column_width;
            var v_ratio = (overlay_y - element_dimension.Height) / row_height;
            if (h_ratio > 7.0 / 8)
            {
                if (v_ratio > .33)
                {
                    // the user probably wants to increase the RowSpan at the same time
                    Element.IncreaseHeight();
                }
                else if (v_ratio < .66)
                {
                    // the user probably wants to decrease the RowSpan at the same time
                    Element.DecreaseHeight();
                }
                Element.IncreaseWidth();
                start_position = null;
            }
            if (v_ratio > 7.0 / 8)
            {
                if (h_ratio > .33)
                {
                    // the user probably wants to increase the ColumnSpan at the same time
                    Element.IncreaseWidth();
                }
                else if (h_ratio < .66)
                {
                    // the user probably wants to decrease the ColumnSpan at the same time
                    Element.DecreaseWidth();
                }
                Element.IncreaseHeight();
                start_position = null;
            }
            if (h_ratio < -7.0 / 8)
            {
                if (v_ratio > .33)
                {
                    // the user probably wants to increase the RowSpan at the same time
                    Element.IncreaseHeight();
                }
                else if (v_ratio < .66)
                {
                    // the user probably wants to decrease the RowSpan at the same time
                    Element.DecreaseHeight();
                }
                Element.DecreaseWidth();
                start_position = null;
            }
            if (v_ratio < -7.0 / 8)
            {
                if (h_ratio > .33)
                {
                    // the user probably wants to increase the ColumnSpan at the same time
                    Element.IncreaseWidth();
                }
                else if (h_ratio < .66)
                {
                    // the user probably wants to decrease the ColumnSpan at the same time
                    Element.DecreaseWidth();
                }
                Element.DecreaseHeight();
                start_position = null;
            }
        }
        protected override bool ShouldRender()
        {
            if (!render_required)
            {
                render_required = true;
                return false;
            }
            return base.ShouldRender();
        }
        [Inject] private IJSRuntime js { get; set; }
    }
}
