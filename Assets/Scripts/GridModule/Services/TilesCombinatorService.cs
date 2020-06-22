
using System;
using System.Collections.Generic;
using GridModule.Models;
using System.Linq;
using UnityEngine;
using Grid = GridModule.Models.Grid;

namespace GridModule.Services
{
    public static class TilesCombinatorService
    {
        public static List<TileData> PlaceTilesOnCart(List<Tile> tiles, int cartWidth)
        {
            int tilesTotalSquare = tiles.Aggregate(0, (square, tile) => square + tile.Square);

            const int maxCartRows = 20;
            
            int cartMinimumRows = tilesTotalSquare % cartWidth > 0
                ? (int) Math.Ceiling((double) tilesTotalSquare / cartWidth)
                : tilesTotalSquare / cartWidth;


            bool areTilesArrangedOnGrid;
            int cartRows = cartMinimumRows;
            List<TileData> arrangedTiles;

            do
            {
                Grid cartGrid = new Grid(cartWidth, cartRows);

                List<TileData> gridTiles = tiles.ConvertAll(tile => new TileData
                {
                    Tile = tile,
                    AvailablePoints = cartGrid.GetAvailablePoints(tile),
                });

                areTilesArrangedOnGrid = ArrangeTilesToFitGrid(cartGrid, gridTiles);
                arrangedTiles = gridTiles;
            } while (!areTilesArrangedOnGrid && cartRows++ < maxCartRows);
            

            return arrangedTiles;
        }

        private static bool ArrangeTilesToFitGrid(Grid grid, List<TileData> gridTiles)
        {
            foreach (TileData tile in gridTiles)
            {
                foreach (GridPoint point in tile.AvailablePoints)
                {
                    if (grid.CanTileBePlaced(tile.Tile, point))
                    {
                        grid.PlaceTile(tile.Tile, point);
                        tile.SetPlacedPoint(point);
                        break;
                    }
                }
            
                if (!tile.IsPlaced)
                {
                    return false;
                }
            }

            return true;
        }
    }
}