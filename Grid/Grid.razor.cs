﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

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
        /// Allows you to override the default header and render your own content.
        /// </summary>
        /// <remarks>
        /// If you override the header the header level and alignment attributes will have no effect.
        /// 
        /// The context is the Title you have set on this element.
        /// </remarks>
        [Parameter] public RenderFragment<string> Header { get; set; }
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

        private bool allowResize = false;

        /// <summary>
        /// Whether to allow resizing elements or not. Default is false.
        /// Note that this feature is only supported when an AspectRatio is set, otherwise this flag has no effect.
        /// </summary>
        [Parameter] public bool AllowResize 
        {
            get { return allowResize; }
            set
            {
                if(allowResize != value)
                {
                    allowResize = value;
                    StateHasChanged();
                }
            }
        }

        private bool allowMove = false;

        /// <summary>
        /// Whether to allow moving elements or not. Default is false.
        /// Note that this feature is only supported when an AspectRatio is set, otherwise this flag has no effect.
        /// Important: elements are movable by dragging the title bar. Hence, without setting a title for your element, your element won't have a title bar and you won't be able to move them.
        /// </summary>
        [Parameter] public bool AllowMove
        {
            get { return allowMove; }
            set
            {
                if (allowMove != value)
                {
                    allowMove = value;
                    StateHasChanged();
                }
            }
        }

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
        /// <summary>
        /// Callback for when an element is moved.
        /// </summary>
        [Parameter] public Action<ElementMoveArgs> OnMove { get; set; }
        /// <summary>
        /// Callback for when an element is resized.
        /// </summary>
        [Parameter] public Action<ElementResizeArgs> OnResize { get; set; }
        /// <summary>
        /// If enabled, moving or resizing an element will make sure that other elements flow away to make room for the moved/resized element.
        /// </summary>
        [Parameter] public bool PreventOverlaps { get; set; }
        /// <summary>
        /// To control CSS classes of all elements uniformily, assign the classes to this property. Classes defined on the Element directly take precedence.
        /// </summary>
        [Parameter] public string ElementClass { get; set; }
        /// <summary>
        /// To control styles of all elements uniformily, assign the styles to this property. Styles defined on the Element directly take precedence.
        /// </summary>
        [Parameter] public string ElementStyle { get; set; }
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
        internal void Add(Area area)
        {
            if (!areas.Contains(area))
            {
                areas.Add(area);
            }
        }
        private string AreaString
        {
            get
            {
                return string.Join(" ", Enumerable.Range(0, row_definitions.Count).Select(r =>
                {
                    var row = string.Join(" ", Enumerable.Range(0, column_definitions.Count).Select(c =>
                    {
                        var match = areas.FirstOrDefault(a => a.Row <= r && r < a.Row + a.ActualRowSpan && a.Column <= c && c < a.Column + a.ActualColumnSpan);
                        return match?.Name ?? ".";
                    }));
                    return "'" + row + "'";
                }));
            }
        }
        internal void UpdateArea()
        {
            ReRenderSelfButNoChild();
        }
        internal void Add(Element element)
        {
            if (!elements.Contains(element))
            {
                elements.Add(element);
            }
        }
        #endregion
        private object additional_style => AdditionalAttributes == null || !AdditionalAttributes.ContainsKey("style") ? null : AdditionalAttributes["style"];
        private IEnumerable<KeyValuePair<string, object>> additional_attribues_without_style => AdditionalAttributes?.Where(kv => kv.Key != "style");
        private readonly List<RowDefinition> row_definitions = new List<RowDefinition>();
        private readonly List<ColumnDefinition> column_definitions = new List<ColumnDefinition>();
        private readonly List<Area> areas = new List<Area>();
        private readonly List<Element> elements = new List<Element>();
        private void ReRenderSelfButNoChild()
        {
            foreach (var element in elements)
            {
                element.render_required = false;
            }
            StateHasChanged();
        }
        private readonly List<Element> fixed_elements = new List<Element>();
        internal async Task ResolveOverlapsAsync(Element fixed_element, (int Row, int Col) push_to)
        {
            if (!PreventOverlaps)
            {
                return;
            }
            fixed_elements.Add(fixed_element);
            while (true)
            {
                // we search for an overlapping element. There might be many, but if we move one, that might cause even more conflicts.
                // Those are resolved before we could move the next, so we need to start the search for overlaps again.
                var overlapping_element = elements.Except(fixed_elements).Where(e => e.OverlapsWith(fixed_element)).FirstOrDefault();
                if (overlapping_element == null)
                {
                    break; // there are no more overlapping elements
                }
                await FindNewPlaceAsync(overlapping_element, push_to);
            }
            fixed_elements.Remove(fixed_element);
        }
        private async Task FindNewPlaceAsync(Element element, (int Row, int Col) push_to)
        {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            var pseudo_element = new Element { ActualRow = element.ActualRow, ActualColumn = element.ActualColumn, ActualRowSpan = element.ActualRowSpan, ActualColumnSpan = element.ActualColumnSpan };
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
            // we try to move the element as little as possible. we therefore start searching around the original Row/Column and expand our "radius" from there.
            // we traverse the possible positions in the following way (* denotes original position):
            //
            //    456
            //    3*7
            //    218
            //   ...9
            //
            // the walking pattern therefore is d l u u r r d d d l l l, ...
            // that pattern is always repeating the count of how often to walk in a certain direction twice [d l] [uu rr] [ddd lll] [uuuu rrrr] ....
            (int Row, int Col) direction = push_to;
            for (int count = 1; count < 8; ++count)
            {
                for (int r = 0; r < 2; ++r)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        // walk into the direction
                        pseudo_element.ActualRow += direction.Row;
                        pseudo_element.ActualColumn += direction.Col;
                        // see whether this is a legal position
                        if (pseudo_element.ActualRow < 0 || pseudo_element.ActualColumn < 0 || pseudo_element.ActualColumn + pseudo_element.ActualColumnSpan >= column_definitions.Count)
                        {
                            continue;
                        }
                        // since it is, let's see whether there is any overlap with the other elements:
                        if (elements.Where(e => e != element).All(e => !pseudo_element.OverlapsWith(e)))
                        {
                            // we found a good new place! let's move the actual element and quit.
                            await element.MoveToAsync(pseudo_element.ActualRow, pseudo_element.ActualColumn);
                            return;
                        }
                    }
                    direction = direction switch { (-1, 0) => (0, 1), (0, 1) => (1, 0), (1, 0) => (0, -1), (0, -1) => (-1, 0) }; // change direction
                }
            }
            // fallback:
            await element.MoveDownAsync();
        }

        private readonly List<Element> render_elements = new List<Element>();
        internal void RenderNothingBut(Element element)
        {
            if (!render_elements.Any())
            {
                elements.ForEach(e => e.render_required = false);
            }
            render_elements.Add(element);
            element.render_required = true;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                foreach (var element in elements)
                {
                    await ResolveOverlapsAsync(element, (1, 0));
                }
            }
            render_elements.Clear();
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}