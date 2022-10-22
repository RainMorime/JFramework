using System.Collections.Generic;
using UnityEngine;

namespace JYJFramework.Pathfinding
{
    public class AStarManager : Singleton<AStarManager>
    {
        private int width;
        private int height;
        private AStarNode[,] nodes;
        private List<AStarNode> openList;
        private List<AStarNode> closeList;

        public void InitScene(int width, int height)
        {
            this.width = width;
            this.height = height;
            nodes = new AStarNode[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? ANodeType.Stop : ANodeType.Move);
                    nodes[i, j] = node;
                }
            }
        }

        public List<AStarNode> FindPath(Vector2 origin, Vector2 target)
        {
            if (origin.x < 0 || origin.x >= width || origin.y < 0 || origin.y >= height ||
                target.x < 0 || target.x >= width || target.y < 0 || target.y >= height)
            {
                Debug.LogWarning("开始或结束点在地图格子范围外!");
                return null;
            }

            AStarNode originNode = nodes[(int)origin.x, (int)origin.y];
            AStarNode targetNode = nodes[(int)target.x, (int)target.y];
            if (originNode.type == ANodeType.Stop || targetNode.type == ANodeType.Stop)
            {
                Debug.LogWarning("开始或结束点被阻挡！");
                return null;
            }

            closeList.Clear();
            openList.Clear();
            originNode.Clear();
            closeList.Add(originNode);

            while (true)
            {
                FindNearlyNodeToOpenList(originNode.x - 1, originNode.y - 1, 1.4f, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x, originNode.y - 1, 1, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x + 1, originNode.y - 1, 1.4f, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x - 1, originNode.y, 1, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x + 1, originNode.y, 1, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x - 1, originNode.y + 1, 1.4f, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x, originNode.y + 1, 1, originNode, targetNode);
                FindNearlyNodeToOpenList(originNode.x + 1, originNode.y + 1, 1.4f, originNode, targetNode);

                if (openList.Count == 0)
                {
                    Debug.Log("死路");
                    return null;
                }

                openList.Sort(SortOpenList);
                Debug.Log("****************");
                foreach (var node in openList)
                {
                    Debug.Log("点" + node.x + "," + node.y + ":g=" + node.g + "h=" + node.h + "f=" + node.f);
                }

                closeList.Add(openList[0]);
                originNode = openList[0];
                openList.RemoveAt(0);

                if (originNode == targetNode)
                {
                    List<AStarNode> path = new List<AStarNode>();
                    path.Add(targetNode);
                    while (targetNode.father != null)
                    {
                        path.Add(targetNode.father);
                        targetNode = targetNode.father;
                    }

                    path.Reverse();

                    return path;
                }
            }
        }

        private int SortOpenList(AStarNode a, AStarNode b)
        {
            if (a.f >= b.f) return 1;
            return -1;
        }

        private void FindNearlyNodeToOpenList(int x, int y, float g, AStarNode father, AStarNode target)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            AStarNode origin = nodes[x, y];
            if (origin == null || origin.type == ANodeType.Stop || closeList.Contains(origin) ||
                openList.Contains(origin)) return;
            origin.father = father;
            origin.g = father.g + g;
            origin.h = Mathf.Abs(target.x - origin.x) + Mathf.Abs(target.y - origin.y);
            origin.f = origin.g + origin.h;
            openList.Add(origin);
        }
    }
}