using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jcisarano.laser.standard
{

    public class Laser : WeaponBase
    {
        public int _damage = 1;

        protected override void Init()
        {
            base.Init();

            _laserLineRenderer = GetComponentInChildren<LineRenderer>();
            if (_laserLineRenderer == null)
            {
                //nothing to do here
                Destroy(this);
            }
        }

        void Update()
        {
            DrawLaser();
        }

        protected virtual void DrawLaser()
        {
            if (_fireLaser && _laserLineRenderer != null)
            {
                _laserLineRenderer.enabled = true;

                _laserLineRenderer.startWidth = 0.03f;
                _laserLineRenderer.endWidth = 0.06f;

                Vector3 origin = _laserStart.transform.position;
                Vector3 direction = _laserStart.transform.forward;
                Vector3 endpoint = origin + direction * _laserRange;

                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit, _laserRange))
                {
                    endpoint = hit.point;

                    //Target tt = GetTargetForGameObject(hit.collider.gameObject);
                    //tt?.TakeDamage(_damage);
                }

                _laserLineRenderer.SetPosition(0, origin);
                _laserLineRenderer.SetPosition(1, endpoint);
            }
            else if (_laserLineRenderer != null)
            {
                _laserLineRenderer.enabled = false;
            }
        }
    }
}