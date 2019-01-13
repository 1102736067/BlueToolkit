using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlueToolkit
{
    public class ExportFile     
    {
        /// <summary>
        /// �����ļ������� ·��Ϊ���̸�Ŀ¼
        /// </summary>
        public const string EXPORT_FOLDER = "ExportedObj";

        //�������
        private static void MaterialsToFile(Dictionary<string, MaterialData> materialList, string folder, string filename)
        {
            using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".mtl"))
            {
                foreach (KeyValuePair<string, MaterialData> kvp in materialList)
                {
                    sw.Write("\n");
                    sw.Write("newmtl {0}\n", kvp.Key);
                    sw.Write("Ka  0.6 0.6 0.6\n");
                    sw.Write("Kd  0.6 0.6 0.6\n");
                    sw.Write("Ks  0.9 0.9 0.9\n");
                    sw.Write("d  1.0\n");
                    sw.Write("Ns  0.0\n");
                    sw.Write("illum 2\n");

                    if (kvp.Value.TextureName != null)
                    {
                        string destinationFile = kvp.Value.TextureName;


                        int stripIndex = destinationFile.LastIndexOf('/');

                        if (stripIndex >= 0)
                            destinationFile = destinationFile.Substring(stripIndex + 1).Trim();


                        string relativeFile = destinationFile;

                        destinationFile = folder + "/" + destinationFile;

                        try
                        {
                            File.Copy(kvp.Value.TextureName, destinationFile);
                        }
                        catch
                        {

                        }


                        sw.Write("map_Kd {0}", relativeFile);
                    }

                    sw.Write("\n\n\n");
                }
            }
        }
        /// <summary>
        /// ��������ģ��
        /// </summary>
        /// <param name="mf"></param>
        /// <param name="folder"></param>
        /// <param name="filename"></param>
        public static void ExportObj(MeshFilter mf, string folder, string filename)
        {
            Dictionary<string, MaterialData> materialList = new Dictionary<string, MaterialData>();
            MeshData data = new MeshData(mf, materialList);

            using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                sw.Write(data.ToString());
            }

            MaterialsToFile(materialList, folder, filename);
        }
        /// <summary>
        /// �������ģ��
        /// </summary>
        /// <param name="mf"></param>
        /// <param name="folder"></param>
        /// <param name="filename"></param>
        public static void ExportObjs(MeshFilter[] mf, string folder, string filename)
        {
            Dictionary<string, MaterialData> materialList = new Dictionary<string, MaterialData>();

            using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                foreach (MeshFilter mesh in mf)
                {
                    MeshData data = new MeshData(mesh, materialList);
                    sw.Write(data.ToString());
                }
            }

            MaterialsToFile(materialList, folder, filename);
        }
        /// <summary>
        /// ��������Ŀ¼
        /// </summary>
        /// <returns></returns>
        public static bool CreateExportFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(EXPORT_FOLDER);
            }
            catch
            {
                EditorUtility.DisplayDialog("����", "���������ļ���ʧ��", "�ر�");
                return false;
            }

            return true;
        }
    }
}
