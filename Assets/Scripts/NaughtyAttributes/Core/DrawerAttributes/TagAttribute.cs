using System;

namespace NaughtyAttributes
{
    /// <summary>
    ///     Make tags appear as tag popup fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TagAttribute : DrawerAttribute
    {
    }
}