using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.OneEntity;
using CodeBase.Map.Services.Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace CodeBase.Map.Services
{
    public sealed class LevelMapGenerator
    {
        public void GenerateObstacle(LevelMap.MapLayers mapLayers, LevelMap.Configuration config, GameObject root)
        {
            var mapSize = mapLayers.Obstacle.size;

            var borderCount = (mapSize.y + mapSize.x) * 2 - 4;
            var allCellsCount = mapSize.y * mapSize.x - borderCount;

            var allowedObstaclesCount = (config.Freedom * allCellsCount) / 100;
            int generatedObstaclesCount = default;

            List<Vector3Int> spots = new();

            const int spotFrequencyValue = 6;
            var allowedSpotsCount = allCellsCount / spotFrequencyValue;

            while (generatedObstaclesCount <= allowedObstaclesCount)
            {
                var isSpotsNotArranged = spots.Count < allowedSpotsCount;
                if (isSpotsNotArranged)
                    CreateSpotOrigin(mapSize, mapLayers.Obstacle, config.ObstacleTile, spots,
                        ref generatedObstaclesCount, root);
                else
                    ExtendRandomSpot(spots, mapSize, mapLayers.Obstacle, config.ObstacleTile,
                        ref generatedObstaclesCount);
            }
        }

        public void CreateBox(LevelMap.MapLayers mapLayers, LevelMap.Configuration config)
        {
            ApplyToMap(mapLayers.Ground, (x, y)
                => mapLayers.Ground.SetTile(new Vector3Int(x, y), config.GroundTile));

            ApplyToMap(mapLayers.Obstacle, (x, y)
                =>
            {
                var size = mapLayers.Ground.size;
                var isMapEdge = (x == size.x - 1 || y == size.y - 1) || (x is 0 || y is 0);
                if (isMapEdge)
                    mapLayers.Obstacle.SetTile(new Vector3Int(x, y), config.ObstacleTile);
            });
        }

        public Vector3Int GetFreeRandomPosOnObstacleMap(Vector3Int mapSize, Tilemap map, GameObject rootObject)
        {
            Vector3Int pos = default;
         
            var grid = map.GetComponentInParent<Grid>() ?? map.GetComponent<Grid>();
            if (grid is null) throw new Exception();

            var isRandomPosFree = false;
            while (isRandomPosFree is false)
            {
                Random random = new();
                pos = new Vector3Int(random.Next(0, mapSize.x - 2), random.Next(0, mapSize.y - 2));
                isRandomPosFree = map.GetTile(pos) is null;

                if (!isRandomPosFree) continue;
                
                var hit = Physics2D.CircleCast(GetTrueCellPosition(pos, map, rootObject), grid.cellSize.x / 2, Vector2.zero);
                isRandomPosFree = hit.transform is null || 
                                  hit.transform.gameObject.TryGetComponent<GameEntity>(out _) is false;
            }

            return pos;
        }

        public void FixImpassablePlaces(IEnumerable<Vector3Int> spawns, MapNavigator mapNavigator, LevelMap map)
        {
            Vector3Int? root = null;
            foreach (var spawn in spawns)
            {
                root ??= spawn;
                var path = mapNavigator
                    .TryFindPath((Vector3Int)root, spawn, map, ignoreObstacles: true);
                foreach (var position in path)
                    if (map.IsObstacle(position))
                        map.MapData.SetTile(position, null);
            }
        }

        public void CreateColliders(Tilemap map)
        {
            var grid = map.GetComponentInParent<Grid>() ?? map.GetComponent<Grid>();
            if (grid is null) throw new Exception();
            
            ApplyToMap(map, (x, y) =>
            {
                var tile = map.GetTile(new Vector3Int(x, y));
                if (tile is not null)
                {
                    var box = map.gameObject.AddComponent<BoxCollider2D>();
                    box.size = grid.cellSize;
                    box.offset = new Vector3(x, y, 0) + map.tileAnchor;
                }
            });
        }
        
        private void CreateSpotOrigin(Vector3Int mapSize, Tilemap map, Tile obstacle,
            List<Vector3Int> spots, ref int generatedObstaclesCount, GameObject root)
        {
            var spotOrigin = GetFreeRandomPosOnObstacleMap(mapSize, map, root);
            map.SetTile(spotOrigin, obstacle);

            spots.Add(spotOrigin);
            generatedObstaclesCount++;
        }

        private void ExtendRandomSpot(List<Vector3Int> spots, Vector3Int mapSize, Tilemap map,
            Tile obstacle, ref int generatedObstaclesCount)
        {
            var randomSpot = spots[new Random().Next(0, spots.Count - 1)];
            const int percentageGoByHorizontal = 20;

            Vector3Int cachedPosition;
            var depth = 1;
            var direction = 1;
            while (true)
            {
                var nowGoByHorizontal = new Random().Next(0, 100) <= percentageGoByHorizontal;
                var tile = GoSide(depth, nowGoByHorizontal is false, direction, out var isExitOfMap,
                    out cachedPosition, randomSpot, mapSize, map);
                if (isExitOfMap)
                {
                    direction = direction / direction * -1;
                    depth = 0;
                    continue;
                }

                if (tile is not null)
                {
                    depth++;
                    continue;
                }

                map.SetTile(cachedPosition, obstacle);
                generatedObstaclesCount++;
                return;
            }
        }

        private void ApplyToMap(Tilemap target, Action<int, int> iteration)
        {
            for (var i = 0; i < target.size.x; i++)
            {
                for (var j = 0; j < target.size.y; j++)
                    iteration?.Invoke(i, j);
            }
        }

        [CanBeNull]
        private TileBase GoSide(int depth, bool isVertical, int direction,
            out bool isExitOfMap, out Vector3Int cachedPosition,
            Vector3Int randomSpot, Vector3Int mapSize, Tilemap map)
        {
            isExitOfMap = default;

            direction = Math.Clamp(direction, -1, 1);
            if (direction > 0) direction += depth - 1;
            if (direction < 0) direction -= depth - 1;

            cachedPosition = isVertical
                ? randomSpot + new Vector3Int(0, direction)
                : randomSpot + new Vector3Int(direction, 0);

            if (cachedPosition.y >= mapSize.y || cachedPosition.x >= mapSize.x
                                              || cachedPosition.y < 0 || cachedPosition.x < 0)
                isExitOfMap = true;

            return map.GetTile(cachedPosition);
        }
        
        private Vector3 GetTrueCellPosition(Vector3Int pos, Tilemap tilemap, GameObject rootMapObject)
            => Vector3.Scale((new Vector3(pos.x, pos.y) + tilemap.tileAnchor) - rootMapObject.transform.position,
                rootMapObject.transform.localScale);
    }
}