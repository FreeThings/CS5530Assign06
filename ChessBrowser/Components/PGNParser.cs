using MySqlX.XDevAPI.Common;
using System.Diagnostics.Eventing.Reader;

// Authors: Aiden de Boer and Josh Greenbaum
// Date: 2025-03-07

namespace ChessBrowser.Components
{
    // This class is responsible for parsing PGN files and extracting the relevant information
    public static class PGNParser
    {

        public static List<ChessGame> ChessGameParser(string[] PGN)
        {

            List<ChessGame> games = new List<ChessGame>();
            ChessGame game = new ChessGame();
            bool flag = false;

            foreach (string line in PGN)
            {
                // Flag alternates between true and false to determine if the line is a tag or a move
                if (string.IsNullOrWhiteSpace(line)) 
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
                // If the line is a tag, parse the tag and its contents
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
                            if (contents[0] == '1')
                            {

                                if (contents[1] == '/')
                                {
                                    game.Result = 'D';
                                } else
                                {
                                    game.Result = 'W';
                                }
                            }
                            else
                            {
                                game.Result = 'B';
                            }
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

            // If the last game has moves, add it to the list of games
            if (!string.IsNullOrEmpty(game.Moves))
            {
                games.Add(game);
            }
            
            return games;
        }

    }
}
