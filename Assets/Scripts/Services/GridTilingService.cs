using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridModule.Models;
using Grid = GridModule.Models.Grid;
using Tile = GridModule.Models.Tile;
using Random = System.Random;


namespace Services
{
    public static class GridTilingService
    {
        /**
         * Maximum attempts number of covering grid with tiles before assuming the full coverage is impossible
         */
        private const int MaxGridGenerationCount = 1000;
        
        private static readonly Random Rnd = new Random();
        
        private static readonly List<Tile> AvailableTiles = new List<Tile>
        {
            new Tile { Height = 1, Width = 3 },
            new Tile { Height = 3, Width = 1 },
            new Tile { Height = 2, Width = 2 },
            new Tile { Height = 1, Width = 2 },
            new Tile { Height = 2, Width = 1 },
        };
        
        public static List<Tile> GetTilesForGrid(Grid grid)
        {
            // Debug.Log(grid);

            List<TileData> tiles = AvailableTiles.ConvertAll(tile => new TileData
            {
                Tile = tile,
                AvailablePoints = grid.GetAvailablePoints(tile)
            });

            int targetEmptyFreePointsCount = 0;
            int gridGenerationCount = 0;
            List<Tile> tileSet;
            
            do
            {
                grid.Reset();
                tileSet = GetTileSetForGrid(tiles, grid);
                gridGenerationCount++;
                // Debug.Log(grid);
                // Debug.Log($"Tiles ({tileSet.Count}): " + string.Join(", ", tileSet));
            } while (grid.FreePointsCount > targetEmptyFreePointsCount && gridGenerationCount != MaxGridGenerationCount);


            // Debug.Log(grid);
            Debug.Log(
                tileSet.Count > 0
                    ? $"Grid tiled in {gridGenerationCount} attempts: {string.Join(", ", tileSet)}"
                    : $"Grid didn't tiled in {gridGenerationCount} attempts"
            );
            
            return tileSet;
        }

        private static List<Tile> GetTileSetForGrid(List<TileData> tiles, Grid grid)
        {
            List<TileData> availableTiles = new List<TileData>();
            foreach (var tile in tiles)
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
                    if (grid.CanTileBePlaced(randomTile.Tile, point))
                    {
                        isTilePlaced = true;
                        grid.PlaceTile(randomTile.Tile, point);
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
    }
}