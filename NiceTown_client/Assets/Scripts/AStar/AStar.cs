using System.Collections;
using System.Collections.Generic;
using MFarm.Map;
using UnityEngine;
//using UnityEngine.Rendering;

namespace MFarm.AStar
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList;    //当前选中Node周围的8个点
        private HashSet<Node> closedNodeList;   //所有被选中的点

        private bool pathFound;


        /// <summary>
        /// 构建路径更新Stack的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStack"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStack)
        {
            Debug.Log("building paths");
            pathFound = false;

            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                //Debug.Log("finding shortest path");
                //查找最短路径
                if (FindShortestPath())
                {
                    //构建NPC移动路径
                    //Debug.Log("shortest path found");
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                    //Debug.Log("update shortest path complete");
                }
            }
        }



        /// <summary>
        /// 构建网格节点信息，初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            //Debug.Log("generate grid node");
            if (GridMapManager.Instance == null)
            {
                Debug.LogError("GridMapManager.Instance is null");
            }

            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数组
                Debug.Log(sceneName);
                Debug.Log("building grid dimension");
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();

                closedNodeList = new HashSet<Node>();
            }
            else
            {
                Debug.LogError("GetGridDimensionFail");
                //Debug.Log(sceneName);
                return false;
            }

            //gridNodes的范围是从0,0开始所以需要减去原点坐标得到实际位置
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;

                    TileDetails tile = GridMapManager.Instance.GetTileDetails(key);
                    //Debug.Log("position at tile");
                    //Debug.Log(key.ToString());

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                    else
                    {
                        Debug.Log("tile is null");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 找到最短路径所有node添加到 closedNodeList
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //添加起点
            openNodeList.Add(startNode);
            //Debug.Log("FindShortestPath activated");
            while (openNodeList.Count > 0)
            {//节点排序，Node内涵比较函数
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                //计算周围8个Node补充到OpenList
                EvaluateNeighbourNodes(closeNode);
            }
            //Debug.Log("found shortest path");
            return pathFound;
        }


        /// <summary>
        /// 评估周围8个点，并生成对应消耗值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;
            //Debug.Log("evaluating positions");
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);
                    //Debug.Log("looking for neighbors");
                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //链接父节点
                            validNeighbourNode.parentNode = currentNode;
                            //Debug.Log("add one position into nodelist");
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 找到有效的Node,非障碍，非已选择
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode ==null)
            {
                Debug.LogError("neighbor node is null");
            }

            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
            {
                return null;
            }
            else
                return neighbourNode;
        }


        /// <summary>
        /// 返回两点距离值
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14的倍数+10的倍数</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            //Debug.Log("calculating distance");
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            //Debug.Log("distance calculated");
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }



        /// <summary>
        /// 更新路径每一步的坐标和场景名字
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStep"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;
            //Debug.Log("updating path");
            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                Debug.Log(newStep.gridCoordinate.ToString());
                //压入堆栈
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
                //Debug.Log("update complete");
                //Debug.Log(newStep.gridCoordinate.ToString()+"pushed into stack");
            }
        }
    }
}
