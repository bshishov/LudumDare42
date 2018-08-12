using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "furniture", menuName = "Furniture")]
    public class Furniture : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public GameObject Prefab;
    }
}
