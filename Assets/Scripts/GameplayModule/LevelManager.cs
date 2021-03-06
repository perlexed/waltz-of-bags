﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Grid = GridModule.Models.Grid;
using GridPoint = GridModule.Models.GridPoint;
using Tile = GridModule.Models.Tile;
using TileData = GridModule.Models.TileData;
using GridModule;
using GridModule.Services;

namespace GameplayModule
{
    public class LevelManager : MonoBehaviour
    {
        public int cartGridWidth;
        
        public GameObject gridContainer;
        public GameObject gridElementPrefab;

        public GameObject bag2X1Prefab;
        public GameObject bag3X1Prefab;
        public GameObject bag2X2Prefab;
        public GameObject luggageCart;

        public List<GameObject> generatedGridElements;

        private InteractionManager _interactionManager;
        private TimelineController _timelineController;

        private Dictionary<string, GameObject> _tileToBagMap;
        private List<Grid> _initializedGrids;
        private BagController[] _bags;
        private bool _shouldRefreshBagsOnUpdate;

        private bool AreAllBagsOnCart => _bags
            .ToList()
            .Aggregate(true, (areAllBagsOnCart, bag) => areAllBagsOnCart && bag.isOnCart);
        
        private bool AreAllBagsOnShelf => _bags
            .ToList()
            .Aggregate(true, (areAllBagsOnCart, bag) => areAllBagsOnCart && bag.isOnShelf);

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
            
            _initializedGrids = InitializedGridList.GetGrids();
        }

        private Grid GetRandomGrid()
        {
            return _initializedGrids[Rnd.Next(_initializedGrids.Count)];
        }

        public void CreateRandomLevel(DifficultyEnum difficulty)
        {
            CreateLevel(GetRandomGrid(), difficulty);
            
            _shouldRefreshBagsOnUpdate = true;
        }

        private void CreateLevel(Grid grid, DifficultyEnum difficulty)
        {
            GenerateShelfGrid(grid);
            
            List<Tile> tiles = GridTilingService.GetTilesForGrid(grid, difficulty);

            // Randomly rotate tiles than can be rotated to get a little variety at bags positions at the cart 
            List<Tile> randomlyRotatedTiles = tiles.ConvertAll(
                tile => tile.CanBeRotated() && (Rnd.Next(2) == 1)
                    ? new Tile{Height = tile.Width, Width = tile.Height}
                    : tile 
            );
            
            List<TileData> arrangedTiles = TilesCombinatorService.PlaceTilesOnCart(randomlyRotatedTiles, cartGridWidth);
            
            GenerateBags(arrangedTiles);
        }

        private void GenerateBags(List<TileData> tiles)
        {
            List<BagController> createdBags = tiles.ConvertAll(CreateBagByTile);
            _bags = createdBags.ToArray();
            
            foreach (var bag in _bags)
            {
                bag.OnBagPickupStatusChangeEvent += OnBagPickupStatusChange;
            }
        }

        private void OnBagPickupStatusChange()
        {
            _timelineController.OnBagPickupStatusChange(AreAllBagsOnShelf, AreAllBagsOnCart);
        }

        public void ClearCreatedInstances()
        {
            foreach (BagController bag in _bags)
            {
                Destroy(bag.gameObject);
            }

            foreach (GameObject generatedGridElement in generatedGridElements)
            {
                Destroy(generatedGridElement);
            }
        }

        private BagController CreateBagByTile(TileData gridTile)
        {
            GameObject bagPrefab = _tileToBagMap[gridTile.Tile.GetStringHash()];

            // We calculated tile positions with the coordinator on it's top left point
            // so we need to offset this bag by it's width/height 
            Vector2 bagAddition = new Vector2(
                (gridTile.Tile.Width - 1) / 2f,
                (gridTile.Tile.Height - 1) / 2f
            );
            Vector2 bagCoordinates = (Vector2) luggageCart.transform.position + gridTile.Coordinates.ToVector() + bagAddition;

            GameObject bagGameObject = Instantiate(
                bagPrefab,
                bagCoordinates,
                bagPrefab.transform.rotation,
                luggageCart.transform
            );

            if (gridTile.Tile.isVertical)
            {
                bagGameObject.transform.Rotate(new Vector3(0, 0, -90));
            }
                
            BagController bag = bagGameObject.GetComponent<BagController>();

            bag.shelfGrid = gridContainer;
            bag.interactionManager = _interactionManager;
            bag.timelineController = _timelineController;
            bag.isOnCart = true;
            bag.RefreshGridElements();

            return bag;
        }

        private void GenerateShelfGrid(Grid grid)
        {
            generatedGridElements = new List<GameObject>();
            
            foreach (GridPoint point in grid.GetEmptyPoints())
            {
                // Mirror grid point down because we're making it grow downward
                Vector2 pointVector = point.ToVector() * new Vector2(1, -1);
                    
                GameObject gridElement = Instantiate(
                    gridElementPrefab,
                    (Vector2) gridContainer.transform.position + pointVector,
                    gridContainer.transform.rotation,
                    gridContainer.transform
                );
                
                gridElement.GetComponent<GridElementController>().interactionManager = _interactionManager;
                generatedGridElements.Add(gridElement);
            }
        }

        public void Update()
        {
            if (_shouldRefreshBagsOnUpdate)
            {
                foreach (BagController bag in _bags)
                {
                    bag.RefreshGridElements();
                }

                _shouldRefreshBagsOnUpdate = false;
            }
        }
    }
}

