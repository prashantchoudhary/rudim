﻿using Rudim.Board;
using Rudim.Common;

namespace Rudim.Search
{
    static class Quiescent
    {
        public static int Nodes = 0;
        public static int Search(BoardState boardState, int alpha, int beta)
        {
            Nodes++;

            var eval = SimpleEvaluation.Evaluate(boardState);

            if (eval >= beta)
                return beta;
            if (eval > alpha)
                alpha = eval;

            boardState.GenerateMoves();
            // TODO : Flag in GenerateMoves to avoid extra iteration?
            foreach (var move in boardState.Moves)
            {
                MoveOrdering.PopulateMoveScore(move, boardState);
            }
            MoveOrdering.SortMoves(boardState);


            for (var i = 0; i < boardState.Moves.Count; ++i)
            {
                var move = boardState.Moves[i];
                if (!move.IsCapture())
                    break; // If sorted, once a quiet move is reached we won't need to visit the remaining nodes

                boardState.MakeMove(move);
                if (boardState.IsInCheck(boardState.SideToMove.Other()))
                {
                    boardState.UnmakeMove(move);
                    continue;
                }
                var score = -Search(boardState, -beta, -alpha);
                boardState.UnmakeMove(move);

                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
            }
            return alpha;
        }
    }
}
