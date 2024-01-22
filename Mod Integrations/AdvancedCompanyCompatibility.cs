namespace InsanityRemastered.ModIntegration
{
    public class AdvancedCompanyCompatibility
    {
        public static bool nightVision;

        private static void UnequipHeadLightUtility()
        {
            nightVision = false;
        }
        private static void HeadLightUtilityUse(bool on)
        {
            if (on)
            {
                nightVision = true;
            }
            else if (!on)
            {
                nightVision = false;
            }
        }

    }
}
