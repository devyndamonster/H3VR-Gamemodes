using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamemodes
{
    public class ValueButtonInt : ValueButton
    {
        public int DefaultValue;
        public int MinValue;
        public int MaxValue;
        public int Increment;
        public UnityAction<int> OnValueChanged;

        private int _value;

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value && WithinRange(value))
                {
                    _value = value;
                    ValueText.text = _value.ToString();
                    OnValueChanged.Invoke(value);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _value = DefaultValue;
            ValueText.text = _value.ToString();
        }

        public bool WithinRange(int value)
        {
            return MinValue <= value && MaxValue >= value;
        }

        public override void OnDecreasePressed()
        {
            Value -= Increment;
        }

        public override void OnIncreasePressed()
        {
            Value += Increment;
        }
    }
}

