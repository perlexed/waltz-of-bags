using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = GridModule.Models.Grid;

namespace Services
{
    public class ShelfGridGenerator : MonoBehaviour
    {
        public Transform gridContainerTransform;
        public GameObject gridElementPrefab;

        private InteractionManager _interactionManager;
        
        private static readonly int[,] DefaultLevelGrid = {
            {1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1},
        };

        private void Start()
        {
            _interactionManager = gameObject.GetComponent<InteractionManager>();
            
            GenerateShelfGrid(DefaultLevelGrid);
        }

        public void GenerateShelfGrid(int[,] sourceGrid)
        {
            int[,] gridBase = {
                {0, 0, 0, 0},
                {1, 0, 0, 0},
            };
            
            Grid grid = new Grid(gridBase);

            for (int i = 0; i < 20; i++)
            {
                GridTilingService.GetTilesForGrid(grid);
            }
            
            
            for (int y = 0; y < sourceGrid.GetLength(0); y += 1) {
                for (int x = 0; x < sourceGrid.GetLength(1); x += 1) {
                    if (sourceGrid[y, x] == 0)
                    {
                        continue;
                    }

                    Vector2 containerPosition = gridContainerTransform.position;
                    Vector2 gridElementPosition = new Vector2(x + containerPosition.x, -1 * y + containerPosition.y);
                    
                    GameObject gridElement = Instantiate(gridElementPrefab, gridElementPosition, gridContainerTransform.rotation, gridContainerTransform);
                    gridElement.GetComponent<GridElementController>().interactionManager = _interactionManager;
                }
            }
        }
    }
}

