using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace BlueToolkit
{
    /// <summary>
    /// ����������
    /// </summary>
    public class MeshData      
    {
        private int _vertexOffset = 0;
        private int _normalOffset = 0;
        private int _uvOffset = 0;
        private StringBuilder _data;
        private Dictionary<string, MaterialData> _materialDic;

        public MeshData()
        {
            _data = new StringBuilder();
            _materialDic = new Dictionary<string, MaterialData>();
        }

        public Dictionary<string, MaterialData> GetMaterialDic()
        {
            return _materialDic;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="meshFilter"></param>
        /// <param name="materialDic"></param>
        public void SaveData(MeshFilter meshFilter)
        {
            _data = new StringBuilder();
            SaveMeshData(_data, meshFilter, _materialDic);
        }

        //���������ݱ�����ı���Ϣ
        private void SaveMeshData(StringBuilder data,MeshFilter meshFilter, Dictionary<string, MaterialData> materialDic)
        {
            //������������
            SaveMeshName(meshFilter, data);

            //���涥������
            SaveVertices(meshFilter, data);

            //���淨������
            SaveNormals(meshFilter, data);

            //����uv����
            SaveUVs(meshFilter, data);

            //�����������
            SaveMaterails(meshFilter, data, materialDic);
        }

        //������������
        private void SaveMeshName(MeshFilter meshFilter, StringBuilder data)
        {
            data.Append("g ").Append(meshFilter.name).Append("\n");
        }
        //���涥������
        private void SaveVertices(MeshFilter meshFilter, StringBuilder data)
        {
            foreach (Vector3 ver in meshFilter.sharedMesh.vertices)
            {
                Vector3 worldPos = meshFilter.transform.TransformPoint(ver);

                //��Ϊ����ϵ������x������Ҫ��ת
                data.Append(string.Format("v {0} {1} {2}\n", -worldPos.x, worldPos.y, worldPos.z));
            }
            data.Append("\n");
        }
        //���淨������
        private void SaveNormals(MeshFilter meshFilter, StringBuilder data)
        {
            foreach (Vector3 normal in meshFilter.sharedMesh.normals)
            {
                Vector3 directionWorld = meshFilter.transform.TransformDirection(normal);

                data.Append(string.Format("vn {0} {1} {2}\n", -directionWorld.x, directionWorld.y, directionWorld.z));
            }
            data.Append("\n");
        }

        //����uv����
        private void SaveUVs(MeshFilter meshFilter, StringBuilder data)
        {
            foreach (Vector3 uv in meshFilter.sharedMesh.uv)
            {
                data.Append(string.Format("vt {0} {1}\n", uv.x, uv.y));
            }
        }

        //�����������
        private void SaveMaterails(MeshFilter meshFilter, StringBuilder data, Dictionary<string, MaterialData> materialDic)
        {
            Mesh mesh = meshFilter.sharedMesh;
            Material[] materialArray = meshFilter.GetComponent<Renderer>().sharedMaterials;
            string materialName = "";

            for (int materialIndex = 0; materialIndex < mesh.subMeshCount; materialIndex++)
            {
                materialName = materialArray[materialIndex].name;
                data.Append("\n");

                data.Append("usemtl ")
                    .Append(materialName)
                    .Append("\n");

                data.Append("usemap ")
                    .Append(materialName)
                    .Append("\n");

                //ɸѡͬ�����ʣ����ظ����
                if (!materialDic.ContainsKey(materialName))
                {
                    MaterialData materialData = new MaterialData();

                    materialData.Name = materialName;

                    materialData.TextureName =
                        materialArray[materialIndex].mainTexture ?
                        AssetDatabase.GetAssetPath(materialArray[materialIndex].mainTexture) : null;

                    materialDic[materialData.Name] = materialData;
                }

                //��������������
                int[] triangles = mesh.GetTriangles(materialIndex);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    data.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[i] + 1 + _vertexOffset, triangles[i + 1] + 1 + _normalOffset,
                        triangles[i + 2] + 1 + _uvOffset));
                }
            }

            _vertexOffset += mesh.vertices.Length;
            _normalOffset += mesh.normals.Length;
            _uvOffset += mesh.uv.Length;
        }

        public override string ToString()
        {
            return _data.ToString();
        }
    }

    public struct MaterialData
    {
        public string Name;
        public string TextureName;
    }
}
