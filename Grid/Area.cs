using Microsoft.AspNetCore.Components;
using System;

namespace Excubo.Blazor.Grids
{
    public class Area : ComponentBase
    {
        private int row;
        private int row_span;
        private int column;
        private int column_span;

        [Parameter] public string Name { get; set; }
        [Parameter] public int Row { get => row; set { row = value; Grid?.UpdateArea(); } }
        [Parameter] public int RowSpan { get => row_span; set { row_span = value; Grid?.UpdateArea(); } }
        internal int ActualRowSpan => Math.Max(1, RowSpan);
        [Parameter] public int Column { get => column; set { column = value; Grid?.UpdateArea(); } }
        [Parameter] public int ColumnSpan { get => column_span; set { column_span = value; Grid?.UpdateArea(); } }
        internal int ActualColumnSpan => Math.Max(1, ColumnSpan);
        [CascadingParameter] public Grid Grid { get; set; }
        protected override void OnParametersSet()
        {
            System.Diagnostics.Debug.Assert(Grid != null);
            Grid.Add(this);
            base.OnParametersSet();
        }
    }
}
