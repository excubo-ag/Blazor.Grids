using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids
{
    public partial class Grid
    {
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
        /// <summary>
        /// If enabled, the Grid manages the number of rows automatically. This is useful for the dashboard scenario where elements can be moved down as much as wanted, and the Grid just adds rows.
        /// </summary>
        [Parameter] public bool AutoRows { get; set; }
        #endregion
        #region internal API
        internal TitleSettings TitleSettings { get; set; } = new TitleSettings();
        internal MoveResizeOverlaySettings MoveResizeOverlaySettings { get; set; } = new MoveResizeOverlaySettings();
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
            UpdateRows();
        }
        internal void UpdateRows()
        {
            // if we have "auto-rows", then see whether we have the right amount of rows
            if (AutoRows && elements.Any())
            {
                var max_row = elements.Max(e => e.Row + Math.Max(1, e.RowSpan) - 1);
                var delta = max_row - row_definitions.Count;
                if (delta > 0)
                {
                    row_definitions.AddRange(Enumerable.Range(0, delta).Select(_ => new RowDefinition()));
                }
                else if (delta < 0)
                {
                    row_definitions.RemoveRange(max_row, -delta);
                }
                if (delta != 0)
                {
                    ReRenderSelfButNoChild();
                }
            }
        }
        #endregion
        private object additional_style => AdditionalAttributes == null || !AdditionalAttributes.ContainsKey("style") ? null : AdditionalAttributes["style"];
        private IEnumerable<KeyValuePair<string, object>> additional_attribues_without_style => AdditionalAttributes?.Where(kv => kv.Key != "style");
        private readonly List<RowDefinition> row_definitions = new List<RowDefinition>();
        private readonly List<ColumnDefinition> column_definitions = new List<ColumnDefinition>();
        private readonly List<Element> elements = new List<Element>();
        private void ReRenderSelfButNoChild()
        {
            foreach (var element in elements)
            {
                element.render_required = false;
            }
            StateHasChanged();
        }
        internal void RenderNothingBut(Element element)
        {
            foreach (var e in elements)
            {
                e.render_required = (e == element);
            }
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
