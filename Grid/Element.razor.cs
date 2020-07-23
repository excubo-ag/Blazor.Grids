using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Excubo.Blazor.Grids
{
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
        [Parameter] public EventCallback<int> ColumnChanged { get; set; }
        /// <summary>
        /// The row that this element should be in. In case the element spans multiple rows, this value indicates the row which contains the top left corner of the element.
        /// </summary>
        [Parameter] public int Row { get; set; }
        [Parameter] public EventCallback<int> RowChanged { get; set; }
        /// <summary>
        /// The number of columns that this element should span.
        /// Any value less than 1 is interpreted as 1.
        /// </summary>
        [Parameter] public int ColumnSpan { get; set; }
        internal int ActualColumnSpan => Math.Max(1, ColumnSpan);
        [Parameter] public EventCallback<int> ColumnSpanChanged { get; set; }
        /// <summary>
        /// The number of rows that this element should span.
        /// Any value less than 1 is interpreted as 1.
        /// </summary>
        [Parameter] public int RowSpan { get; set; }
        internal int ActualRowSpan => Math.Max(1, RowSpan);
        [Parameter] public EventCallback<int> RowSpanChanged { get; set; }
        /// <summary>
        /// The title that should appear at the top of the element
        /// </summary>
        [Parameter] public string Title { get; set; }
        /// <summary>
        /// Callback for when the element was moved.
        /// </summary>
        [Parameter] public Action OnMove { get; set; }
        /// <summary>
        /// Callback for when the element was resized.
        /// </summary>
        [Parameter] public Action OnResize { get; set; }
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
        public void Resize(int height, int width)
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
            RowSpan = height;
            ColumnSpan = width;
            if (higher || wider)
            {
                var push_direction = higher ? (1, 0) : (0, 1); // push elements downwards or rightwards
                Grid.ResolveOverlaps(this, push_to: push_direction);
            }
            Grid.RenderNothingBut(this);
            _ = RowSpanChanged.InvokeAsync(RowSpan);
            _ = ColumnSpanChanged.InvokeAsync(ColumnSpan);
            InvokeResizeEvents();
        }
        /// <summary>
        /// Moves the element to the specified position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void MoveTo(int row, int column)
        {
            if (row < 0)
            {
                throw new ArgumentException("Row of an element may not be negative", nameof(row));
            }
            if (column < 0)
            {
                throw new ArgumentException("Column of an element may not be negative", nameof(column));
            }
            Row = row;
            Column = column;
            Grid.ResolveOverlaps(this, (-1, 0));
            Grid.RenderNothingBut(this);
            _ = RowChanged.InvokeAsync(Row);
            _ = ColumnChanged.InvokeAsync(Column);
            InvokeMoveEvents();
        }
        #endregion
        private int actual_column_span => ColumnSpan < 2 ? 1 : ColumnSpan;
        private int actual_row_span => RowSpan < 2 ? 1 : RowSpan;
        private string area => string.IsNullOrEmpty(Area) ? null : $"grid-area: {Area};";
        private string column
        {
            get
            {
                if (!string.IsNullOrEmpty(Area))
                {
                    return null;
                }
                if (Column == 0)
                {
                    if (ColumnSpan < 2)
                    {
                        return "grid-column: 1;";
                    }
                    return $"grid-column: 1 / span {ColumnSpan};";
                }
                else
                {
                    if (ColumnSpan < 2)
                    {
                        return $"grid-column: {Column + 1};";
                    }
                    return $"grid-column: {Column + 1} / span {ColumnSpan};";
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
                if (Row == 0)
                {
                    if (RowSpan < 2)
                    {
                        return "grid-row: 1;";
                    }
                    return $"grid-row: 1 / span {RowSpan};";
                }
                else
                {
                    if (RowSpan < 2)
                    {
                        return $"grid-row: {Row + 1};";
                    }
                    return $"grid-row: {Row + 1} / span {RowSpan};";
                }
            }
        }
        protected override void OnParametersSet()
        {
            System.Diagnostics.Debug.Assert(Grid != null);
            Grid.Add(this);
            base.OnParametersSet();
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
            OnMove?.Invoke();
            Grid.OnMove?.Invoke(this);
        }
        private void InvokeResizeEvents()
        {
            OnResize?.Invoke();
            Grid.OnResize?.Invoke(this);
        }
        internal void MoveRight()
        {
            Column += 1;
            Grid.ResolveOverlaps(this, (0, -1));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
            _ = ColumnChanged.InvokeAsync(Column);
        }
        internal void MoveLeft()
        {
            Column -= 1;
            Grid.ResolveOverlaps(this, (0, 1));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
            _ = ColumnChanged.InvokeAsync(Column);
        }
        internal void MoveUp()
        {
            Row -= 1;
            Grid.ResolveOverlaps(this, (1, 0));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
            _ = RowChanged.InvokeAsync(Row);
        }
        internal void MoveDown()
        {
            Row += 1;
            Grid.ResolveOverlaps(this, (-1, 0));
            Grid.RenderNothingBut(this);
            InvokeMoveEvents();
            _ = RowChanged.InvokeAsync(Row);
        }
        internal void IncreaseWidth()
        {
            ColumnSpan = ColumnSpan < 2 ? 2 : ColumnSpan + 1;
            Grid.ResolveOverlaps(this, (0, 1));
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
            _ = ColumnSpanChanged.InvokeAsync(ColumnSpan);
        }
        internal void IncreaseHeight()
        {
            RowSpan = RowSpan < 2 ? 2 : RowSpan + 1;
            Grid.ResolveOverlaps(this, (1, 0));
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
            _ = RowSpanChanged.InvokeAsync(RowSpan);
        }
        internal void DecreaseWidth()
        {
            ColumnSpan -= 1;
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
            _ = ColumnSpanChanged.InvokeAsync(ColumnSpan);
        }
        internal void DecreaseHeight()
        {
            RowSpan -= 1;
            Grid.RenderNothingBut(this);
            InvokeResizeEvents();
            _ = RowSpanChanged.InvokeAsync(RowSpan);
        }
        internal bool OverlapsWith(Element other)
        {
            // two elements overlap if the not not overlap, duh
            // two elements do not overlap if one element is further to the left, top, right, or bottom than the other.
            return !(other.Row + other.ActualRowSpan <= Row
                  || other.Column + other.ActualColumnSpan <= Column
                  || other.Row >= Row + ActualRowSpan
                  || other.Column >= Column + ActualColumnSpan);
        }
    }
}
