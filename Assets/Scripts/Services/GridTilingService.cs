using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using GridModule.Models;
using Grid = GridModule.Models.Grid;
using Tile = GridModule.Models.Tile;

namespace Services
{
    using Difficulties = Dictionary<DifficultyEnum, int>;
    
    public class GridTilingService
    {
        private const int NormalDifficultyPointsPercentage = 10;
        
        private readonly Grid _grid;
        private readonly List<TileData> _tiles;
        private readonly DifficultyEnum _difficulty;

        private readonly Difficulties _freePointsByDifficulty;

        public struct TileSetSearchResponse
        {
            public List<Tile> TileSet;

            public bool IsTileSetFound;
        }

        private GridTilingService(Grid grid, DifficultyEnum difficulty)
        {
            _grid = grid;
            _difficulty = difficulty;
            
            _tiles = AvailableTiles.ConvertAll(tile => new TileData
            {
                Tile = tile,
                AvailablePoints = grid.GetAvailablePoints(tile)
            });    


            int hardMinimumFreePoints = GetHardMinimumFreePoints();
            int normalMinimumFreePointsByPercentage =
                (int) Math.Round((double) (100 * NormalDifficultyPointsPercentage) / _grid.FreePointsCount);
            
            _freePointsByDifficulty = new Difficulties
            {
                {DifficultyEnum.Hard, hardMinimumFreePoints},
                {DifficultyEnum.Normal, Math.Max(hardMinimumFreePoints + 2, normalMinimumFreePointsByPercentage)}
            };
            
            // Debug.Log($"Difficulties: hard - {_freePointsByDifficulty[DifficultyEnum.Hard]}, normal - {_freePointsByDifficulty[DifficultyEnum.Normal]}");
        }
        
        /**
         * Maximum attempts number of covering grid with tiles before assuming the full coverage is impossible
         */
        private const int MaxGridGenerationCount = 1000;
        
        private static readonly System.Random Rnd = new System.Random();
        
        private static readonly List<Tile> AvailableTiles = new List<Tile>
        {
            new Tile { Height = 1, Width = 3 },
            new Tile { Height = 3, Width = 1 },
            new Tile { Height = 2, Width = 2 },
            new Tile { Height = 1, Width = 2 },
            new Tile { Height = 2, Width = 1 },
        };

        public static List<Tile> GetTilesForGrid(Grid grid, DifficultyEnum difficulty)
        {
            return (new GridTilingService(grid, difficulty)).GetTiles();
        }

        private List<Tile> GetTiles()
        {
            return GetTilesByFreePoints(_freePointsByDifficulty[_difficulty]).TileSet;
        }
        
        private TileSetSearchResponse GetTilesByFreePoints(int targetEmptyFreePointsCount)
        {
            int gridGenerationCount = 0;
            List<Tile> tileSet;
            
            do
            {
                _grid.Reset();
                tileSet = GetTileSetForTileTypes();
                gridGenerationCount++;
                // Debug.Log(_grid);
                // Debug.Log($"Tiles ({tileSet.Count}): " + string.Join(", ", tileSet));
            } while (_grid.FreePointsCount > targetEmptyFreePointsCount && gridGenerationCount != MaxGridGenerationCount);


            // Debug.Log(grid);
            // Debug.Log(
            //     tileSet.Count > 0
            //         ? $"Grid tiled in {gridGenerationCount} attempts: {string.Join(", ", tileSet)}"
            //         : $"Grid didn't tiled in {gridGenerationCount} attempts"
            // );

            return new TileSetSearchResponse
            {
                TileSet = tileSet,
                IsTileSetFound = _grid.FreePointsCount <= targetEmptyFreePointsCount
            };
        }

        private List<Tile> GetTileSetForTileTypes()
        {
            List<TileData> availableTiles = new List<TileData>();
            foreach (var tile in _tiles)
            {
                availableTiles.Add(tile);
            }

            List<Tile> placedTiles = new List<Tile>();

            while (availableTiles.Count > 0)
            {
                int randomTileIndex = Rnd.Next(availableTiles.Count);
                TileData randomTile = availableTiles[randomTileIndex];
                // Debug.Log($"random {randomTileIndex}/{availableTiles.Count}: {randomTile.Tile}");
           
                bool isTilePlaced = false;
            
                foreach (GridPoint point in randomTile.AvailablePoints)
                {
                    if (_grid.CanTileBePlaced(randomTile.Tile, point))
                    {
                        isTilePlaced = true;
                        _grid.PlaceTile(randomTile.Tile, point);
                        placedTiles.Add(randomTile.Tile);
                        // Debug.Log(randomTile.Tile);
                        // Debug.Log(grid);
                        break;
                    }
                }

                if (!isTilePlaced)
                {
                    // Debug.Log($"Excluding tile index {randomTileIndex}: " + randomTile.Tile);
                    availableTiles.RemoveAt(randomTileIndex);
                }
            }

            return placedTiles;
        }

        private int GetHardMinimumFreePoints()
        {
            int hardDifficultyMinimumFreePoints = 0;

            do
            {
                TileSetSearchResponse searchResult = GetTilesByFreePoints(hardDifficultyMinimumFreePoints);
                if (searchResult.IsTileSetFound)
                {
                    return hardDifficultyMinimumFreePoints;
                }
                
                hardDifficultyMinimumFreePoints++;
            } while (true);
        }
    }
}