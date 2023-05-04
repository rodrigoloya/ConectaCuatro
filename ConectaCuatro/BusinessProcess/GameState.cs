namespace ConectaCuatro.BusinessProcess;

public class GameState
{

    Ficha[] _pieces;
    short maxColumns;
    short maxRows;


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

    public bool IsGameOver { get; private set; }

    public short PlayerTurn { get; set; }

    public short CurrentTurn { get; set; }

    

    public void ResetGame()
    {
        //Reset Score
        //set flag 
        IsGameOver = false;
        _pieces = ResetFichas();
        _CheckForWin = WinState.None;
        //Player Turn can be coin tossed
        PlayerTurn = 1;
    }

    public short PlayPiece(short column)
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
        short row = GetLandingRow(column);


        //Obterner la celda en la que caera la ficha basado en la columna
        short index = Convert.ToInt16(  row * maxColumns + column );
        //si esta libre entonces marcarla como ocupada.
        _pieces[index].IsOccupied = true;
        _pieces[index].Player = _players[PlayerTurn - 1];
        _pieces[index].Column = column;
        _pieces[index].Row = row;

        CurrentTurn = index;

        //cambiar alternar jugador
        PlayerTurn = (short)(PlayerTurn == 1 ? 2 : 1);

        CheckForWin(index);
        return row;
    }

    private short GetLandingRow(short column)
    {
        short row = -1;        
        int idx = column;

        while (  row < maxRows -1 && _pieces[idx].IsOccupied == false)
        {
            row++;
            idx += maxColumns;
        }
         
        return Convert.ToByte( row);
    }

    private bool CheckHorizontal(short idx)
    {
        //find the most farther from left

        //mientras no sea null, mismo player, misma columna
        short i = 1;
        short newIndex = idx;
        while (newIndex -1  >= 0 &&
            _pieces[newIndex-1].Player!=null &&
            _pieces[newIndex-1].Player.Id == _pieces[idx].Player.Id &&
            _pieces[newIndex-1].Row == _pieces[idx].Row)
        {
            newIndex =Convert.ToByte( idx - i);
            i++;
        }
        return CheckFordward(newIndex);
    }
    private bool CheckDiagonaUp(short idx)
    {
        //find the most farther from left

        //mientras no sea null, mismo player, misma columna
        short col =  _pieces[idx].Column;
        short row =  _pieces[idx].Row;
        short newIndex = idx;
        short i = 0;
        while (col >= 0 && row < maxRows &&
            _pieces[idx - i + (maxColumns * i)].Player != null &&
            _pieces[idx - i + (maxColumns * i)].Player.Id == _pieces[idx].Player.Id)
        {
            newIndex = Convert.ToByte(idx - i+(maxColumns*i));
            i++;
            col--;
            row++;
        }
        return CheckDiagonalRightUp(newIndex);
    }

    private bool CheckDiagonaDown(short idx)
    {
        //find the most farther from left

        //mientras no sea null, mismo player, misma columna
        short col = _pieces[idx].Column;
        short row = _pieces[idx].Row;
        short newIndex = idx;
        int i = 0;
        if (col != 0 && row != 0)
        {
            while (col >= 0 && row >= 0 &&
                _pieces[idx - i - (maxColumns * i)].Player != null &&
                _pieces[idx - i - (maxColumns * i)].Player.Id == _pieces[idx].Player.Id)
            {
                newIndex = Convert.ToByte(idx - i - (maxColumns * i));
                i++;
                col--;
                row--;
            }
        }
        return CheckDiagonalRightDown(newIndex);
    }

    private bool CheckFordward(short idx)
    {
        var player = _pieces[idx].Player;
        short count = 0;
        short i = _pieces[idx].Column;
        short j = 0;
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

    private bool CheckBackward(short idx)
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

    private bool CheckDownward(short idx)
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

    

    private bool CheckDiagonalRightUp(short idx)
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

    private bool CheckDiagonalLeftUp(short idx)
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

    private bool CheckDiagonalLeftDown(short idx)
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


    private bool CheckDiagonalRightDown(short idx)
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
        return _CheckForWin;

    }

    private WinState _CheckForWin;

    private WinState CheckForWin(short idx)
    {
        //obtener el current player
        if (CheckHorizontal(idx))
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
        if (CheckDiagonaUp(idx))
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

        if (CheckDiagonaDown(idx))
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
        
        return _CheckForWin;
    }

}

public class Ficha {
       

    public short Id { get; set; }

    public Player Player { get; set; }

    public bool IsOccupied { get; set; }

    public short Column { get; set; }
    public short Row { get; set; }

}


public class Player
{
    public int GamesPlayed { get; set; }
    public int Score { get; set; }
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
