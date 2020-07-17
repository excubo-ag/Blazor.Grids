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
        [Parameter] public EventCallback<int> ColumnSpanChanged { get; set; }
        /// <summary>
        /// The number of rows that this element should span.
        /// Any value less than 1 is interpreted as 1.
        /// </summary>
        [Parameter] public int RowSpan { get; set; }
        [Parameter] public EventCallback<int> RowSpanChanged { get; set; }
        /// <summary>
        /// The title that should appear at the top of the element
        /// </summary>
        [Parameter] public string Title { get; set; }
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
            RowSpan = height;
            _ = RowSpanChanged.InvokeAsync(RowSpan);
            ColumnSpan = width;
            _ = ColumnSpanChanged.InvokeAsync(ColumnSpan);
            Grid.UpdateRows();
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
            _ = RowChanged.InvokeAsync(Row);
            Column = column;
            _ = ColumnChanged.InvokeAsync(Column);
            Grid.UpdateRows();
        }
        #endregion
        private int actual_column_span => ColumnSpan < 2 ? 1 : ColumnSpan;
        private int actual_row_span => RowSpan < 2 ? 1 : RowSpan;
        private string column
        {
            get
            {
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
        internal bool render_required = true;
        protected override bool ShouldRender()
        {
            if (!render_required)
            {
                render_required = true;
                return false;
            }
            return base.ShouldRender();
        }
        internal void MoveRight()
        {
            Grid.RenderNothingBut(this);
            Column += 1;
            _ = ColumnChanged.InvokeAsync(Column);
        }
        internal void MoveLeft()
        {
            Grid.RenderNothingBut(this);
            Column -= 1;
            _ = ColumnChanged.InvokeAsync(Column);
        }
        internal void MoveUp()
        {
            Grid.RenderNothingBut(this);
            Row -= 1;
            Grid.UpdateRows();
            _ = RowChanged.InvokeAsync(Row);
        }
        internal void MoveDown()
        {
            Grid.RenderNothingBut(this);
            Row += 1;
            Grid.UpdateRows();
            _ = RowChanged.InvokeAsync(Row);
        }
        internal void IncreaseWidth()
        {
            Grid.RenderNothingBut(this);
            ColumnSpan = ColumnSpan < 2 ? 2 : ColumnSpan + 1;
            _ = ColumnSpanChanged.InvokeAsync(ColumnSpan);
        }
        internal void IncreaseHeight()
        {
            Grid.RenderNothingBut(this);
            RowSpan = RowSpan < 2 ? 2 : RowSpan + 1;
            _ = RowSpanChanged.InvokeAsync(RowSpan);
            Grid.UpdateRows();
        }
        internal void DecreaseWidth()
        {
            Grid.RenderNothingBut(this);
            ColumnSpan -= 1;
            _ = ColumnSpanChanged.InvokeAsync(ColumnSpan);
        }
        internal void DecreaseHeight()
        {
            Grid.RenderNothingBut(this);
            RowSpan -= 1;
            _ = RowSpanChanged.InvokeAsync(RowSpan);
            Grid.UpdateRows();
        }
    }
}
