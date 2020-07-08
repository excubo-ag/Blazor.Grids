using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Excubo.Blazor.Grids
{
    public class Columns : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            for (int i = 0; i < Count; ++i)
            {
                builder.OpenComponent<ColumnDefinition>(0);
                builder.SetKey(i);
                builder.AddAttribute(1, nameof(ColumnDefinition.Width), Width);
                builder.CloseComponent();
            }
        }
        [CascadingParameter] public Grid Grid { get; set; }
        [Parameter] public string Width { get; set; } = "auto";
        [Parameter] public int Count { get; set; }
    }
}
