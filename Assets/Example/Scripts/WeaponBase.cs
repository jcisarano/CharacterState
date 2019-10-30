using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jcisarano.laser.standard
{

    public class WeaponBase : MonoBehaviour
    {
        protected bool _fireLaser = true;
        public float _laserRange = 10.0f;
        protected LineRenderer _laserLineRenderer;

        public GameObject _laserStart;

        protected GameObject _target;
        protected GameObject _targetGameObject;

        private void Awake()
        {
            Init();
        }

        protected virtual void Init() { }

        public virtual void SetFireStatus(bool shouldFire)
        {
            _fireLaser = shouldFire;
        }

        protected virtual GameObject GetTargetForGameObject(GameObject obj)
        {
            if (obj != null)
            {
                if (_target == null || obj != _targetGameObject)
                {
                    _target = obj.GetComponent<GameObject>();
                    _targetGameObject = obj;
                }

                return _target;
            }

            return null;
        }
    }
}