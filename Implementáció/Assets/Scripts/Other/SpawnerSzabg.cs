using System.Text.RegularExpressions;
using UnityEngine;

namespace Other
{
    public class SpawnerSzabg : MonoBehaviour
    {
        public TextAsset text;
        public GameObject[] objects;
        public float scale;

        private void Start()
        {
            string data = text.text;
            string[] lines = Regex.Split(data, "\n|\r|\r\n");
            foreach (var line in lines)
            {
                string[] parts = line.Split();
                foreach (var part in parts)
                {
                    var tmp = part.Split(",");
                    Vector3 pos = new(float.Parse(tmp[0]) * scale, 0, float.Parse(tmp[1]) * scale);
                    var obj = Instantiate(objects[int.Parse(tmp[2])], pos, Quaternion.identity);
                    obj.transform.Rotate(Vector3.up, Random.Range(0, 360));
                }
            }
        }
    }
}
