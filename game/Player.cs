using System.Globalization;

namespace game
{
    internal class Player
    {
        internal string Name { get; set; }
        internal char Symbol { get; set; }
        internal bool IsComputer { get; set; }
        internal int Score { get; set; }
        internal bool Forfeited { get; set; }

        internal Player(string i_Name, char i_Symbol)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            Name = textInfo.ToTitleCase(i_Name.ToLower());
            Symbol = i_Symbol;
        }
    }
}