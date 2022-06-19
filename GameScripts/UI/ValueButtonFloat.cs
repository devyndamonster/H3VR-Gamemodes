using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamemodes
{
    public class ValueButtonFloat : ValueButton
    {
        public float DefaultValue;
        public float MinValue;
        public float MaxValue;
        public float Increment;
        public UnityAction<float> OnValueChanged;

        private float _value;

        public float Value
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
            Value = DefaultValue;
        }

        public bool WithinRange(float value)
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

