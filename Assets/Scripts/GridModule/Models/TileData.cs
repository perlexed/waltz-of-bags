using System.Collections.Generic;

namespace GridModule.Models
{
    public class TileData
    {
        public Tile Tile;
        public List<GridPoint> AvailablePoints;
        public bool IsPlaced;
        public GridPoint Coordinates;

        public void SetPlacedPoint(GridPoint coordinates)
        {
            IsPlaced = true;
            Coordinates = coordinates;
        }
    }
}