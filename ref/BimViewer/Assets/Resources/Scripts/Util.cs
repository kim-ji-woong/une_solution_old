using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net;
using System.Collections;
using System.IO;

namespace Assets.Resources.Scripts
{
    public class Util
    {
        public static IEnumerator loadShapeObjects(string fileName, GameObject originalObject,bool useTransform = true)
        {
            WWW www = new WWW(fileName);

            yield return www;

            createModelByLevel(originalObject, www.text, useTransform );

            yield return null;
        }

        public static List<GameObject> createModelByLevel(GameObject originalObject, string data, bool useTransform = true)
        {
            string[] levelBlockSeperator = new string[] { "[LEVEL]\n" };
            string[] lineSeperator = new string[] { "\n" };

            string[] levelBlocks = data.Split(levelBlockSeperator, StringSplitOptions.RemoveEmptyEntries);

            List<GameObject> createdModelList = new List<GameObject>();

            foreach (string levelBlock in levelBlocks)
            {
                string[] lines = levelBlock.Split(lineSeperator, StringSplitOptions.RemoveEmptyEntries);
                string levelId = lines[0];

                //GameObject levelObject = GameObject.Instantiate(parentObject);

                //levelObject.name = levelId;

                //levelObject.transform.parent = parentObject.transform;

                string newLevelBlock = levelBlock.Replace(levelId + "\n", "");

                createdModelList.AddRange(createMeshObjects(originalObject, newLevelBlock, levelId, useTransform));
            }

            return createdModelList;
        }

