using System.Media;

namespace ChessBrowser.Components
{
    public class ChessGame
    {

        public string Round { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }
        public int WhiteElo { get; set; }
        public int BlackElo { get; set; }
        public char Result { get; set; }
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventSite { get; set; }
        public DateTime EventDate { get; set; }
        public string Moves { get; set; }



    }
}
