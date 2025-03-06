using System.Media;

namespace ChessBrowser.Components
{
    public class ChessGame
    {

        private string round { get; set; }
        private string whitePlayer { get; set; }
        private string blackPlayer { get; set; }
        private int whiteElo { get; set; }
        private int blackElo { get; set; }
        private char result { get; set; }
        private int eventID { get; set; }
        private string eventName { get; set; }
        private string eventSite { get; set; }
        private DateTime date { get; set; }
        private string moves { get; set; }



    }
}
