using System;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Data.Configs
{
    [Serializable] public sealed class MotionConfiguration
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotationSpeed;
        
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
    }
}