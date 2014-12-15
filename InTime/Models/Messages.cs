namespace InTime.Models
{
    public static class Messages
    {
        public enum RequeteSql { Reussi, Echec };
        public enum ChampsBloquer { Non, Oui };
        public static string Envoyer
        {
            get
            {
                return "Envoyer";
            }
        }

        public static string Echec
        {
            get
            {
                return "Echec";
            }
        }
    }
}