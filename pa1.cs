using System;
using static System.Console;

namespace Bme121
{
    static class Program
    {
        static int rows, columns;
        static bool[,] board; // true if tile is removed, false if tile is empty
        static int pawnARow = -1;
        static int pawnACol = -1;
        static int pawnBRow = -1;
        static int pawnBCol = -1;
        static int platformRowA = -1;
        static int platformColA = -1;
        static int platformRowB = -1;
        static int platformColB = -1;
        static string nameA, nameB;
        static bool isTurnA = true; // true: turn of player A, false: turn of player B
        static bool continueGame = true; //if false will exit the game
        
        static string[] letters = { "a","b","c","d","e","f","g","h","i","j","k","l",
            "m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
        
        static void Main( )
        {
            Initialization();
            while (continueGame)
            {
                DrawGameBoard();
                PlayerMove();
            }
            WriteLine();
        }
        
        static void DrawGameBoard() //method set up the board
        {
            const string h  = "\u2500"; // horizontal line
            const string v  = "\u2502"; // vertical line
            const string tl = "\u250c"; // top left corner
            const string tr = "\u2510"; // top right corner
            const string bl = "\u2514"; // bottom left corner
            const string br = "\u2518"; // bottom right corner
            const string vr = "\u251c"; // vertical join from right
            const string vl = "\u2524"; // vertical join from left
            const string hb = "\u252c"; // horizontal join from below
            const string ha = "\u2534"; // horizontal join from above
            const string hv = "\u253c"; // horizontal vertical cross
            
            Write("     ");
            for(int index = 0; index < board.GetLength(1); index++)
            {
                Write("{0}   ", letters[index]);
            }
            WriteLine();
                
            // Draw the top board boundary.
            Write( "   " );
            for( int c = 0; c < board.GetLength(1); c ++ )
            {
                if( c == 0 ) Write( tl );
                Write( "{0}{0}{0}", h );
                if( c == board.GetLength(1) - 1 ) Write( "{0}", tr ); 
                else                                Write( "{0}", hb );
            }
            WriteLine( );
            
            // Draw the board rows.
            for( int r = 0; r < board.GetLength(0); r ++ )
            {
                Write( " {0} ", letters[ r ] );
                
                // Draw the row contents.
                for( int c = 0; c < board.GetLength(1); c ++ )
                {
                    if( c == 0 ) Write( v );
                    MarkTile(r,c);
                    Write(v);
                }
                WriteLine( );
                
                // Draw the boundary after the row.
                if( r != board.GetLength(0) - 1 )
                { 
                    Write( "   " );
                    for( int c = 0; c < board.GetLength(1); c ++ )
                    {
                        if( c == 0 ) Write( vr );
                        Write( "{0}{0}{0}", h );
                        if( c == board.GetLength(1) - 1 ) Write( "{0}", vl ); 
                        else                                Write( "{0}", hv );
                    }
                    WriteLine( );
                }
                else
                {
                    Write( "   " );
                    for( int c = 0; c < board.GetLength(1); c ++ )
                    {
                        if( c == 0 ) Write( bl );
                        Write( "{0}{0}{0}", h );
                        if( c == board.GetLength(1) - 1 ) Write( "{0}", br ); 
                        else                                Write( "{0}", ha );
                    }
                    WriteLine( );
                }
            }
        }
        
        // Initialization
        static void Initialization( )
        {
            WriteLine("Welcome to Isolation!");
            WriteLine();
            
            //ask for players' names
            Write( "Enter your name [default Player A]: " );
            nameA = ReadLine( );
            if (nameA.Length == 0) nameA = "Player A";
            WriteLine( "Your name is: {0} and your pawn will be represented by A", nameA);  
            
            Write( "Enter your name [default Player B]: " );
            nameB = ReadLine( );
            if (nameB.Length == 0) nameB = "Player B";
            WriteLine( "Your name is: {0} and your pawn will be represented by B", nameB);
            
            
            bool isValid = false;
            
            //ask for no of rows
            rows = 0;
            while (! isValid)
            {
                Write("Enter number of rows [default 6]: ");
                string response = ReadLine( );
                
                if( response.Length == 0 ) rows = 6;
                else rows = int.Parse(response);
                if ( rows > 4 && rows < 26) isValid = true;
                else
                {
                    WriteLine("Invalid number, 4 < rows < 26");
                }
            }
            WriteLine("rows: {0}", rows);
            
            // ask for no of columns
            columns = 0;
            isValid = false;
            while (! isValid)
            {
                Write("Enter number of columns [default 8]: ");
                string response = ReadLine( );
                
                if( response.Length == 0 ) columns = 8;
                else columns = int.Parse(response);
                if (columns > 4 && columns < 26) isValid = true;
                else 
                {
                    WriteLine("Invalid number, 4 < columns < 26");
                }
            }
            WriteLine("columns: {0}", columns);
            
            board = new bool[rows, columns];
            DrawGameBoard(); // draw board so player can choose platform coordination
            
            platformRowA = pawnARow = (int) Math.Floor((rows - 1) / 2d);
            platformColA = pawnACol = 0;
            platformRowB = pawnBRow = (int) Math.Ceiling((rows - 1) / 2d);
            platformColB = pawnBCol = columns - 1;
            
            //ask for platform positions
            isValid = false;
            while (! isValid)
            {
                Write("Player {0}, please enter your platform row [default c]: ", nameA);
                string response = ReadLine();
                if (response.Length == 0) isValid = true;
                else if (response.Length != 1) 
                {
                    WriteLine("You must enter only 1 coordinate letter");
                    isValid = false;
                }
                if (response.Length == 1) 
                {
                    platformRowA = pawnARow = Array.IndexOf(letters, response);
                    if (platformRowA < 0 || platformRowA >= rows) WriteLine("The platform must be on the gameboard");
                    else isValid = true;
                }
            }
            WriteLine("Platform row of player A: {0}", platformRowA);
            
            isValid = false;
            while (! isValid)
            {
                Write("Player {0}, please enter your platform column [default a]: ", nameA);
                string response = ReadLine();
                if (response.Length == 0) isValid = true;
                else if (response.Length != 1) WriteLine("You must enter only 1 coordinate letter");
                if (response.Length == 1) 
                {
                    platformColA = pawnACol = Array.IndexOf(letters, response);
                    if (platformColA < 0 || platformColA >= columns) WriteLine("The platform must be on the gameboard");
                    else isValid = true;
                }
            }
            WriteLine("Platform column of player A: {0}", platformColA);
            
            isValid = false;
            while (! isValid)
            {
                Write("Player {0}, please enter your platform row [default d]: ", nameB);
                string response = ReadLine();
                if (response.Length == 0) isValid = true;
                else if (response.Length != 1) WriteLine("You must enter only 1 coordinate letter");
                if (response.Length == 1)
                {
                    platformRowB = pawnBRow = Array.IndexOf(letters, response);
                    if (platformRowB < 0 || platformRowB >= rows) WriteLine("The platform must be on the gameboard");
                    else isValid = true;
                }
            }
            WriteLine("Platform row of player B: {0}", platformRowB);
            
            isValid = false;
            while (! isValid)
            {
                Write("Player {0}, please enter your platform column [default h]: ", nameB);
                string response = ReadLine();
                if (response.Length == 0) isValid = true;
                else if (response.Length != 1) WriteLine("You must enter only 1 coordinate letter");
                if ( response.Length == 1)
                {
                    platformColB = pawnBCol = Array.IndexOf(letters, response);
                    if (platformColB < 0 || platformColB >= columns) WriteLine("The platform must be on the gameboard");
                    else if (platformColB == platformColA) WriteLine("Your platform must not coincide with {0}", nameA);
                    else isValid = true;
                }
            }
            WriteLine("Platform column of player B: {0}", platformColB); 
        }
        
        static void MarkTile(int row, int col)
        {
            const string sp = " ";      // space
            const string pa = "A";      // pawn A
            const string pb = "B";      // pawn B
            const string bb = "\u25a0"; // block
            const string fb = "\u2588"; // left half block
            const string lh = "\u258c"; // left half block
            const string rh = "\u2590"; // right half block
            
            if (row == pawnARow && col == pawnACol) Write(sp + pa + sp);
            else if (row == pawnBRow && col == pawnBCol) Write(sp + pb + sp);
            else if (row == platformRowA && col == platformColA) Write(sp +bb + sp);
            else if (row == platformRowB && col == platformColB) Write(sp + bb + sp);
            else if (board[row,col] == false) Write(rh + fb + lh);
            else Write(sp + sp + sp);
        }
        
        static void PlayerMove()
        {
            bool isValid = true;
            
            while (isValid)
            {
                if (isTurnA) Write(nameA); else Write(nameB);
                Write(", enter your move [abcd] or [quit] to exit the game: ");
                string response = ReadLine();
                if (response == "quit") 
                {
                    continueGame = false;
                    return;
                }
                if (response.Length != 4)
                    WriteLine("Invalid input, the move must contain 4 coordinate letters");
                else
                {
                    int nextRow = Array.IndexOf(letters, response.Substring(0,1));
                    int nextCol = Array.IndexOf(letters, response.Substring(1,1));
                    int removeRow = Array.IndexOf(letters, response.Substring(2,1));
                    int removeCol = Array.IndexOf(letters, response.Substring(3,1));
                        
                    if (nextRow >= rows || nextCol >= columns     // out of the board
                    || nextRow == pawnARow && nextCol == pawnACol // coincide with pawn A
                    || nextRow == pawnBRow && nextCol == pawnBCol // coincide with pawn B
                    || board[nextRow, nextCol]                    // if tile is removed, this is true 
                    || isTurnA && (int) Math.Abs(nextRow - pawnARow) > 1
                    || isTurnA && (int) Math.Abs(nextCol - pawnACol) > 1
                    || ! isTurnA && (int) Math.Abs(nextRow - pawnBRow) > 1
                    || ! isTurnA && (int) Math.Abs(nextCol - pawnBCol) > 1) 
                    WriteLine("You can't move there");
                    
                    else if (removeRow >= rows || removeCol >= columns
                    || isTurnA && removeRow == pawnBRow && removeCol == pawnBCol
                    || ! isTurnA && removeRow == pawnARow && removeCol == pawnACol
                    || board[removeRow, removeCol]
                    || removeRow == nextRow && removeCol == nextCol
                    || removeRow == platformRowA && removeCol == platformColA
                    || removeRow == platformRowB && removeCol == platformColB)
                    WriteLine("You can't remove that tile");
                    
                    else
                    {
                        isValid = false;
                        if (isTurnA) 
                        {
                            pawnARow = nextRow; 
                            pawnACol = nextCol;
                        }
                        else 
                        {
                            pawnBRow = nextRow;
                            pawnBCol = nextCol;
                        }
                        board[removeRow, removeCol] = true;
                        isTurnA = ! isTurnA; //switch to player B's turn
                    }
                }
            }
        }
    }
}
