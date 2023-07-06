using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeBase.Tanks_Turrets.Services.Control.Player
{
    public sealed class PlayerInputHandler : MonoBehaviour, IDisposable
    {
        [SerializeField] private Texture2D _cursorAim;
        
        private TankSteering _tankSteering;
        
        private readonly HoldFlag _isMoveInputHold = new(); 
        private readonly HoldFlag _isRotateInputHold = new();
        private readonly HoldFlag _isPointerObserved = new();

        public void Awake()
            => Cursor.SetCursor(_cursorAim, Vector2.one, CursorMode.Auto);

        public void Init(TankSteering tankSteering)
            => _tankSteering = tankSteering;

        public void OnMove(InputAction.CallbackContext context)
        {
            if(_isMoveInputHold.Flag) return;
            _isMoveInputHold.Flag = true;
            ObserveHold(_isMoveInputHold, context, () => _tankSteering.Move((int)context.ReadValue<float>()))
                .Forget();
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if(_isRotateInputHold.Flag) return;
            _isRotateInputHold.Flag = true;
            ObserveHold(_isRotateInputHold, context, () =>_tankSteering.Turn((int)context.ReadValue<float>()))
                .Forget();
        }

        public async void OnLook(InputAction.CallbackContext context)
        {
            if(_isPointerObserved.Flag) return;
            
            _isPointerObserved.Flag = true;
            while (_isPointerObserved.Flag)
            {
                await UniTask.Yield();
                var recPos = context.ReadValue<Vector2>();
                var worldPoint = Camera.main!.ScreenToWorldPoint(recPos);
                _tankSteering.RotateTower(worldPoint);
            }
        }

        public void OnFire(InputAction.CallbackContext context)
            => _tankSteering.TargetTank.Fire();

        private async UniTaskVoid ObserveHold(HoldFlag flag, InputAction.CallbackContext context, Action action)
        {
            while (context.action.IsPressed())
            {
                await UniTask.Yield();
                action?.Invoke();
            }
            flag.Flag = false;
        }

        public void Dispose()
        {
            _isPointerObserved.Flag = false;
            _isMoveInputHold.Flag = false;
            _isRotateInputHold.Flag = false;
        }
        
        private sealed class HoldFlag
        {
            public bool Flag;
        }
    }
}