using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour {
    public Strafing strafing;
    public List<StrafePoint> path;
    public StrafePoint strafeStatePoint;

    private class AStarNodeData {
        public StrafePoint parent;
        public float g;
        public float h;
        public float f => g + h;
        public AStarNodeData(StrafePoint parent, float g, float h) {
            this.parent = parent;
            this.g = g;
            this.h = h;
        }
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private StrafePoint GetClosestPoint() {
        StrafePoint closestPoint = null;
        float closestDistance = 9999999f;
        foreach (StrafePoint point in strafing.strafePoints) {
            float currentDistance = Vector3.Distance(point.transform.position, transform.position);
            if (currentDistance < closestDistance) {
                closestPoint = point;
                closestDistance = currentDistance;
            }
        }
        return closestPoint;
    }

    private StrafePoint GetClosestPointToTarget(GameObject target) {
        StrafePoint closestPoint = null;
        float closestDistance = 9999999f;
        foreach (StrafePoint point in strafing.strafePoints) {
            float currentDistance = Vector3.Distance(point.transform.position, target.transform.position);
            if (currentDistance < closestDistance) {
                closestPoint = point;
                closestDistance = currentDistance;
            }
        }
        return closestPoint;
    }

    public bool IsTargetInGap(GameObject target, float gapDistance) {
        if(path == null || path.Count == 0) return false;
        StrafePoint endPoint = path[path.Count-1];
        float distanceToTarget = Vector3.Distance(endPoint.transform.position, target.transform.position);
        if (distanceToTarget > gapDistance) {
            return false;
        }
        return true;
    }

    public List<StrafePoint> GetAStarPath(GameObject target) {
        path = new List<StrafePoint>();
        StrafePoint startPoint = GetClosestPoint();
        StrafePoint endPoint = GetClosestPointToTarget(target);

        Dictionary<StrafePoint, AStarNodeData> nodesData = new Dictionary<StrafePoint, AStarNodeData>();
        List<StrafePoint> openList = new List<StrafePoint>();
        HashSet<StrafePoint> closedSet = new HashSet<StrafePoint>();

        nodesData.Add(startPoint, new AStarNodeData(null, 0, Vector3.Distance(startPoint.transform.position, endPoint.transform.position)));
        openList.Add(startPoint);

        while (openList.Count > 0) {
            StrafePoint currentPoint = openList[0];
            foreach (StrafePoint point in openList) {
                if (nodesData[point].f < nodesData[currentPoint].f) {
                    currentPoint = point;
                }
            }
            if (currentPoint == endPoint) {
                path = new List<StrafePoint>();
                while (currentPoint != null) {
                    currentPoint.selected = true;
                    currentPoint.DebugPaint();
                    path.Add(currentPoint);
                    currentPoint = nodesData[currentPoint].parent;
                }
                path.Reverse();
                return path;
            }
            openList.Remove(currentPoint);
            closedSet.Add(currentPoint);

            foreach (StrafePoint neighbor in currentPoint.neighbors) {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = nodesData[currentPoint].g + Vector3.Distance(currentPoint.transform.position, neighbor.transform.position);

                if (!openList.Contains(neighbor) || tentativeG < nodesData[neighbor].g) {
                    nodesData[neighbor] = new AStarNodeData(currentPoint, tentativeG, Vector3.Distance(neighbor.transform.position, endPoint.transform.position));

                    if (!openList.Contains(neighbor)) {
                        openList.Add(neighbor);
                    }
                }

            }
        }
        path = new List<StrafePoint>();
        return path = new List<StrafePoint>();
    }

    public void ClearPath() {
        if(path == null || path.Count == 0) return;
        foreach (StrafePoint point in path) {
            point.selected = false;
            point.DebugPaint();
        }
        path.Clear();
    }

}
