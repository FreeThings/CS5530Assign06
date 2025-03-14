﻿using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Mysqlx;

namespace ChessBrowser.Components.Pages
{
    public partial class ChessBrowser
    {
        /// <summary>
        /// Bound to the Unsername form input
        /// </summary>
        private string Username = "";

        /// <summary>
        /// Bound to the Password form input
        /// </summary>
        private string Password = "";

        /// <summary>
        /// Bound to the Database form input
        /// </summary>
        private string Database = "";

        /// <summary>
        /// Represents the progress percentage of the current
        /// upload operation. Update this value to update 
        /// the progress bar.
        /// </summary>
        private int Progress = 0;

        /// <summary>
        /// This method runs when a PGN file is selected for upload.
        /// Given a list of lines from the selected file, parses the 
        /// PGN data, and uploads each chess game to the user's database.
        /// </summary>
        /// <param name="PGNFileLines">The lines from the selected file</param>
        private async Task InsertGameData(string[] PGNFileLines)
        {
            // This will build a connection string to your user's database on atr,
            // assuimg you've filled in the credentials in the GUI
            string connection = GetConnectionString();

            // Parse PGN Data
            List<ChessGame> games = PGNParser.ChessGameParser(PGNFileLines);

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // TODO:
                    //   Iterate through your data and generate appropriate insert commands
                    uint white_id, black_id, event_id;

                    int gameNumber = 0;

                    foreach (ChessGame game in games)
                    {
                        gameNumber++;
                        //Check if black player in database
                        string black_player_query = "SELECT Name FROM Players WHERE Name = @black";
                        MySqlCommand black_player_cmd = new MySqlCommand(black_player_query, conn);
                        black_player_cmd.Parameters.AddWithValue("@black", game.BlackPlayer);

                        //If player not in database, insert player
                        using (MySqlDataReader reader = black_player_cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                string insert_player_query = "INSERT INTO Players (Name, Elo) VALUES (@black, @elo)";
                                MySqlCommand insert_player_cmd = new MySqlCommand(insert_player_query, conn);
                                insert_player_cmd.Parameters.AddWithValue("@black", game.BlackPlayer);
                                insert_player_cmd.Parameters.AddWithValue("@elo", game.BlackElo);
                                insert_player_cmd.ExecuteNonQuery();
                            }
                        }

                        // Check if black player Elo is higher
                        string getBlackPlayerElo_query = "SELECT Elo FROM Players WHERE Name = @black";
                        MySqlCommand getBlackPlayerElo_cmd = new MySqlCommand(getBlackPlayerElo_query, conn);
                        getBlackPlayerElo_cmd.Parameters.AddWithValue("@black", game.BlackPlayer);
                        uint currentBlackElo = (uint)getBlackPlayerElo_cmd.ExecuteScalar();

                        if (game.BlackElo > currentBlackElo)
                        {
                            string update_player_query = "UPDATE Players SET Elo = @elo WHERE Name = @black";
                            MySqlCommand update_player_cmd = new MySqlCommand(update_player_query, conn);
                            update_player_cmd.Parameters.AddWithValue("@black", game.BlackPlayer);
                            update_player_cmd.Parameters.AddWithValue("@elo", game.BlackElo);
                            update_player_cmd.ExecuteNonQuery();
                        }

                        string white_player_query = "SELECT Name FROM Players WHERE Name = @white";
                        MySqlCommand white_player_cmd = new MySqlCommand(white_player_query, conn);
                        white_player_cmd.Parameters.AddWithValue("@white", game.WhitePlayer);


                        //If player not in database, insert player
                        using (MySqlDataReader reader = white_player_cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                string insert_player_query = "INSERT INTO Players (Name, Elo) VALUES (@white, @elo)";
                                MySqlCommand insert_player_cmd = new MySqlCommand(insert_player_query, conn);
                                insert_player_cmd.Parameters.AddWithValue("@white", game.WhitePlayer);
                                insert_player_cmd.Parameters.AddWithValue("@elo", game.WhiteElo);
                                insert_player_cmd.ExecuteNonQuery();
                            }
                        }

                        // Check if white player Elo is higher
                        string getWhitePlayerElo_query = "SELECT Elo FROM Players WHERE Name = @white";
                        MySqlCommand getWhtiePlayerElo_cmd = new MySqlCommand(getWhitePlayerElo_query, conn);
                        getWhtiePlayerElo_cmd.Parameters.AddWithValue("@white", game.WhitePlayer);
                        uint currentWhiteElo = (uint)getWhtiePlayerElo_cmd.ExecuteScalar();

                        if (game.WhiteElo > currentWhiteElo)
                        {
                            string update_player_query = "UPDATE Players SET Elo = @elo WHERE Name = @black";
                            MySqlCommand update_player_cmd = new MySqlCommand(update_player_query, conn);
                            update_player_cmd.Parameters.AddWithValue("@black", game.WhitePlayer);
                            update_player_cmd.Parameters.AddWithValue("@elo", game.WhiteElo);
                            update_player_cmd.ExecuteNonQuery();
                        }

