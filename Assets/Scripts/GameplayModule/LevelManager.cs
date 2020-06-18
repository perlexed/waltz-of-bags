using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using GameplayModule;
using UnityEngine;
using Grid = GridModule.Models.Grid;
using GridPoint = GridModule.Models.GridPoint;
using Tile = GridModule.Models.Tile;
using Services;

using TileSetSearchResponse = Services.GridTilingService.TileSetSearchResponse;

namespace GameplayModule
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject gridContainer;
        public GameObject gridElementPrefab;

        public GameObject bag2X1Prefab;
        public GameObject bag3X1Prefab;
        public GameObject bag2X2Prefab;
        public GameObject luggageCart;

        private InteractionManager _interactionManager;
        private TimelineController _timelineController;

        private Dictionary<string, GameObject> _tileToBagMap;
        private List<Grid> _grids;
        public List<GameObject> generatedGridElements;

        private static readonly System.Random Rnd = new System.Random();

        private void Start()
        {
            _interactionManager = gameObject.GetComponent<InteractionManager>();
            _timelineController = gameObject.GetComponent<TimelineController>();
            
            _tileToBagMap = new Dictionary<string, GameObject>
            {
                {"2X1", bag2X1Prefab},
                {"1X2", bag2X1Prefab},
                {"3X1", bag3X1Prefab},
                {"1X3", bag3X1Prefab},
                {"2X2", bag2X2Prefab},
            };
            
            _grids = GetInitializedGrids();
        }

        private Grid GetRandomGrid()
        {
            return _grids[Rnd.Next(_grids.Count)];
        }
        
        private static List<Grid> GetInitializedGrids()
        {
            return new List<Grid>
            {
                new Grid(new [,] {
                    {0, 0, 0, 0, 1, 0},
                    {0, 1, 0, 0, 0, 0},
                    {1, 1, 1, 1, 1, 1},
                    {1, 1, 0, 0, 0, 0},
                }),
                new Grid(new[,] {
                    {0, 0, 1, 0, 0, 0},
                    {0, 0, 1, 0, 0, 0},
                    {1, 1, 1, 1, 1, 1},
                    {0, 0, 0, 1, 0, 0},
                }),
                new Grid(new [,] {
                    {0, 1, 0, 0, 1, 0},
                    {0, 0, 0, 0, 0, 0},
                }),
                new Grid(new [,] {
                    {0, 0, 0, 0, 0, 0},
                    {0, 1, 0, 0, 0, 0},
                }),
            };
        }

        public void CreateRandomLevel(DifficultyEnum difficulty)
        {
            CreateLevel(GetRandomGrid(), difficulty);
        }

        private void CreateLevel(Grid grid, DifficultyEnum difficulty)
        {
            GenerateShelfGrid(grid);
            
            TileSetSearchResponse response = GridTilingService.GetTilesForGrid(grid, difficulty);
            
            grid.Reset();
            
            GenerateBags(response.TileSet);
        }

        private void GenerateBags(List<Tile> tiles)
        {
            List<BagController> createdBags = new List<BagController>();            

            foreach (Tile tile in tiles)
            {
                GameObject bagPrefab = _tileToBagMap[tile.GetStringHash()];

                GameObject bagGameObject = Instantiate(bagPrefab, luggageCart.transform.position,
                    luggageCart.transform.rotation, luggageCart.transform);

                BagController bag = bagGameObject.GetComponent<BagController>();

                bag.shelfGrid = gridContainer;
                bag.interactionManager = _interactionManager;
                bag.timelineController = _timelineController;
                bag.RefreshGridElements();
                
                createdBags.Add(bag);
            }

            _timelineController.InitBags(createdBags.ToArray());
        }

        private void GenerateShelfGrid(Grid grid)
        {
            generatedGridElements = new List<GameObject>();
            
            foreach (GridPoint point in grid.GetEmptyPoints())
            {
                Vector2 containerPosition = gridContainer.transform.position;
                Vector2 gridElementPosition = new Vector2(point.X + containerPosition.x, -1 * point.Y + containerPosition.y);
                    
                GameObject gridElement = Instantiate(gridElementPrefab, gridElementPosition, gridContainer.transform.rotation, gridContainer.transform);
                gridElement.GetComponent<GridElementController>().interactionManager = _interactionManager;
                generatedGridElements.Add(gridElement);
            }
        }
    }
}

