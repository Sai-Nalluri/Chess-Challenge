using System;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    Random rnd = new Random();

    Move bestMove;
    Board board;

    public const int Infinity = 999999999;
    const int PawnValue = 100;
    const int KnightValue = 300;
    const int BishopValue = 320;
    const int RookValue = 500;
    const int QueenValue = 900;

    public Move Think(Board boardPara, Timer timer)
    {
        board = boardPara;
        bestMove = Move.NullMove;

        Search(3, true);

        return bestMove;
    }

    public int Search(int depth, bool isRoot = false)
    {
        Move[] moves = board.GetLegalMoves();

        // If we reached a leaf node
        if (depth == 0)
        {
            return Evaluate();
        }

        // If no legal moves
        if (moves.Count() == 0)
        {
            if (board.IsInCheck())
            {
                return -Infinity;
            }
            // Stalemate
            return 0;
        }

        int bestEvalutation = -Infinity;

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            // Go deeper in the tree
            int evaluation = -Search(depth - 1);
            board.UndoMove(move);

            if (evaluation > bestEvalutation)
            {
                bestEvalutation = evaluation;
                // This will return the best move in all the tree, which can be illegal 
                // move or a totally bad one in the current position. So only record 
                // bestEvalution if is in root node
                if (isRoot) bestMove = move;
            }
        }

        return bestEvalutation;
    }

    public int Evaluate()
    {
        int whiteEval = CountMaterial(ChessChallenge.Chess.Board.WhiteIndex);
        int blackEval = CountMaterial(ChessChallenge.Chess.Board.BlackIndex);

        int evaluation = whiteEval - blackEval;
        // We could leave it like this, but they we would not know if this is for black or white
        int perspective = board.IsWhiteToMove ? 1 : -1;

        return evaluation * perspective;
    }

    public int CountMaterial(int colorIndex)
    {
        int material = 0;
        bool isWhite = colorIndex == ChessChallenge.Chess.Board.WhiteIndex ? true : false;
        material += board.GetPieceList(PieceType.Pawn, isWhite).Count * PawnValue;
        material += board.GetPieceList(PieceType.Knight, isWhite).Count * KnightValue;
        material += board.GetPieceList(PieceType.Bishop, isWhite).Count * BishopValue;
        material += board.GetPieceList(PieceType.Rook, isWhite).Count * RookValue;
        material += board.GetPieceList(PieceType.Queen, isWhite).Count * QueenValue;
        return material;
    }
}