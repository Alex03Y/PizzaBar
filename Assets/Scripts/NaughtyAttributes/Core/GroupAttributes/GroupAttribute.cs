namespace NaughtyAttributes
{
    public abstract class GroupAttribute : NaughtyAttribute
    {
        public GroupAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}