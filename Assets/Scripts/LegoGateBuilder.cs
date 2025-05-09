using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class LegoGateBuilder
    {
        private Grid _grid;
        private LegoGateBlock _block;
        
        private List<LegoGate> _gates = new();
        private List<WallStruct> verticalWalls = new();
        private List<WallStruct> horizontalWalls = new();

        public LegoGateBuilder(Grid grid, LegoGateBlock block)
        {
            _grid = grid;
            _block = block;
        }

        public void GenerateBlockingWallCoordinates()
        {
            var cols = _grid.GridItems.GetLength(0);
            var rows = _grid.GridItems.GetLength(1);

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    TryAddNeighborEdgeAsBlockingWall(x,y+1,false,horizontalWalls);
                    TryAddNeighborEdgeAsBlockingWall(x+1,y,true,verticalWalls);
                    TryAddNeighborEdgeAsBlockingWall(x,y-1,false,horizontalWalls);
                    TryAddNeighborEdgeAsBlockingWall(x-1,y,true,verticalWalls);
                }
            }
            
            var gateBlocks = new List<LegoGate>();
            IterateWalls(rows, horizontalWalls, gateBlocks, false,
                (wallStruct)=> wallStruct.y, 
                (wallStruct)=> wallStruct.x);
            IterateWalls(cols, verticalWalls, gateBlocks, true,
                (wallStruct)=> wallStruct.x, 
                (wallStruct)=> wallStruct.y);

            _gates.AddRange(gateBlocks);
        }

        private void TryAddNeighborEdgeAsBlockingWall(int x, int y,bool isVertical, List<WallStruct> wallList)
        {
            if (!_grid.IsTileDisabled(x,y) && !HasLegoGate(x,y))
            {
                wallList.Add(new WallStruct(isVertical,x,y));
            }
        }

        private bool HasLegoGate(int x, int y)
        {
            foreach (var legoGate in _gates)
            {
                var gatePosition = legoGate.GatePosition;
                if(legoGate.isVertical ?
                       gatePosition.x == x && y >= gatePosition.y && y < gatePosition.y + legoGate.Height:
                       gatePosition.y == y && x >= gatePosition.x && x < gatePosition.x + legoGate.Height
                       ) return true;
            }

            return false;
        }

        private void IterateWalls(int limit, List<WallStruct> horizontalWalls, List<LegoGate> gateBlocks,
            bool verticality,Func<WallStruct,int> getGroupValue, Func<WallStruct,int> getOrderValue )
        {
            for (var y = -1; y <= limit; y++)
            {
                var equalYWalls = horizontalWalls.FindAll(wallStruct=> getGroupValue(wallStruct) == y);
                if(equalYWalls.Count == 0) continue;
                equalYWalls.Sort();
             
                var height = 1;
               
                var needStartIndexUpdate = true;
                var startIndex = (equalYWalls[0].x, equalYWalls[0].y);
                for (var i = 0; i < equalYWalls.Count; i++)
                {
                    var currentWall = equalYWalls[i];
                   
                    if (needStartIndexUpdate) startIndex = (currentWall.x, currentWall.y);
;
                    if( i + 1 < equalYWalls.Count && getOrderValue(equalYWalls[i+1]) == getOrderValue(currentWall) + 1 )
                    {
                        needStartIndexUpdate = false;
                        height++;
                    }
                    else
                    {
                        gateBlocks.Add(new LegoGate(_block, startIndex.x, startIndex.y, height,LegoPiece.LegoColor.GRAY,verticality, false,LegoGate.GateType.BLOCKER));
                        needStartIndexUpdate = true;
                        height = 1;
                    }
                }
            }
        }

        private struct WallStruct : IComparable<WallStruct>
        {
            public bool isVertical;
            public int x;
            public int y;
            
            public WallStruct(bool isVertical, int x, int y)
            {
                this.isVertical = isVertical;
                this.x = x;
                this.y = y;
            }

            public int CompareTo(WallStruct other)
            {
                return isVertical ? y > other.y ? 1 : y < other.y ? -1 :
                    0 : x > other.x ? 1 : x < other.x ? -1 : 0;
            }
        }

        public void GenerateLegoWalls(int xposition, int yposition, int width, LegoPiece.LegoColor color, bool isVertical)
        {
            var targetWall = new LegoGate(_block, xposition, yposition, width, color, isVertical,true,LegoGate.GateType.GATE);
            _gates.Add(targetWall);
        }

        public List<LegoGate> GetAllGates()
        {
            return _gates;
        }
    }
}