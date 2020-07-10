using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;

namespace Excubo.Blazor.Grids
{
    public class Dashboard : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<Grid>(0);
            builder.AddAttribute(1, nameof(Grid.AllowMove), true);
            builder.AddAttribute(2, nameof(Grid.AllowResize), true);
            builder.AddAttribute(3, nameof(Grid.AspectRatio), AspectRatio);
            builder.AddAttribute(4, nameof(Grid.AutoRows), true);
            builder.AddAttribute(5, nameof(Grid.ChildContent), ChildContent);
            builder.AddAttribute(6, nameof(Grid.ColumnGap), ColumnGap);
            builder.AddAttribute(7, nameof(Grid.RowGap), RowGap);
            if (AdditionalAttributes != null)
            {
                builder.AddMultipleAttributes(8, AdditionalAttributes);
            }
            builder.CloseComponent();
        }
        #region API
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        /// <summary>
        /// Define all rows, columns and elements here:
        /// <br/>
        /// &lt;Grid&gt;<br/>
        /// &lt;RowDefinition Height="auto"/&gt;<br/>
        /// &lt;ColumnDefinition Width="1fr"/&gt;<br/>
        /// &lt;ColumnDefinition Width="1fr"/&gt;<br/>
        /// &lt;Element Column="1"&gt;content...&lt;/Element Column="1"&gt;<br/>
        /// &lt;/Grid&gt;<br/>
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
        /// <summary>
        /// Optional aspect ratio of grid tiles. Default value is 1.
        /// <br/>
        /// AspectRatio = Width/Height
        /// <br/>
        /// Examples:
        /// An aspect ratio of 1 results in square tiles.
        /// An aspect ratio of 2 result in tiles that are twice as wide as high.
        /// An aspect ratio of 0.5 result in tiles that are twice as high as wide.
        /// </summary>
        [Parameter] public double AspectRatio { get; set; } = 1.0;
        /// <summary>
        /// The gap between rows.
        /// </summary>
        [Parameter] public string RowGap { get; set; } = "inherit";
        /// <summary>
        /// The gap between columns.
        /// </summary>
        [Parameter] public string ColumnGap { get; set; } = "inherit";
        #endregion

    }
}
