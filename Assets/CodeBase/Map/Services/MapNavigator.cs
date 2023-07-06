using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CodeBase.Infrastructure;
using CodeBase.Map.Services.Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CodeBase.Map.Services
{
    public sealed class MapNavigator
    {
        
        [CanBeNull]
        public IEnumerable<Vector3Int> TryFindPath(Vector3Int start, Vector3Int target, LevelMap levelMap, 
            bool ignoreObstacles = false)
        { 
            List<PathNode> checkedNodes = new(); 
            List<PathNode> waitingPool = new();
            
            checkedNodes.Clear();
            waitingPool.Clear();
            
            PathNode startNode = new( ConvertToVector2(start), ConvertToVector2(target),
                previousNode: null, 0);
            
            checkedNodes.Add(startNode);
            waitingPool.AddRange(GetNearbyNodes(startNode, ConvertToVector2(target), levelMap));

            while (waitingPool.Count is not 0)
            {
                var nodeToCheck = waitingPool.FirstOrDefault(n => 
                    n.Heft == waitingPool.Min(pn => pn.Heft));
                if (nodeToCheck is null) throw new Exception();

                var isEnd = (int)nodeToCheck.Position.x == target.x
                            && (int)nodeToCheck.Position.y == target.y;

                if (isEnd) return CalculatePath(nodeToCheck);
                var isObstacle = levelMap.IsObstacle(ConvertToVector3Int(nodeToCheck.Position)) && !ignoreObstacles;
                
                waitingPool.Remove(nodeToCheck);
                if (isObstacle)
                {
                    checkedNodes.Add(nodeToCheck);
                    continue;
                }
                
                if (checkedNodes.Any(n => n.Position == nodeToCheck.Position) is false)
                {
                    checkedNodes.Add(nodeToCheck);
                    waitingPool.AddRange(GetNearbyNodes(nodeToCheck, ConvertToVector2(target), levelMap));
                }
            }
            return null;
        }

        private IEnumerable<PathNode> GetNearbyNodes(PathNode origin, Vector2 target, LevelMap levelMap)
        {
            List<PathNode> nodes = new()
            {
                new PathNode(origin.Position + Vector2.up, target, origin, origin.Remoteness + 1),
                new PathNode(origin.Position + Vector2.down, target, origin, origin.Remoteness + 1),
                new PathNode(origin.Position + Vector2.left, target, origin, origin.Remoteness + 1),
                new PathNode(origin.Position + Vector2.right, target, origin, origin.Remoteness + 1),
            };

            var removeList = new List<PathNode>();
            foreach (var node in nodes)
            {
                bool isIncorrectCellIndex = node.Position.VectorInArrayRange(levelMap.MapData.size) is false;
                if (isIncorrectCellIndex) removeList.Add(node);
            }

            removeList.ForEach(n => nodes.Remove(n));
            return nodes;
        }

        private IEnumerable<Vector3Int> CalculatePath(PathNode root, Stack<PathNode> path = default)
        {
            path ??= new Stack<PathNode>();
            path.Push(root);
            return root.PreviousNode is not null ? 
                CalculatePath(root.PreviousNode, path) 
                : path.Select(n => ConvertToVector3Int(n.Position));
        }

        private Vector3Int ConvertToVector3Int(Vector2 target)
            => new ()
            {
                x = (int)target.x,
                y = (int)target.y,
            };

        private Vector2 ConvertToVector2(Vector3Int target)
            => new ()
            {
                x = target.x,
                y = target.y,
            };

        private sealed class PathNode
        {
            public Vector2 Position { get; }
            public PathNode PreviousNode { get; }
            public int Heft { get; }
            public int Remoteness { get; }

            public PathNode(Vector2 position, Vector2 target, PathNode previousNode, int remoteness)
            {
                Position = position;
                PreviousNode = previousNode;
                Remoteness = remoteness;
                Heft = remoteness * (int)Math.Abs(target.x - position.x) + (int)Math.Abs(target.y - position.y);
            }
        }
    }
}