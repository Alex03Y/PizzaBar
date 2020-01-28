using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnableIfAttribute : DrawerAttribute
    {
        public EnableIfAttribute(string condition)
        {
            ConditionOperator = ConditionOperator.And;
            Conditions = new string[1] {condition};
        }

        public EnableIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
        {
            ConditionOperator = conditionOperator;
            Conditions = conditions;
        }

        public string[] Conditions { get; }
        public ConditionOperator ConditionOperator { get; }
        public bool Reversed { get; protected set; }
    }
}