                        //Get white player id
                        string get_player_id_query = "SELECT pID FROM Players WHERE Name = @white";
                        MySqlCommand get_player_id_cmd = new MySqlCommand(get_player_id_query, conn);
                        get_player_id_cmd.Parameters.AddWithValue("@white", game.WhitePlayer);
                        white_id = (uint)get_player_id_cmd.ExecuteScalar();

                        //Get black player id
                        get_player_id_query = "SELECT pID FROM Players WHERE Name = @black";
                        get_player_id_cmd = new MySqlCommand(get_player_id_query, conn);
                        get_player_id_cmd.Parameters.AddWithValue("@black", game.BlackPlayer);
                        black_id = (uint)get_player_id_cmd.ExecuteScalar();

                        //Check if Event in database
                        string event_query = "SELECT Name FROM Events WHERE Name = @event AND Date = @date AND Site = @site";
                        MySqlCommand event_cmd = new MySqlCommand(event_query, conn);
                        event_cmd.Parameters.AddWithValue("@event", game.EventName);
                        event_cmd.Parameters.AddWithValue("@date", game.EventDate);
                        event_cmd.Parameters.AddWithValue("@site", game.EventSite);

                        //If event not in database, insert event
                        using (MySqlDataReader reader = event_cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                string insert_event_query = "INSERT INTO Events (Name, Date, Site) VALUES (@event, @date, @site)";
                                MySqlCommand insert_event_cmd = new MySqlCommand(insert_event_query, conn);
                                insert_event_cmd.Parameters.AddWithValue("@event", game.EventName);
                                insert_event_cmd.Parameters.AddWithValue("@date", game.EventDate);
                                insert_event_cmd.Parameters.AddWithValue("@site", game.EventSite);
                                insert_event_cmd.ExecuteNonQuery();
                            }
                        }

                        //Get event id
                        string get_event_id_query = "SELECT eID FROM Events WHERE Name = @event AND Date = @date AND Site = @site";
                        MySqlCommand get_event_id_cmd = new MySqlCommand(get_event_id_query, conn);
                        get_event_id_cmd.Parameters.AddWithValue("@event", game.EventName);
                        get_event_id_cmd.Parameters.AddWithValue("@date", game.EventDate);
                        get_event_id_cmd.Parameters.AddWithValue("@site", game.EventSite);
                        event_id = (uint)get_event_id_cmd.ExecuteScalar();

                        // Insert the game into the database
                        string insert_query = "INSERT INTO Games (Round, Result, Moves, BlackPlayer, WhitePlayer, eID) VALUES (@round, @result, @moves, @black, @white, @eID)";
                        MySqlCommand cmd = new MySqlCommand(insert_query, conn);
                        cmd.Parameters.AddWithValue("@white", white_id);
                        cmd.Parameters.AddWithValue("@black", black_id);
                        cmd.Parameters.AddWithValue("@result", game.Result);
                        cmd.Parameters.AddWithValue("@moves", game.Moves);
                        cmd.Parameters.AddWithValue("@round", game.Round);
                        cmd.Parameters.AddWithValue("@eID", event_id);
                        cmd.ExecuteNonQuery();


                        // Progress bar update
                        Progress = (int)(((double)gameNumber / (double)games.Count) * 100);

