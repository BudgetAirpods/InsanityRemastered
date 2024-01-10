using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityRemasteredMod.Mod_Integrations
{
    internal class AdvancedCompanyIntegration
    {
        public static bool nightVision;

        private static void UnequipNightVision()
        {
            nightVision = false;
        }
        private static void NightVisionUse(bool on)
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
