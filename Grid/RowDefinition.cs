using Microsoft.AspNetCore.Components;

namespace Excubo.Blazor.Grids
{
    public class RowDefinition : ComponentBase
    {
        [CascadingParameter] public Grid Grid { get; set; }
        [Parameter] public string Height { get; set; } = "auto";
        private int? row_index;
        protected override void OnParametersSet()
        {
            if (row_index == null)
            {
                System.Diagnostics.Debug.Assert(Grid != null);
                row_index = Grid.Add(this);
            }
            base.OnParametersSet();
        }
        protected override bool ShouldRender() => false;
    }
}
