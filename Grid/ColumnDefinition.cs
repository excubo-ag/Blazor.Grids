using Microsoft.AspNetCore.Components;

namespace Excubo.Blazor.Grids
{
    public class ColumnDefinition : ComponentBase
    {
        [CascadingParameter] public Grid Grid { get; set; }
        [Parameter] public string Width { get; set; } = "auto";
        private int? column_index;
        protected override void OnParametersSet()
        {
            if (column_index == null)
            {
                System.Diagnostics.Debug.Assert(Grid != null);
                column_index = Grid.Add(this);
            }
            base.OnParametersSet();
        }
        protected override bool ShouldRender() => false;
    }
}