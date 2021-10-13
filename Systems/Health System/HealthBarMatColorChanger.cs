
using UnityEngine;

namespace SLE
{
    public class HealthBarMatColorChanger : MonoBehaviour
    {
#if UNDER_DEVELOPMENT
        [SerializeField]
#endif
        private Color color;

        MeshRenderer meshRenderer;
        MaterialPropertyBlock propertyBlock;

        private int _BaseColorID = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(_BaseColorID, color);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
