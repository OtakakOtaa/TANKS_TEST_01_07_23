using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Configurations;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.OneEntity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace CodeBase.Map.Services.Core
{
    public sealed class LevelMap
    {
        private readonly LevelMapGenerator _levelMapGenerator;
        private readonly MapLayers _mapLayers;
        private readonly MapNavigator _mapNavigator;

        private readonly List<Vector3Int> _enemySpawnPoints;
        private Vector3Int _playerSpawnPoint;
        private readonly Transform _rootMapObject;
        private readonly Grid _grid;
        private readonly GameConfiguration _gameConfiguration;

        public Configuration Config { get; }
        public Tilemap MapData => _mapLayers.Obstacle;
        

        public LevelMap(LevelMapGenerator levelMapGenerator, MapNavigator mapNavigator,
            GameConfiguration gameConfiguration, MapLayers mapLayers,Transform rootMapObject)
        {
            _mapNavigator = mapNavigator;
            _levelMapGenerator = levelMapGenerator;

            _mapLayers = mapLayers;
            Config = gameConfiguration.MapConfiguration;
            _rootMapObject = rootMapObject;
            _grid = rootMapObject.GetComponent<Grid>();
            _gameConfiguration = gameConfiguration;
            
            _enemySpawnPoints = new Vector3Int[gameConfiguration.EnemyTankCount + gameConfiguration.EnemyTurretCount]
                .ToList();
        }

        public IEnumerable<Vector3> EnemySpawnPoints => _enemySpawnPoints.Select(GetTrueCellPosition);
        public Vector3 PlayerSpawnPoint => GetTrueCellPosition(_playerSpawnPoint);

        public void CreateMap()
        {
            ClearAll();
            SetSize();
            _levelMapGenerator.CreateBox(_mapLayers, Config);
            _levelMapGenerator.GenerateObstacle(_mapLayers, Config, _rootMapObject.gameObject);
            ArrangeSpawnPoints();
            _levelMapGenerator.FixImpassablePlaces(SpawnPoints(), _mapNavigator, this);
            _levelMapGenerator.CreateColliders(_mapLayers.Obstacle);
            ArrangeObstacleBoxes();
        }

        public bool IsObstacle(Vector3Int position)
        {
            if (position.VectorInArrayRange(_mapLayers.Obstacle.size) is false) 
                return false;
            
            var tile = _mapLayers.Obstacle.GetTile(position);
            if (tile is not null) return true;
            
            var hit = Physics2D.CircleCast(GetTrueCellPosition(position), _grid.cellSize.x / 2, Vector2.zero);
            return hit.transform is not null && hit.transform.gameObject.TryGetComponent<GameEntity>(out _);
        }

        public Vector3Int GetRandomFreePlace()
            => _levelMapGenerator.GetFreeRandomPosOnObstacleMap(_mapLayers.Obstacle.size, _mapLayers.Obstacle, _rootMapObject.gameObject);

        public Vector3 GetTrueCellPosition(Vector3Int pos)
            => Vector3.Scale((new Vector3(pos.x, pos.y) + _mapLayers.Obstacle.tileAnchor) - _rootMapObject.position,
                _rootMapObject.transform.localScale);

        [CanBeNull]
        public Vector3Int? GetWorldPosToMap(Vector3 pos)
        {
            var originPos = _rootMapObject.position - (pos - _mapLayers.Obstacle.tileAnchor) * -1;
            var scaleValue = _rootMapObject.transform.localScale;
            var posScale = new Vector3(1 / scaleValue.x, 1 / scaleValue.y);
            pos = Vector3.Scale(originPos, posScale);
            
            var mapPos = new Vector3Int((int)Math.Round(pos.x), (int)Math.Round(pos.y));
            var isCorrectValue = mapPos.VectorInArrayRange(_mapLayers.Obstacle.size);
            if (isCorrectValue is false) return null;
            return mapPos;
        }

        private void SetSize()
        {
            _mapLayers.Ground.size = new Vector3Int(Config.MapSize.x, Config.MapSize.y);
            _mapLayers.Obstacle.size = new Vector3Int(Config.MapSize.x, Config.MapSize.y);
        }

        private void ArrangeSpawnPoints()
        {
            var iterator = 0;
            const int minDistanceFromPlayerValue = 3;  
            var minDistanceFromPlayer = Math.Min(Config.MapSize.x, Config.MapSize.y) / minDistanceFromPlayerValue;

            _playerSpawnPoint = GetPosition(); 
            
            while (iterator < _enemySpawnPoints.Count) 
            {
                var position = GetPosition();

                var spawnCloseToPlayer = Math.Abs(position.x - _playerSpawnPoint.x) < minDistanceFromPlayer ||
                                         Math.Abs(position.y - _playerSpawnPoint.y) < minDistanceFromPlayer;
                
                if(spawnCloseToPlayer) 
                    continue;

                var isPositionNotSpawnPoint = _enemySpawnPoints.Any(p => p == position) is false; 
                if(isPositionNotSpawnPoint)
                    _enemySpawnPoints[iterator++] = position;
            }

            Vector3Int GetPosition() =>
                _levelMapGenerator.GetFreeRandomPosOnObstacleMap(_mapLayers.Obstacle.size, _mapLayers.Obstacle, _rootMapObject.gameObject);
        }

        private void ArrangeObstacleBoxes()
        {
            for (var i = 0; i < _gameConfiguration.BoxCount; i++)
            {
                var posOnMap = _levelMapGenerator.GetFreeRandomPosOnObstacleMap(_mapLayers.Obstacle.size, _mapLayers.Obstacle, _rootMapObject.gameObject);
                var originPos = GetTrueCellPosition(posOnMap);
                var box = Object.Instantiate(_gameConfiguration.ObstacleBox);
                box.transform.position = originPos;
                box.Constructor(_gameConfiguration.BoxHeath);
            }
        }
        
        private void ClearAll()
        {
            _mapLayers.Ground.ClearAllTiles();
            _mapLayers.Obstacle.ClearAllTiles();
        }

        private IEnumerable<Vector3Int> SpawnPoints()
            => new List<Vector3Int>(_enemySpawnPoints) { _playerSpawnPoint };

        [Serializable] public class Configuration
        {
            [Header("Assets")] [SerializeField] private Tile _groundTile;
            [SerializeField] private Tile _obstacleTile;

            [Header("Prompts")] [SerializeField] private Vector2Int _mapSize;
            [SerializeField, Range(0, 40)] private float _freedom;

            public Tile GroundTile => _groundTile;
            public Tile ObstacleTile => _obstacleTile;

            public Vector2Int MapSize => _mapSize;
            public float Freedom => _freedom;
        }

        [Serializable] public class MapLayers
        {
            [SerializeField] private Tilemap _ground;
            [SerializeField] private Tilemap _obstacle;

            public Tilemap Ground => _ground;
            public Tilemap Obstacle => _obstacle;
        }
    }
}