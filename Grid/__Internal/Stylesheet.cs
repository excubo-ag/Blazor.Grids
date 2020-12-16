using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids.__Internal
{
    public class Stylesheet : ComponentBase
    {
        [Parameter] public string Src { get; set; }
        [Inject] IJSRuntime js { get; set; }
        private bool render_required = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Src != null)
            {
                var condition = $"document.head.querySelector(`[href='{Src}']`) == null";
                var action = $"let s = document.createElement('link'); s.setAttribute('rel', 'stylesheet'); s.setAttribute('href', '{Src}'); document.head.appendChild(s);";
                await js.InvokeVoidAsync("eval", $"if ({condition}) {{ {action} }}");
                render_required = false;
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        protected override bool ShouldRender() => render_required;
    }
}