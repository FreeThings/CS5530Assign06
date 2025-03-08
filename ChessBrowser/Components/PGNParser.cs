using System.Diagnostics.Eventing.Reader;

namespace ChessBrowser.Components
{
    public static class PGNParser
    {

        public static List<ChessGame> ChessGameParser(string[] PGN)
        {

            Console.WriteLine("Parsing PGN...");

            List<ChessGame> games = new List<ChessGame>();
            ChessGame game = new ChessGame();
            bool flag = false;

            foreach (string line in PGN)
            {
                if(string.IsNullOrWhiteSpace(line)) 
                { 
                    if (!flag)
                    {
                        flag = true;
                        continue;
                    } 
                    else
                    {
                        games.Add(game);
                        game = new ChessGame();
                        flag = false;
                        continue;
                    }
                }
                if (line.Substring(0,1) == "[")
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
                            if (DateTime.TryParse(contents, out DateTime date))
                            {
                                game.EventDate = date.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                game.EventDate = "0000-00-00";
                            }
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
            
            if (!string.IsNullOrEmpty(game.Moves))
            {
                games.Add(game);
            }
            
            return games;
        }

    }
}
