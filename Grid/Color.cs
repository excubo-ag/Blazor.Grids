namespace Excubo.Blazor.Grids
{
    public struct Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public static implicit operator Color((int R, int G, int B) value)
        {
            return new Color
            {
                R = value.R,
                G = value.G,
                B = value.B
            };
        }
        public static implicit operator Color(int rgb)
        {
            return new Color
            {
                R = rgb / 256 / 256,
                G = (rgb / 256) % 256,
                B = rgb % 256
            };
        }
    }
}