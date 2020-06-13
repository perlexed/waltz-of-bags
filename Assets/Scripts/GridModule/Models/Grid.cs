
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

        public bool isVerbose;

        public Grid(int[,] grid)
        {
            _grid = grid.Clone() as int[,];
            _sourceGrid = grid.Clone() as int[,];
        }

        private void Log(string logString)
        {
            if (isVerbose)
            {
                Debug.Log(logString);
            }
        }

        public void Reset()
        {
            _grid = _sourceGrid.Clone() as int[,];
        }

        public int Width => _grid.GetLength(1);

        public int Height => _grid.GetLength(0);

        public int PointsCount => Height * Width;

        public int FreePointsCount
        {
            get
            {
                int freePointsCount = 0;
                
                for (int y = 0; y < Height; y += 1)
                {
                    for (int x = 0; x < Width; x += 1) {
                        if (_grid[y, x] == 0)
                        {
                            freePointsCount++;
                        }
                    }
                }

                return freePointsCount;
            }
        }

        public override string ToString()
        {
            List<string> rows = new List<string>();
            
            for (int y = 0; y < Height; y += 1)
            {

                List<int> rowValues = new List<int>();
                
                for (int x = 0; x < Width; x += 1) {
                    rowValues.Add(_grid[y, x]);
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

                    if (_grid[tileYPosition, tileXPosition] > 0)
                    {
                        Log($"Tile x = {tileXPosition}, y = {tileYPosition} is not empty: {_grid[tileYPosition, tileXPosition]}");
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

                    _grid[tileYPosition, tileXPosition] = 1;
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
    }
}