        private static Mesh createSingleMesh(string rawData,ref string id)
        {
            Mesh mesh = new Mesh();

            string[] lines = rawData.Split('\n');

            //string[] meshInfo = lines[0].Split(',');

            //id = meshInfo[0];

            //if (2 != meshInfo.Length)
            //{
            //    Debug.Log(lines[0]);
            //}

            //int vertexNumber = (int.Parse(meshInfo[1])-1)*3;            

            int vertexNumber = (lines.Length - 1) / 4 * 3;
            int faceNumber = vertexNumber / 3;

            Vector3[] vertices = new Vector3[vertexNumber];
            Vector3[] normals = new Vector3[vertexNumber];

            int[] triangles = new int[faceNumber * 3];
            int triIndexMax = faceNumber * 3 - 1;

            int triIndex = 0;
            int normalCounter = 0;
            int vertexCounter = 0;

            for (int i = 0; i < lines.Length; i++) 
            {
                string[] points = lines[i].Split(' ');
                if (points.Length == 3)
                {
                    //string sx = points[0];
                    float x = float.Parse(points[0]);
                    float z = float.Parse(points[1]);
                    float y = float.Parse(points[2]);

                    //노말일때 처리.
                    if (i % 4 == 3)
                    {
                        normals[normalCounter] = new Vector3(x, y, z); //점3개 동일 normal.
                        normalCounter++;
                        normals[normalCounter] = new Vector3(x, y, z);
                        normalCounter++;
                        normals[normalCounter] = new Vector3(x, y, z);
                        normalCounter++;
                    }
                    else
                    {
                        vertices[vertexCounter] = new Vector3(x, y, z);
                        vertexCounter++;
                        triangles[triIndex] = triIndexMax - triIndex;
                        triIndex++;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private static Mesh createInstanceMesh(string rawData, ref string id, Dictionary<string, Mesh> meshMap, 
            ref Vector3 origin, ref Vector3 forward, ref Vector3 up, ref Vector3 right,ref float scale)
        {
            Mesh mesh = new Mesh();

            string[] lines = rawData.Split('\n');

            string[] meshInfo = lines[0].Split(',');

            id = meshInfo[0];

            if (3 != meshInfo.Length)
            {
                Debug.Log(lines[0]);
            }

            string typeName = meshInfo[1];

            string lineCount = meshInfo[2];

            //transform 가져오기
            string[] origins = lines[1].Split(' ');

            origin = new Vector3(float.Parse(origins[0]), float.Parse(origins[2]), float.Parse(origins[1]));

            scale = float.Parse(lines[2]);

            string[] vectorElem = lines[3].Split(' ');
            Vector3 biasX = new Vector3(float.Parse(vectorElem[0]), float.Parse(vectorElem[2]), float.Parse(vectorElem[1]));

            vectorElem = lines[4].Split(' ');
            Vector3 biasY = new Vector3(float.Parse(vectorElem[0]), float.Parse(vectorElem[2]), float.Parse(vectorElem[1]));

            vectorElem = lines[5].Split(' ');
            Vector3 biasZ = new Vector3(float.Parse(vectorElem[0]), float.Parse(vectorElem[2]), float.Parse(vectorElem[1]));

            right = biasX;
            forward = biasY;
            up = biasZ;

            if (meshMap.ContainsKey(typeName))
            {
                mesh = meshMap[typeName];

                return mesh;
            }
            else
            {
                meshMap.Add(typeName, mesh);
            }

            //int vertexNumber = (int.Parse(lineCount) - 6) * 3;
            int vertexNumber = (lines.Length - 1) / 4 * 3;
            int faceNumber = vertexNumber / 3;

            Vector3[] vertices = new Vector3[vertexNumber];
            Vector3[] normals = new Vector3[vertexNumber];

            int[] triangles = new int[faceNumber * 3];
            int triIndexMax = faceNumber * 3 - 1;

            int triIndex = 0;
            int normalCounter = 0;
            int vertexCounter = 0;

            for (int i = 6; i < lines.Length; i++) //0~5 라인은 건너뛴다. (id와 총 라인 갯수,트랜스폼 정보)
            {
                string[] points = lines[i].Split(' ');
                if (points.Length == 3)
                {
                    //string sx = points[0];
                    float x = float.Parse(points[0]);
                    float z = float.Parse(points[1]);
                    float y = float.Parse(points[2]);

                    //노말일때 처리.
                    if (i % 4 == 1)
                    {
                        normals[normalCounter] = new Vector3(x, y, z); //점3개 동일 normal.
                        normalCounter++;
                        normals[normalCounter] = new Vector3(x, y, z);
                        normalCounter++;
                        normals[normalCounter] = new Vector3(x, y, z);
                        normalCounter++;
                    }
                    else
                    {
                        vertices[vertexCounter] = new Vector3(x, y, z);
                        vertexCounter++;
                        triangles[triIndex] = triIndexMax - triIndex;
                        triIndex++;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        public static IEnumerator loadShape(string fileName, GameObject targetObject)
        {
            //GameObject floor = GameObject.Find("Floor");
            //floor.AddComponent(MeshFilter);

            //	floor.AddComponent(MeshRenderer);	

            Mesh mesh = new Mesh();

            //floorMesh.name = "floorMesh";



            WWW www = new WWW(fileName);

            yield return www;

            Util.createSingleMeshObject(targetObject, www.text);

            yield return null;
        }

        public static string GetIp()
        {
            string hostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostName);

            foreach (var ip in ipEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        //public static void loadLocalShapeObjects(string fileName, GameObject parentObject)
        //{
        //    string rawData = File.ReadAllText(fileName);
        //    createMeshObjects(parentObject, rawData);
        //}

        public static List<GameObject> createMeshObjects(GameObject originalObject, string rawData,string levelId,bool useTransform = true)
        {
            string[] stringSeparators = new string[] { "[HEADER]\n" };

            string[] rawDataList = rawData.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, Mesh> meshMap = new Dictionary<string, Mesh>();

            GameObject levelObject = GameObject.Find(levelId);

            if (null == levelObject)
            {
                levelObject = new GameObject(levelId);

                levelObject.tag = "Level";
            }

            List<GameObject> createdModelObjectList = new List<GameObject>();
                

            foreach (string shapeData in rawDataList)
            {
                string id = "";

                GameObject gameObj = new GameObject();

                MeshFilter meshFilter = gameObj.AddComponent<MeshFilter>();
                MeshCollider meshCollider = gameObj.AddComponent<MeshCollider>();
                MeshRenderer meshRenderer = gameObj.AddComponent<MeshRenderer>();

                meshRenderer.material = originalObject.GetComponent<MeshRenderer>().material;

                gameObj.transform.parent = levelObject.transform;                

                if (useTransform)
                {
                    Vector3 origin = Vector3.zero;
                    Vector3 forward = Vector3.zero;
                    Vector3 up = Vector3.zero;
                    Vector3 right = Vector3.zero;

                    float scale = 1.0f;

                    Mesh mesh = createInstanceMesh(shapeData, ref id, meshMap, ref origin, ref forward, ref up, ref right,ref scale);

                    meshFilter.sharedMesh = mesh;

                    //try
                    //{
                    //    meshCollider.sharedMesh = mesh;
                    //}
                    //catch
                    //{
                    //    meshCollider.sharedMesh.Clear();
                    //    meshCollider.sharedMesh = mesh;
                    //}
                                

                    gameObj.transform.localPosition = origin + forward;

                    Vector3 worldForwardPos = gameObj.transform.position;

                    gameObj.transform.localPosition = origin;

                    gameObj.transform.LookAt(worldForwardPos, up);
                    //gameObj.transform.forward = forward;
                    //gameObj.transform.right = right;
                    //gameObj.transform.up = up;
                    

                    gameObj.transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                {
                    Mesh mesh = createSingleMesh(shapeData, ref id);

                    meshFilter.sharedMesh = mesh;
                    meshCollider.sharedMesh = mesh;

                    
                    gameObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                if (id.Length == 0)
                {
                    gameObj.name = originalObject.name + levelObject.transform.childCount;
                }                    
                else
                    gameObj.name = id;

                createdModelObjectList.Add(gameObj);
            }

            return createdModelObjectList;
        }

        public static void loadLocalShape(string fileName, GameObject targetObject)
        {
            string rawData = File.ReadAllText(fileName);
            createSingleMeshObject(targetObject, rawData);
        }

        public static void createSingleMeshObject(GameObject targetObject, string rawData)
        {
            Mesh mesh = new Mesh();

            string[] lines = rawData.Split('\n');

            int vertexNumber = lines.Length / 4 * 3;
            int faceNumber = vertexNumber / 3;

            Vector3[] vertices = new Vector3[vertexNumber];
            Vector3[] normals = new Vector3[vertexNumber];
            //Vector2 [] uvs = new Vector2[vertexNumber];

            int[] triangles = new int[faceNumber * 3];
            int triIndexMax = faceNumber * 3 - 1;

            int triIndex = 0;
            int normalCounter = 0;
            int vertexCounter = 0;
            //int uvCounter = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string[] points = lines[i].Split(' ');
                if (points.Length == 3)
                {
                    //string sx = points[0];
                    float x = float.Parse(points[0]);
                    float z = float.Parse(points[1]);
                    float y = float.Parse(points[2]);

                    //노말일때 처리.
                    if (i % 4 == 3)
                    {
                        normals[normalCounter] = new Vector3(x, y, z); //점3개 동일 normal.
                        normalCounter++;
                        normals[normalCounter] = new Vector3(x, y, z);
                        normalCounter++;
                        normals[normalCounter] = new Vector3(x, y, z);
                        normalCounter++;
                    }
                    else
                    {
                        vertices[vertexCounter] = new Vector3(x, y, z);
                        vertexCounter++;
                        triangles[triIndex] = triIndexMax - triIndex;
                        triIndex++;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            targetObject.GetComponent<MeshFilter>().mesh = mesh;
            targetObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }


    }
}
