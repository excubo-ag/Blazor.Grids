using Microsoft.AspNetCore.Components;

namespace Excubo.Blazor.Grids
{
    public class TitleSettings : ComponentBase
    {
        [CascadingParameter] public Grid Grid { get; set; }
        /// <summary>
        /// Specifies which level the title for each element should be. Allowed values are 1 to 6. Default value is 2.
        /// </summary>
        [Parameter] public int HeadingLevel { get; set; } = 2;
        /// <summary>
        /// Specifies how titles for each element should be aligned with the element. Default value is Center.
        /// </summary>
        [Parameter] public Alignment Alignment { get; set; } = Alignment.Center;
        protected override void OnParametersSet()
        {
            System.Diagnostics.Debug.Assert(Grid != null);
            Grid.TitleSettings = this;
            base.OnParametersSet();
        }
        protected override bool ShouldRender() => false;
    }
}