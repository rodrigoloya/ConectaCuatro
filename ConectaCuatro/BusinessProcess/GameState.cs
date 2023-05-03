namespace ConectaCuatro.BusinessProcess;

public class GameState
{

    Ficha[] _pieces;
    byte maxColumns;
    byte maxRows;


    public GameState()
    {
        maxColumns = 7;
        maxRows = 6;

        _players = new List<Player> {  new Player { Id = 1, Name = "Player 1"},
        new Player { Id = 2, Name = "Player 1"}}.ToArray();
        _pieces = ResetFichas();

        //Player Turn can be coin tossed
        PlayerTurn = 1;
    }

    private Ficha[] ResetFichas()
    {
      var fichas =  new Ficha[maxColumns * maxRows];

        for (int i = 0; i < fichas.Length; i++)
        {
            fichas[i] = new Ficha() { Id = (byte)i};
        }

        return fichas;
    }

    Player[] _players;

    public Player[] Players { get { return _players; } }

    public bool IsGameStarted { get; private set; }

    public byte PlayerTurn { get; set; }

    public byte CurrentTurn { get; set; }

    

    public void ResetGame()
    {
        //Reset Score
        //set flag 
        IsGameStarted = false;
        _pieces = ResetFichas();
        _CheckForWin = WinState.None;
        //Player Turn can be coin tossed
        PlayerTurn = 1;
    }

    public byte PlayPiece(byte column)
    {
        //Validar entrada
        if (column > maxColumns - 1)
        {
            throw new InvalidMoveException("Juagada invalida, fuera de los limites del tablero");
        }

        //Verificar GameOver
        if (!_pieces.Any(p => p.IsOccupied == false))
        {
            throw new InvalidMoveException("Game Over!");

        }

        //verificar que la columna no este llena en el indice de 0 a 6
        if (_pieces[column].IsOccupied)
        {
            throw new InvalidMoveException("Jugada invalida la columna esta llena");
        }


        //Devolver la posicion de landing row
        byte row = GetLandingRow(column);


        //Obterner la celda en la que caera la ficha basado en la columna
        byte index = Convert.ToByte( row * maxColumns + column);
        //si esta libre entonces marcarla como ocupada.
        _pieces[index].IsOccupied = true;
        _pieces[index].Player = _players[PlayerTurn - 1];
        _pieces[index].Column = column;
        _pieces[index].Row = row;

        CurrentTurn = index;

        //cambiar alternar jugador
        PlayerTurn = (byte)(PlayerTurn == 1 ? 2 : 1);

        CheckForWin(index);
        return row;
    }

    private byte GetLandingRow(byte column)
    {
        sbyte row = -1;        
        int idx = column;

        while (  row < maxRows -1 && _pieces[idx].IsOccupied == false)
        {
            row++;
            idx += maxColumns;
        }
         
        return Convert.ToByte( row);
    }

    private bool CheckFordward(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int i = _pieces[idx].Column;
        int j = 0;
        for( ; i < maxColumns; i++)
        {
            if(_pieces[idx + j].Player != null &&
                _pieces[idx+j].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    private bool CheckBackward(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int i = _pieces[idx].Column;
        int j = 0;
        for (; i >= 0; i--)
        {
            if (_pieces[idx - j].Player != null &&
                _pieces[idx - j].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    private bool CheckDownward(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int i = _pieces[idx].Row;
        int j = 0;
        for (; i < maxRows; i++)
        {
            if (_pieces[idx + (maxColumns * j)].Player != null &&
                _pieces[idx + (maxColumns * j)].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    [Obsolete("Invalid Use case, players turns will never have a ficha above")]
    private bool CheckUpward(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int i = _pieces[idx].Row;
        int j = 0;
        for (; i >= 0; i--)
        {
            if (_pieces[idx - (7 * j)].Player != null &&
                _pieces[idx - (7*j)].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    private bool CheckDiagonalRightUp(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int col = _pieces[idx].Column;
        int row = _pieces[idx].Row;
        int j = 0;
        for (; col < maxColumns && row>=0; col++)
        {
            row--;
             var newIdx = idx + j - (j * maxColumns);
            
            if (_pieces[newIdx].Player != null &&
                _pieces[newIdx].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    private bool CheckDiagonalLeftUp(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int col = _pieces[idx].Column;
        int row = _pieces[idx].Row;
        int j = 0;
        for (; col >=0 && row >= 0; col--)
        {
            row--;
            var newIdx = idx - j - (j * maxColumns);

            if (_pieces[newIdx].Player != null &&
                _pieces[newIdx].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    private bool CheckDiagonalLeftDown(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int col = _pieces[idx].Column;
        int row = _pieces[idx].Row;
        int j = 0;
        for (; col >= 0 && row < maxRows; col--)
        {
            row++;
            var newIdx = idx - j + (j * maxColumns);

            if (_pieces[newIdx].Player != null &&
                _pieces[newIdx].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }


    private bool CheckDiagonalRightDown(byte idx)
    {
        var player = _pieces[idx].Player;
        int count = 0;
        int col = _pieces[idx].Column;
        int row = _pieces[idx].Row;
        int j = 0;
        for (; col < maxColumns && row < maxRows; col++)
        {
            row++;
            var newIdx = idx + j + (j * maxColumns);

            if (_pieces[newIdx].Player != null &&
                _pieces[newIdx].Player.Id == player.Id)
            {
                count++;
            }
            else
            {
                break;
            }
            j++;
        }
        return count >= 4;
    }

    public WinState CheckForWin()
    {
        //obtener el current player


        return _CheckForWin;

    }

    private WinState _CheckForWin;

    private WinState CheckForWin(byte idx)
    {
        //obtener el current player
        if (CheckFordward(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }
        if (CheckBackward(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        if (CheckDownward(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        if (CheckUpward(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        if (CheckDiagonalRightUp (idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        if (CheckDiagonalLeftUp(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        if (CheckDiagonalLeftDown(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        if (CheckDiagonalRightDown(idx))
        {
            if (_pieces[idx].Player.Id == 1)
            {
                _CheckForWin = WinState.Player1;
            }
            else
            {
                _CheckForWin = WinState.Player2;
            }
        }

        //TODO check diagonal

        return _CheckForWin;
    }

}

public class Ficha {
       

    public byte Id { get; set; }

    public Player Player { get; set; }

    public bool IsOccupied { get; set; }

    public byte Column { get; set; }
    public byte Row { get; set; }

}


public class Player
{
   
    public int Id { get; set; }
    public string? Name { get; set; }
    
}

public enum WinState
{
    None,
    Player1,
    Player2,
    Tie
}
