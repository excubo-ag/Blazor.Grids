using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Excubo.Blazor.Grids
{
    public class Rows : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            for (int i = 0; i < Count; ++i)
            {
                builder.OpenComponent<RowDefinition>(0);
                builder.SetKey(i);
                builder.AddAttribute(1, nameof(RowDefinition.Height), Height);
                builder.CloseComponent();
            }
        }
        [Parameter] public string Height { get; set; } = "auto";
        [Parameter] public int Count { get; set; }
    }
}