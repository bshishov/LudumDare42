using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIClock : MonoBehaviour
    {
        public RectTransform Arrow;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Arrow != null)
            {
                var t = Placer.Instance.RemainingTime / Placer.Instance.Timer;
                Arrow.localRotation = Quaternion.Euler(
                    Arrow.localRotation.eulerAngles.x,
                    Arrow.localRotation.eulerAngles.y,
                    -t * 360f);
            }
        }
    }
}
