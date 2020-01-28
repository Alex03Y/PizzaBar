namespace PizzaBar.Scripts.Misc
{
    public static class ExperienceConverter
    {
        private static float _experienceRiseCoefficient = 1.5f;
        private static float _amountOfExperienceForOneLvl = 100f;
        private static int _currentLvl = 1;
        private static float _rightEdge, _leftEdge;
        
        public static float GetPercentageForProgressBarAndCurrentLvl(float count, out int lvl)
        {
            if (count > _amountOfExperienceForOneLvl) DefineEdges();
            
            lvl = _currentLvl;

            var amountExpirienceInLvl = _amountOfExperienceForOneLvl - _leftEdge;
            var currentExpirience = count - _leftEdge;
            return (currentExpirience / (amountExpirienceInLvl/100)) / 100;
        }

        private static void DefineEdges()
        {
            _currentLvl++;
            _leftEdge = _amountOfExperienceForOneLvl;
            _amountOfExperienceForOneLvl += _amountOfExperienceForOneLvl;
            _amountOfExperienceForOneLvl *= _experienceRiseCoefficient;
        }
    }
}
