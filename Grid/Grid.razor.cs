using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Excubo.Blazor.Grids
{
    public partial class Grid
    {
        private object additional_style => AdditionalAttributes == null || !AdditionalAttributes.ContainsKey("style") ? null : AdditionalAttributes["style"];
        private IEnumerable<KeyValuePair<string, object>> additional_attribues_without_style => AdditionalAttributes?.Where(kv => kv.Key != "style");
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
        /// Optional aspect ratio of grid tiles
        /// <br/>
        /// AspectRatio = Width/Height
        /// <br/>
        /// Examples:
        /// An aspect ratio of 1 results in square tiles.
        /// An aspect ratio of 2 result in tiles that are twice as wide as high.
        /// An aspect ratio of 0.5 result in tiles that are twice as high as wide.
        /// </summary>
        [Parameter] public double? AspectRatio { get; set; }
        /// <summary>
        /// Whether to allow resizing elements or not. Default is false.
        /// Note that this feature is only supported when an AspectRatio is set, otherwise this flag has no effect.
        /// </summary>
        [Parameter] public bool AllowResize { get; set; }
        /// <summary>
        /// Whether to allow moving elements or not. Default is false.
        /// Note that this feature is only supported when an AspectRatio is set, otherwise this flag has no effect.
        /// Important: elements are movable by dragging the title bar. Hence, without setting a title for your element, your element won't have a title bar and you won't be able to move them.
        /// </summary>
        [Parameter] public bool AllowMove { get; set; }
        /// <summary>
        /// The gap between rows.
        /// </summary>
        [Parameter] public string RowGap { get; set; } = "inherit";
        /// <summary>
        /// The gap between columns.
        /// </summary>
        [Parameter] public string ColumnGap { get; set; } = "inherit";
        private List<RowDefinition> row_definitions = new List<RowDefinition>();
        private List<ColumnDefinition> column_definitions = new List<ColumnDefinition>();
        private List<Element> elements = new List<Element>();
        internal int Add(RowDefinition row_definition)
        {
            var index = row_definitions.Count;
            row_definitions.Add(row_definition);
            ReRenderSelfButNoChild();
            return index;
        }
        internal int Add(ColumnDefinition column_definition)
        {
            var index = column_definitions.Count;
            column_definitions.Add(column_definition);
            ReRenderSelfButNoChild();
            return index;
        }
        internal void Add(Element element)
        {
            elements.Add(element);
        }
        private void ReRenderSelfButNoChild()
        {
            foreach (var element in elements)
            {
                element.render_required = false;
            }
            StateHasChanged();
        }
    }
}
