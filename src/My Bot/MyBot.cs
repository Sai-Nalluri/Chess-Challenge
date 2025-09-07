using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    public const int Infinity = 999999999;
    public const int PawnScore = 100;
    public const int KnightScore = 300;
    public const int BishopScore = 350;
    public const int RookScore = 500;
    public const int QueenScore = 900;

    public Move Think(Board board, Timer timer)
    {
        Move bestMove;
        Search(5, Infinity, Infinity, out bestMove, board);
        return bestMove;
    }

    public int Search(int depth, int alpha, int beta, out Move bestMove, Board board)
    {
        bestMove = Move.NullMove;

        // If we reached a leaf node
        if (depth == 0) return Evaluate(board);

        Move[] moves = board.GetLegalMoves();

        // Check for checkmate or stalemate
        if (moves.Count() == 0)
        {
            if (board.IsInCheck())
            {
                return -Infinity + depth; // To prefer shorted checkmates
            }
            return 0;
        }

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -Search(depth - 1, -beta, -alpha, out _, board); // Ignore out in recursion
            board.UndoMove(move);

            if (score >= beta)
            {
                bestMove = move;
                return beta;
            }
            else if (score > alpha)
            {
                alpha = score;
                bestMove = move;
            }
        }

        return alpha;
    }

    public int Evaluate(Board board)
    {
        int score = 0;

        score += CountMaterial(board);

        return score;
    }

    int CountMaterial(Board board)
    {
        int whiteScore = 0;
        int blackScore = 0;

        whiteScore += board.GetPieceList(PieceType.Pawn, true).Count() * PawnScore;
        whiteScore += board.GetPieceList(PieceType.Knight, true).Count() * KnightScore;
        whiteScore += board.GetPieceList(PieceType.Bishop, true).Count() * BishopScore;
        whiteScore += board.GetPieceList(PieceType.Rook, true).Count() * RookScore;
        whiteScore += board.GetPieceList(PieceType.Queen, true).Count() * QueenScore;

        blackScore += board.GetPieceList(PieceType.Pawn, false).Count() * PawnScore;
        blackScore += board.GetPieceList(PieceType.Knight, false).Count() * KnightScore;
        blackScore += board.GetPieceList(PieceType.Bishop, false).Count() * BishopScore;
        blackScore += board.GetPieceList(PieceType.Rook, false).Count() * RookScore;
        blackScore += board.GetPieceList(PieceType.Queen, false).Count() * QueenScore;

        return (whiteScore - blackScore) * (board.IsWhiteToMove ? 1 : -1);
    }
}