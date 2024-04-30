using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class Spaceship : MonoBehaviour
    {
        public List<Relic> relics;
        public Transform target;
        public int minIndex = -1;
        public int relicCount = 0;
        [SerializeField] private float speed = 20;
        [SerializeField] private LineRenderer line;
        public Color color;
        private void Start()
        {
            FindClosestRelic();
            line.positionCount = 1;
            line.SetPosition(0, transform.position);
            line.material.color = color;
            GetComponent<SpriteRenderer>().material.color = color;
        }
        private void Update()
        {
            if (target == null)
            {
                return;
            }
            MoveTowardsTarget();
            Vector3 dir = target.position - transform.position;
            if (dir.magnitude <= 0.1f && target != null)
            {
                ReachTarget();
            }
        }
        private void ReachTarget()
        {
            Destroy(target.gameObject);
            relics[minIndex] = null;
            target = null;
            FindClosestRelic();
            line.positionCount++;
            relicCount++;
            line.SetPosition(relicCount, transform.position);
        }
        private void MoveTowardsTarget()
        {
            Vector3 dir = target.position - transform.position;
            transform.position += speed * Time.deltaTime * dir.normalized;
        }
        private void FindClosestRelic()
        {
            float distance;
            float min = float.PositiveInfinity;
            minIndex = -1;
            for (int i = 0; i < relics.Count; i++)
            {
                if (relics[i] == null)
                {
                    continue;
                }
                if (relics[i].targeted)
                {
                    continue;
                }
                distance = Vector3.Distance(transform.position, relics[i].transform.position);
                if (min > distance)
                {
                    min = distance;
                    minIndex = i;
                }
            }
            target = relics[minIndex].transform;
            relics[minIndex].targeted = true;
        }
    }
}
