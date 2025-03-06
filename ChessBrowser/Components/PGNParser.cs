using System.Diagnostics.Eventing.Reader;

namespace ChessBrowser.Components
{
    public static class PGNParser
    {

        public static ChessGame ChessGameParser(string PGN)
        {

            String[] lines = PGN.Split("\n");

            ChessGame game = new ChessGame();

            foreach (string line in lines)
            {

                if (line.Substring(0,0) == "[")
                {
                    string tag = line.Substring(1).Split(" ")[0];

                    string contents = line.Split("\"")[1];

                    switch (tag)
                    {
                        case "Event":
                            game.EventName = contents;
                            break;
                        case "Site":
                            game.EventSite = contents;
                            break;
                        case "EventDate":
                            game.EventDate = DateTime.Parse(contents);
                            break;
                        case "Round":
                            game.Round = contents;
                            break;
                        case "White":
                            game.WhitePlayer = contents;
                            break;
                        case "Black":
                            game.BlackPlayer = contents;
                            break;
                        case "Result":
                            game.Result = contents[0];
                            break;
                        case "WhiteElo":
                            game.WhiteElo = int.Parse(contents);
                            break;
                        case "BlackElo":
                            game.BlackElo = int.Parse(contents);
                            break;
                    }
                } else
                {
                    game.Moves += line;
                }

            }

            return game;
        }

    }
}
