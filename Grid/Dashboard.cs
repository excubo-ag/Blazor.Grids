using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
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
            // if Columns is not 0, the user wants to set the column count as a parameter, not as an explicitly managed child. We therefore need to insert the columns here
            if (ColumnCount != 0)
            {
                RenderFragment columns_and_childcontent = (builder2) =>
                {
                    builder2.OpenComponent<Columns>(0);
                    builder2.AddAttribute(1, nameof(Columns.Count), ColumnCount);
                    builder2.AddAttribute(2, nameof(Columns.Width), "1fr");
                    builder2.CloseComponent();
                    builder2.AddContent(3, ChildContent);
                };
                builder.AddAttribute(5, nameof(Grid.ChildContent), columns_and_childcontent);
            }
            else
            {
                builder.AddAttribute(5, nameof(Grid.ChildContent), ChildContent);
            }
            builder.AddAttribute(6, nameof(Grid.ColumnGap), ColumnGap);
            builder.AddAttribute(7, nameof(Grid.RowGap), RowGap);
            builder.AddAttribute(8, nameof(Grid.PreventOverlaps), true);
            if (OnResize != null)
            {
                builder.AddAttribute(9, nameof(Grid.OnResize), OnResize);
            }
            if (OnMove != null)
            {
                builder.AddAttribute(10, nameof(Grid.OnMove), OnMove);
            }
            if (AdditionalAttributes != null)
            {
                builder.AddMultipleAttributes(11, AdditionalAttributes);
            }
            builder.CloseComponent();
        }
        #region API
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        /// <summary>
        /// Define all rows, columns and elements here:
        /// <br/>
        /// &lt;Dashboard&gt;<br/>
        /// &lt;Columns Count="n"&gt;/<br/>
        /// &lt;Element Column="1"&gt;content...&lt;/Element Column="1"&gt;<br/>
        /// &lt;/Dashboard&gt;<br/>
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
        /// <summary>
        /// Callback for when an element is moved.
        /// </summary>
        [Parameter] public Action<Element> OnMove { get; set; }
        /// <summary>
        /// Callback for when an element is resized.
        /// </summary>
        [Parameter] public Action<Element> OnResize { get; set; }
        /// <summary>
        /// The number of columns in the dashboard. If unspecified, you also have to explicitely manage the columns, e.g. by adding &lt;Columns Count="4" Width="1fr"/&gt; to the dashboard.
        /// </summary>
        [Parameter] public int ColumnCount { get; set; }
        #endregion

    }
}
