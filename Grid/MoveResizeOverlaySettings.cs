using Microsoft.AspNetCore.Components;

namespace Excubo.Blazor.Grids
{
    public class MoveResizeOverlaySettings : ComponentBase
    {
        [CascadingParameter] public Grid Grid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Parameter] public Color Color { get; set; } = (0x19, 0xC5, 0xE3);
        /// <summary>
        /// Opacity of the overlay
        /// </summary>
        [Parameter] public int Opacity { get; set; } = 0x88;
        internal string ColorString => "#" + Color.R.ToString("X2") + Color.G.ToString("X2") + Color.B.ToString("X2") + Opacity.ToString("X2");
        protected override void OnParametersSet()
        {
            System.Diagnostics.Debug.Assert(Grid != null);
            Grid.MoveResizeOverlaySettings = this;
            base.OnParametersSet();
        }
        protected override bool ShouldRender() => false;
    }
}
