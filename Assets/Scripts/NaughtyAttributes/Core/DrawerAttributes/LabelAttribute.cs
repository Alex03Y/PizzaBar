using System;

namespace NaughtyAttributes
{
    /// <summary>
    ///     Override default label
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LabelAttribute : DrawerAttribute
    {
        public LabelAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }
}