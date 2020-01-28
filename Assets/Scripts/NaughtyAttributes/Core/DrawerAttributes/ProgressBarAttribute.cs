using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ProgressBarAttribute : DrawerAttribute
    {
        public ProgressBarAttribute(string name = "", float maxValue = 100,
                                    ProgressBarColor color = ProgressBarColor.Blue)
        {
            Name = name;
            MaxValue = maxValue;
            Color = color;
        }

        public string Name { get; }
        public float MaxValue { get; }
        public ProgressBarColor Color { get; }
    }

    public enum ProgressBarColor
    {
        Red,
        Pink,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet,
        White
    }
}