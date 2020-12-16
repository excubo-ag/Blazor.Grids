namespace Excubo.Blazor.Grids.__Internal
{
    internal static class Changes
    {
        public static (bool v_increase, bool v_decrease, bool h_increase, bool h_decrease) GetRequiredChanges(this (double width, double height) ratios, double stronger_threshold, double weaker_threshold)
        {
            var width_increase = ratios.width.AboveThreshold(stronger_threshold, weaker_threshold);
            var width_decrease = ratios.width.BelowThreshold(-stronger_threshold, -weaker_threshold);
            var height_increase = ratios.height.AboveThreshold(stronger_threshold, weaker_threshold);
            var height_decrease = ratios.height.BelowThreshold(-stronger_threshold, -weaker_threshold);

            var should_width_increase = ShouldWidthIncrease(width: (width_increase, width_decrease), height: (height_increase, height_decrease));
            var should_width_decrease = ShouldWidthDecrease(width: (width_increase, width_decrease), height: (height_increase, height_decrease));
            var should_height_increase = ShouldHeightIncrease(width: (width_increase, width_decrease), height: (height_increase, height_decrease));
            var should_height_decrease = ShouldHeightDecrease(width: (width_increase, width_decrease), height: (height_increase, height_decrease));
            return (should_height_increase, should_height_decrease, should_width_increase, should_width_decrease);
        }
        private enum Threshold
        {
            No,
            Maybe,
            Yes
        }
        private static Threshold AboveThreshold(this double v, double stronger_threshold, double weaker_threshold)
        {
            if (v > stronger_threshold)
            {
                return Threshold.Yes;
            }
            if (v > weaker_threshold)
            {
                return Threshold.Maybe;
            }
            return Threshold.No;
        }
        private static Threshold BelowThreshold(this double v, double stronger_threshold, double weaker_threshold)
        {
            if (v < stronger_threshold)
            {
                return Threshold.Yes;
            }
            if (v < weaker_threshold)
            {
                return Threshold.Maybe;
            }
            return Threshold.No;
        }
        private static bool ShouldWidthIncrease((Threshold Increase, Threshold Decrease) width, (Threshold Increase, Threshold Decrease) height) => ShouldIncrease(width, height);
        private static bool ShouldWidthDecrease((Threshold Increase, Threshold Decrease) width, (Threshold Increase, Threshold Decrease) height) => ShouldDecrease(width, height);
        private static bool ShouldHeightIncrease((Threshold Increase, Threshold Decrease) width, (Threshold Increase, Threshold Decrease) height) => ShouldIncrease(height, width);
        private static bool ShouldHeightDecrease((Threshold Increase, Threshold Decrease) width, (Threshold Increase, Threshold Decrease) height) => ShouldDecrease(height, width);
        private static bool ShouldIncrease((Threshold Increase, Threshold Decrease) first, (Threshold Increase, Threshold Decrease) second) => ShouldChange(first.Increase, second);
        private static bool ShouldDecrease((Threshold Increase, Threshold Decrease) first, (Threshold Increase, Threshold Decrease) second) => ShouldChange(first.Decrease, second);
        private static bool ShouldChange(Threshold threshold, (Threshold Increase, Threshold Decrease) second)
        {
            return threshold switch
            {
                Threshold.Yes => true,
                Threshold.Maybe when second.Increase == Threshold.Yes => true,
                Threshold.Maybe when second.Decrease == Threshold.Yes => true,
                _ => false
            };
        }
    }
}