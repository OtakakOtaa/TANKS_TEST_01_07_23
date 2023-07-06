using System;
using CodeBase.CombatReadinessSystems.Behaviors;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Tanks_Turrets.Visual
{
    public sealed class HeathBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Transform _target;
        [SerializeField] private float _height;

        private IHitable _hitable;

        public void Init(IHitable hitable)
            => _hitable = hitable;

        private void LateUpdate()
        {
            if (_target is null) return;
            
            var position = _target.position;
            position.y += _height;
            transform.position = position;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            _slider.value = _hitable.HeathAmount / _hitable.MaxHeathAmount;
        }
    }
}