                        // This tells the GUI to redraw after you update Progress (this should go inside your loop)
                        await InvokeAsync(StateHasChanged);
                    }

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

        }


        /// <summary>
        /// Queries the database for games that match all the given filters.
        /// The filters are taken from the various controls in the GUI.
        /// </summary>
        /// <param name="white">The white player, or "" if none</param>
        /// <param name="black">The black player, or "" if none</param>
        /// <param name="opening">The first move, e.g. "1.e4", or "" if none</param>
        /// <param name="winner">The winner as "W", "B", "D", or "" if none</param>
        /// <param name="useDate">true if the filter includes a date range, false otherwise</param>
        /// <param name="start">The start of the date range</param>
        /// <param name="end">The end of the date range</param>
        /// <param name="showMoves">true if the returned data should include the PGN moves</param>
        /// <returns>A string separated by newlines containing the filtered games</returns>
        private string PerformQuery(string white, string black, string opening,
          string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
        {
            // This will build a connection string to your user's database on atr,
            // assuimg you've typed a user and password in the GUI
            string connection = GetConnectionString();

            // Build up this string containing the results from your query
            string parsedResult = "";

            // Use this to count the number of rows returned by your query
            // (see below return statement)
            int numRows = 0;

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    // SQL Query to include information about search terms:
                    // Search conditions added assuming the term is not null or empty
                    string sql = "SELECT g.Round, g.Result, g.Moves, " +
                                 "w.Name AS WhitePlayer, w.Elo AS WhiteElo, " +
                                 "b.Name AS BlackPlayer, b.Elo AS BlackElo, " +
                                 "e.Name AS Event, e.Site, COALESCE(e.Date, '0000-00-00') AS Date " +
                                 "FROM Games g " +
                                 "JOIN Players w ON g.WhitePlayer = w.pID " +
                                 "JOIN Players b ON g.BlackPlayer = b.pID " +
                                 "JOIN Events e ON g.eID = e.eID " +
                                 "WHERE 1=1";  // Placeholder condition

                    // Add conditions based on the provided filters
                    if (!string.IsNullOrEmpty(white))
                    {
                        sql += " AND w.Name = @white";
                    }
                    if (!string.IsNullOrEmpty(black))
                    {
                        sql += " AND b.Name = @black";
                    }
                    if (!string.IsNullOrEmpty(opening))
                    {
                        sql += " AND g.Moves LIKE @opening";
                    }
                    if (!string.IsNullOrEmpty(winner))
                    {
                        // Filter based on the winner
                        switch (winner)
                        {
                            case "W": // White won
                                sql += " AND g.Result = 'W'";
                                break;
                            case "B": // Black won
                                sql += " AND g.Result = 'B'";
                                break;
                            case "D": // Draw
                                sql += " AND g.Result = 'D'";
                                break;
                        }
                    }
                    if (useDate)
                    {
                        sql += " AND e.Date BETWEEN @start AND @end";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    // Add parameters to the command to prevent SQL injection
                    if (!string.IsNullOrEmpty(white)) cmd.Parameters.AddWithValue("@white", white);
                    if (!string.IsNullOrEmpty(black)) cmd.Parameters.AddWithValue("@black", black);
                    if (!string.IsNullOrEmpty(opening)) cmd.Parameters.AddWithValue("@opening", "%" + opening + "%");
                    if (!string.IsNullOrEmpty(winner)) cmd.Parameters.AddWithValue("@winner", winner);
                    if (useDate)
                    {
                        cmd.Parameters.AddWithValue("@start", start.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@end", end.ToString("yyyy-MM-dd"));
                    }

                    // Execute the query and retrieve results
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            numRows++;

                            // Retrieve player names and Elo ratings
                            string WhitePlayer = (string)reader["WhitePlayer"];
                            string BlackPlayer = (string)reader["BlackPlayer"];
                            uint WhiteElo = Convert.ToUInt32(reader["WhiteElo"]);
                            uint BlackElo = Convert.ToUInt32(reader["BlackElo"]);

                            // Retrieve and translate the result
                            string result;

                            switch (reader["Result"].ToString())
                            {
                                case "W":
                                    result = "W";
                                    break;
                                case "B":
                                    result = "B";
                                    break;
                                case "D":
                                    result = "D";
                                    break;
                                default:
                                    result = "N/A";
                                    break;
                            }

                            string date = "";

                            // Check if the date is valid and format it
                            if (DateTime.TryParse(reader["Date"].ToString(), out DateTime parsedDate))
                            {
                                date = parsedDate.ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                date = "00/00/0000";
                            }

                            // Format the result row
                            parsedResult += "Event: " + reader["Event"].ToString() + "\n" +
                                           "Site: " + reader["Site"].ToString() + "\n" +
                                           "Date: " + date + "\n" +
                                           "White: " + WhitePlayer + " (" + WhiteElo + ")\n" +
                                           "Black: " + BlackPlayer + " (" + BlackElo + ")\n" +
                                           "Result: " + result + "\n";

                            // Include the PGN moves if showMoves is true
                            if (showMoves)
                            {
                                parsedResult += "Moves: " + reader["Moves"].ToString() + "\n";
                            }

                            parsedResult += "\n";  // Add a newline for each result
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            return numRows + " results\n" + parsedResult;
        }


        private string GetConnectionString()
        {
            return "server=atr.eng.utah.edu;database=" + Database + ";uid=" + Username + ";password=" + Password;
        }


        /// <summary>
        /// This method will run when the file chooser is used.
        /// It loads the files contents as an array of strings,
        /// then invokes the InsertGameData method.
        /// </summary>
        /// <param name="args">The event arguments, which contains the selected file name</param>
        private async void HandleFileChooser(EventArgs args)
        {
            try
            {
                string fileContent = string.Empty;

                InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
                if (eventArgs.FileCount == 1)
                {
                    var file = eventArgs.File;
                    if (file is null)
                    {
                        return;
                    }

                    // load the chosen file and split it into an array of strings, one per line
                    using var stream = file.OpenReadStream(1000000); // max 1MB
                    using var reader = new StreamReader(stream);
                    fileContent = await reader.ReadToEndAsync();
                    string[] fileLines = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    // insert the games, and don't wait for it to finish
                    // _ = throws away the task result, since we aren't waiting for it
                    _ = InsertGameData(fileLines);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("an error occurred while loading the file..." + e);
            }
        }

    }

}
