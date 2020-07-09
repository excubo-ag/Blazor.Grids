using Microsoft.AspNetCore.Components;

namespace Excubo.Blazor.Grids
{
    public partial class Element
    {
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
                        return null;
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
                        return null;
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
        [CascadingParameter] public Grid Grid { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public int Column { get; set; }
        [Parameter] public EventCallback<int> ColumnChanged { get; set; }
        [Parameter] public int Row { get; set; }
        [Parameter] public EventCallback<int> RowChanged { get; set; }
        [Parameter] public int ColumnSpan { get; set; }
        [Parameter] public EventCallback<int> ColumnSpanChanged { get; set; }
        [Parameter] public int RowSpan { get; set; }
        [Parameter] public EventCallback<int> RowSpanChanged { get; set; }
        [Parameter] public string Title { get; set; }
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
            Column += 1;
            _ = ColumnChanged.InvokeAsync(Column);
        }
        internal void MoveLeft()
        {
            Column -= 1;
            _ = ColumnChanged.InvokeAsync(Column);
        }
        internal void MoveUp()
        {
            Row -= 1;
            _ = RowChanged.InvokeAsync(Row);
        }
        internal void MoveDown()
        {
            Row += 1;
            _ = RowChanged.InvokeAsync(Row);
        }
    }
}
