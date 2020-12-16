using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Excubo.Blazor.Grids
{
    public struct ElementMoveArgs
    {
        public ElementMoveArgs(Element element, int new_row, int new_column)
        {
            Element = element;
            NewRow = new_row;
            NewColumn = new_column;
        }
        public Element Element { get; }
        public int NewRow { get; }
        public int NewColumn { get; }
    }
    public struct ElementResizeArgs
    {
        public ElementResizeArgs(Element element, int new_row_span, int new_column_span)
        {
            Element = element;
            NewRowSpan = new_row_span;
            NewColumnSpan = new_column_span;
        }
        public Element Element { get; }
        public int NewRowSpan { get; }
        public int NewColumnSpan { get; }
    }
    public partial class Element
    {
        [CascadingParameter] public Grid Grid { get; set; }
        #region API
        /// <summary>
        /// Any content you would like to display in the element
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
        /// <summary>
        /// The column that this element should be in. In case the element spans multiple columns, this value indicates the column which contains the top left corner of the element.
        /// </summary>
        [Parameter] public int Column { get; set; }
        internal int ActualColumn { get; set; }
        [Parameter] public EventCallback<int> ColumnChanged { get; set; }
        /// <summary>
        /// The row that this element should be in. In case the element spans multiple rows, this value indicates the row which contains the top left corner of the element.
        /// </summary>
        [Parameter] public int Row { get; set; }
        internal int ActualRow { get; set; }
        [Parameter] public EventCallback<int> RowChanged { get; set; }
        /// <summary>
        /// The number of columns that this element should span.
        /// Any value less than 1 is interpreted as 1.
        /// </summary>
        [Parameter] public int ColumnSpan { get; set; }
        internal int ActualColumnSpan { get; set; }
        [Parameter] public EventCallback<int> ColumnSpanChanged { get; set; }
        /// <summary>
        /// The number of rows that this element should span.
        /// Any value less than 1 is interpreted as 1.
        /// </summary>
        [Parameter] public int RowSpan { get; set; }
        internal int ActualRowSpan { get; set; }
        [Parameter] public EventCallback<int> RowSpanChanged { get; set; }
        /// <summary>
        /// The title that should appear at the top of the element
        /// </summary>
        [Parameter] public string Title { get; set; }
        /// <summary>
        /// Callback for when the element was moved.
        /// </summary>
        [Parameter] public Action<ElementMoveArgs> OnMove { get; set; }
        /// <summary>
        /// Callback for when the element was resized.
        /// </summary>
        [Parameter] public Action<ElementResizeArgs> OnResize { get; set; }
        /// <summary>
        /// The style of the header. Only visible if a Title is present. Defaults to a thin, dark line at the bottom of the header.
        /// </summary>
        [Parameter] public string HeaderStyle { get; set; } = "border-bottom: 1px solid #202020;";
        /// <summary>
        /// The name of the area this element should be a part of.
        /// </summary>
        [Parameter] public string Area { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        private object additional_style => AdditionalAttributes != null && AdditionalAttributes.ContainsKey("style") ? AdditionalAttributes["style"] : null;
        private object additional_class => AdditionalAttributes != null && AdditionalAttributes.ContainsKey("class") ? AdditionalAttributes["class"] : null;
        /// <summary>
        /// Set the dimensions of the element
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public async Task ResizeAsync(int height, int width)
        {
            if (height <= 0)
            {
                throw new ArgumentException("Height of an element may not be zero or negative", nameof(height));
            }
            if (width <= 0)
            {
                throw new ArgumentException("Width of an element may not be zero or negative", nameof(width));
            }
            var higher = height > ActualRowSpan;
            var wider = width > ActualColumnSpan;
            await UpdateRowSpanAsync(height);
            await UpdateColumnSpanAsync(width);
            if (higher || wider)
            {
                var push_direction = higher ? (1, 0) : (0, 1); // push elements downwards or rightwards
                await Grid.ResolveOverlapsAsync(this, push_to: push_direction);
            }
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
        }
        /// <summary>
        /// Moves the element to the specified position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public async Task MoveToAsync(int row, int column)
        {
            if (row < 0)
            {
                throw new ArgumentException("Row of an element may not be negative", nameof(row));
            }
            if (column < 0)
            {
                throw new ArgumentException("Column of an element may not be negative", nameof(column));
            }
            await UpdateRowAsync(row);
            await UpdateColumnAsync(column);
            await Grid.ResolveOverlapsAsync(this, (-1, 0));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
        }
        #endregion
        private async Task UpdateRowAsync(int value)
        {
            if (ActualRow == value)
            {
                return;
            }
            ActualRow = value;
            if (RowChanged.HasDelegate)
            {
                await RowChanged.InvokeAsync(value);
            }
            else
            {
                Row = value;
            }
        }
        private async Task UpdateColumnAsync(int value)
        {
            if (ActualColumn == value)
            {
                return;
            }
            ActualColumn = value;
            if (ColumnChanged.HasDelegate)
            {
                await ColumnChanged.InvokeAsync(value);
            }
            else
            {
                Column = value;
            }
        }
        private async Task UpdateRowSpanAsync(int value)
        {
            value = Math.Max(1, value);
            if (ActualRowSpan == value)
            {
                return;
            }
            ActualRowSpan = value;
            if (RowSpanChanged.HasDelegate)
            {
                await RowSpanChanged.InvokeAsync(value);
            }
            else
            {
                RowSpan = value;
            }
        }
        private async Task UpdateColumnSpanAsync(int value)
        {
            value = Math.Max(1, value);
            if (ActualColumnSpan == value)
            {
                return;
            }
            ActualColumnSpan = value;
            if (ColumnSpanChanged.HasDelegate)
            {
                await ColumnSpanChanged.InvokeAsync(value);
            }
            else
            {
                ColumnSpan = value;
            }
        }
        private string area => string.IsNullOrEmpty(Area) ? null : $"grid-area: {Area};";
        private string column
        {
            get
            {
                if (!string.IsNullOrEmpty(Area))
                {
                    return null;
                }
                if (ActualColumn == 0)
                {
                    if (ActualColumnSpan < 2)
                    {
                        return "grid-column: 1;";
                    }
                    return $"grid-column: 1 / span {ActualColumnSpan};";
                }
                else
                {
                    if (ActualColumnSpan < 2)
                    {
                        return $"grid-column: {ActualColumn + 1};";
                    }
                    return $"grid-column: {ActualColumn + 1} / span {ActualColumnSpan};";
                }
            }
        }
        private string row
        {
            get
            {
                if (!string.IsNullOrEmpty(Area))
                {
                    return null;
                }
                if (ActualRow == 0)
                {
                    if (ActualRowSpan < 2)
                    {
                        return "grid-row: 1;";
                    }
                    return $"grid-row: 1 / span {ActualRowSpan};";
                }
                else
                {
                    if (ActualRowSpan < 2)
                    {
                        return $"grid-row: {ActualRow + 1};";
                    }
                    return $"grid-row: {ActualRow + 1} / span {ActualRowSpan};";
                }
            }
        }
        protected override void OnParametersSet()
        {
            System.Diagnostics.Debug.Assert(Grid != null);
            Grid.Add(this);
            base.OnParametersSet();
            ActualRow = Row;
            ActualColumn = Column;
            ActualRowSpan = Math.Max(1, RowSpan);
            ActualColumnSpan = Math.Max(1, ColumnSpan);
        }
        internal bool render_required { get; set; } = true;
        protected override bool ShouldRender()
        {
            if (!render_required)
            {
                render_required = true;
                return false;
            }
            return base.ShouldRender();
        }
        private void InvokeMoveEvents()
        {
            var event_args = new ElementMoveArgs(this, ActualRow, ActualColumn);
            OnMove?.Invoke(event_args);
            Grid.OnMove?.Invoke(event_args);
        }
        private void InvokeResizeEvents()
        {
            var event_args = new ElementResizeArgs(this, ActualRowSpan, ActualColumnSpan);
            OnResize?.Invoke(event_args);
            Grid.OnResize?.Invoke(event_args);
        }
        internal async Task MoveRightAsync()
        {
            await UpdateColumnAsync(ActualColumn + 1);
            await Grid.ResolveOverlapsAsync(this, (0, -1));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
        }
        internal async Task MoveLeftAsync()
        {
            await UpdateColumnAsync(ActualColumn - 1);
            await Grid.ResolveOverlapsAsync(this, (0, 1));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
        }
        internal async Task MoveUpAsync()
        {
            await UpdateRowAsync(ActualRow - 1);
            await Grid.ResolveOverlapsAsync(this, (1, 0));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
        }
        internal async Task MoveDownAsync()
        {
            await UpdateRowAsync(ActualRow + 1);
            await Grid.ResolveOverlapsAsync(this, (-1, 0));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
        }
        internal async Task IncreaseWidthAsync()
        {
            await UpdateColumnSpanAsync(ActualColumnSpan + 1);
            await Grid.ResolveOverlapsAsync(this, (0, 1));
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
        }
        internal async Task IncreaseHeightAsync()
        {
            await UpdateRowSpanAsync(ActualRowSpan + 1);
            await Grid.ResolveOverlapsAsync(this, (1, 0));
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
        }
        internal async Task DecreaseWidthAsync()
        {
            await UpdateColumnSpanAsync(ActualColumnSpan - 1);
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
        }
        internal async Task DecreaseHeightAsync()
        {
            await UpdateRowSpanAsync(ActualRowSpan - 1);
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
        }
        internal bool OverlapsWith(Element other)
        {
            // two elements overlap if the not not overlap, duh
            // two elements do not overlap if one element is further to the left, top, right, or bottom than the other.
            return !(other.ActualRow + other.ActualRowSpan <= ActualRow
                  || other.ActualColumn + other.ActualColumnSpan <= ActualColumn
                  || other.ActualRow >= ActualRow + ActualRowSpan
                  || other.ActualColumn >= ActualColumn + ActualColumnSpan);
        }
    }
}