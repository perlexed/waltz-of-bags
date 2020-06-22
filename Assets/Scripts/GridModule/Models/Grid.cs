
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridModule.Models
{
    public class Grid
    {
        private int[,] _grid;

        private readonly int[,] _sourceGrid;

        /**
         * Create empty grid with the given width and height
         */
        public Grid(int width, int height)
        {
            int[,] grid = new int[height, width];
            
            _grid = grid.Clone() as int[,];
            _sourceGrid = grid.Clone() as int[,];
        }
        
        public Grid(int[,] grid)
        {
            _grid = grid.Clone() as int[,];
            _sourceGrid = grid.Clone() as int[,];
        }

        public Grid Clone()
        {
            return new Grid(_grid.Clone() as int[,]);
        }

        private void Log(string logString)
        {
            // Debug.Log(logString);
        }

        public void Reset()
        {
            _grid = _sourceGrid.Clone() as int[,];
        }

        private int Width => _grid.GetLength(1);

        private int Height => _grid.GetLength(0);

        private int PointsCount => Height * Width;

        public int FreePointsCount
        {
            get
            {
                int freePointsCount = 0;
                
                for (int y = 0; y < Height; y += 1)
                {
                    for (int x = 0; x < Width; x += 1) {
                        if (GetAt(x, y) == 0)
                        {
                            freePointsCount++;
                        }
                    }
                }

                return freePointsCount;
            }
        }

        private int GetAt(int x, int y)
        {
            return _grid[y, x];
        }

        private void SetAt(int x, int y, int value)
        {
            _grid[y, x] = value;
        }

        public override string ToString()
        {
            List<string> rows = new List<string>();
            
            for (int y = 0; y < Height; y += 1)
            {

                List<int> rowValues = new List<int>();
                
                for (int x = 0; x < Width; x += 1) {
                    rowValues.Add(GetAt(x, y));
                }
                
                rows.Add(string.Join("\t", rowValues));
            }

            return $"Grid {FreePointsCount}/{PointsCount}:\n" + string.Join("\n", rows);
        }

        public bool CanTileBePlaced(Tile tile, GridPoint point)
        {
            if (point.X >= Width || point.Y >= Height)
            {
                Log($"Target point is out of grid, x = {point.X} (width = {Width}), y = {point.Y} (height = {Height})");
                return false;
            }

            for (int y = 0; y < tile.Height; y += 1)
            {
                for (int x = 0; x < tile.Width; x += 1)
                {
                    int tileYPosition = y + point.Y;
                    int tileXPosition = x + point.X;

                    if (tileYPosition > Height || tileXPosition > Width)
                    {
                        Log($"Tile point is out of grid, x = {tileXPosition} (width = {Width}), y = {tileYPosition} (height = {Height}");
                        return false;
                    }

                    if (GetAt(tileXPosition, tileYPosition) > 0)
                    {
                        Log($"Tile x = {tileXPosition}, y = {tileYPosition} is not empty: {GetAt(tileXPosition, tileYPosition)}");
                        return false;
                    }
                }
            }

            return true;
        }

        public bool PlaceTile(Tile tile, GridPoint point)
        {
            if (!CanTileBePlaced(tile, point))
            {
                return false;
            }
            
            for (int y = 0; y < tile.Height; y += 1)
            {
                for (int x = 0; x < tile.Width; x += 1)
                {
                    int tileYPosition = y + point.Y;
                    int tileXPosition = x + point.X;

                    SetAt(tileXPosition, tileYPosition, 1);
                }
            }

            return true;
        }

        public List<GridPoint> GetAvailablePoints(Tile tile)
        {
            List<GridPoint> availablePoints = new List<GridPoint>();
            
            int xMaxCount = Width - tile.Width;
            int yMaxCount = Height - tile.Height;
            
            for (int y = 0; y <= yMaxCount; y += 1)
            {
                for (int x = 0; x <= xMaxCount; x += 1) {
                    availablePoints.Add(new GridPoint{X = x, Y = y});
                }
            }

            return availablePoints;
        }

        public List<GridPoint> GetEmptyPoints()
        {
            List<GridPoint> emptyPoints = new List<GridPoint>();
            
            for (int y = 0; y < Height; y += 1)
            {
                for (int x = 0; x < Width; x += 1) {
                    if (GetAt(x, y) == 0)
                    {
                        emptyPoints.Add(new GridPoint{X = x, Y = y});
                    }
                }
            }

            return emptyPoints;
        }
    }
}