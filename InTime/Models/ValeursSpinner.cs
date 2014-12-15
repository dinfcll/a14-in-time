using System;

namespace InTime.Models
{
    public static class ValeursSpinner
    {
        public static int ValeurMaximal { get; set; }
        public static int ValeurMinimal { get; set; }
        public static int AnneeEnCours
        {
            get
            {
                return DateTime.Now.Year;
            }
        }

        static ValeursSpinner()
        {
            ValeurMaximal = 2114;
            ValeurMinimal = 2014;
        }
    }
}
