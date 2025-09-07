using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Move Action", menuName = "AI/Commands/Move", order = 100)]
    public class MoveCommand : ActionBase
    {
        [SerializeField] private float radiusMultiplier = 3.5f;
        [SerializeField] private float unitOffset = 3.5f;
        
        private int _unitsOnLayer = 0;
        private int _maxUnitsOnLayer = 1;
        private float _circleRadius = 0;
        private float _radialOffset = 0;
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is AbstractUnit;
        }

        public override void Handle(CommandContext context)
        {
            AbstractUnit unit = (AbstractUnit)context.Commandable;

            if (context.UnitIndex == 0)
            {
                _unitsOnLayer = 0;
                _maxUnitsOnLayer = 1;
                _circleRadius = 0;
                _radialOffset = 0;
            }
            
            Vector3 targetPosition = new (
                context.Hit.point.x + _circleRadius * Mathf.Cos(_radialOffset * _unitsOnLayer), 
                context.Hit.point.y, 
                context.Hit.point.z + _circleRadius * Mathf.Sin(_radialOffset * _unitsOnLayer)
            );
                    
            unit.MoveTo(targetPosition);
            _unitsOnLayer++;
                    
            if (_unitsOnLayer >= _maxUnitsOnLayer)
            {
                _unitsOnLayer = 0;
                _circleRadius += unit.AgentRadius * radiusMultiplier;
                _maxUnitsOnLayer = Mathf.FloorToInt(2 * Mathf.PI * _circleRadius / (unit.AgentRadius * 2 + unitOffset));
                _radialOffset = 2 * Mathf.PI / _maxUnitsOnLayer;
            }
        }
    }
}