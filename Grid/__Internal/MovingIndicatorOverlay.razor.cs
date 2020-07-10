using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids.__Internal
{
    public partial class MovingIndicatorOverlay
    {
        private double X { get; set; }
        private double Y { get; set; }
        private double Width { get; set; }
        private double Height { get; set; }
        internal void SetPosition(double x, double y)
        {
            (X, Y) = (x, y);
            StateHasChanged();
        }
        private Header Header { get; set; }
        private ResizeHandle ResizeHandle { get; set; }
        internal void Release()
        {
            Header = null;
            ResizeHandle = null;
            StateHasChanged();
        }
        internal void Set(Header header)
        {
            Header = header;
            ResizeHandle = null;
            StateHasChanged();
        }
        internal void Set(ResizeHandle resize_handle)
        {
            Header = null;
            ResizeHandle = resize_handle;
            StateHasChanged();
        }
        internal void SetSize(double width, double height)
        {
            (Width, Height) = (width, height);
            StateHasChanged();
        }
        private bool render_required = true;
        private Task MovingAsync(MouseEventArgs e)
        {
            render_required = false;
            if (Header != null)
            {
                return Header.MovingAsync(e);
            }
            else if (ResizeHandle != null)
            {
                return ResizeHandle.MouseMoveAsync(e);
            }
            return Task.CompletedTask;
        }
        private Task MouseUpAsync(MouseEventArgs e)
        {
            render_required = false;
            if (Header != null)
            {
                return Header.MouseUpAsync(e);
            }
            else if (ResizeHandle != null)
            {
                return ResizeHandle.MouseUpAsync(e);
            }
            return Task.CompletedTask;
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
    }
}
