public class GameState
{
    public enum WinState { No_Winner, Player1_Wins, Player2_Wins, Tie }

    private const int Rows = 6;
    private const int Cols = 7;

    // board[row, col]: row 0 is the top row. 0 = empty, 1 = player 1, 2 = player 2
    private readonly byte[,] board = new byte[Rows, Cols];

    /// <summary>How many pieces have been played so far (0 to 42).</summary>
    public int CurrentTurn { get; private set; }

    /// <summary>1 or 2, whoever plays next.</summary>
    public byte PlayerTurn => (byte)(CurrentTurn % 2 + 1);

    public void ResetBoard()
    {
        Array.Clear(board);
        CurrentTurn = 0;
    }

    /// <summary>Drops a piece in the column and returns the landing row (6 = bottom, matches the CSS drop1..drop6).</summary>
    public int PlayPiece(byte col)
    {
        if (col >= Cols) throw new ArgumentException("That column does not exist.");
        if (CheckForWin() != WinState.No_Winner) throw new ArgumentException("The game is over. Reset to play again.");

        for (var row = Rows - 1; row >= 0; row--)
        {
            if (board[row, col] == 0)
            {
                board[row, col] = PlayerTurn;
                CurrentTurn++;
                return row + 1;
            }
        }

        throw new ArgumentException("That column is full.");
    }

    public WinState CheckForWin()
    {
        int[][] directions = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, -1 } };

        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Cols; c++)
            {
                var p = board[r, c];
                if (p == 0) continue;

                foreach (var d in directions)
                {
                    var four = true;
                    for (var k = 1; k < 4; k++)
                    {
                        var rr = r + d[0] * k;
                        var cc = c + d[1] * k;
                        if (rr < 0 || rr >= Rows || cc < 0 || cc >= Cols || board[rr, cc] != p)
                        {
                            four = false;
                            break;
                        }
                    }
                    if (four) return p == 1 ? WinState.Player1_Wins : WinState.Player2_Wins;
                }
            }
        }

        return CurrentTurn >= Rows * Cols ? WinState.Tie : WinState.No_Winner;
    }
}