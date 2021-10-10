/**
 *  Source code from Youtube channel: TheScreamingFedora. 
 *  
 *  Extra features and bug fixes by: Erick Luis de Souza.
 *  Code by : Erick Luis de Souza.
 *  
 *  email me at: erickluiss@gmail.com 
 *  for aditional information.
 * 
 */


using UnityEngine;

namespace SelectionSystem
{
    /// <summary>
    /// The <see cref="SelectionHighlight"/> is a Component to handle the visual part of the Selection System. <br/> 
    /// It requires a <seealso cref="ParticleSystem"/> Component. 
    /// <para> This class cannot be inherited. </para>
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class SelectionHighlight : MonoBehaviour, ISelectionHighlight
    {
        public const float minSize = 2f;
        public const float maxSize = 20f;

        public static readonly Color defaultColor = Color.white;
        public static readonly Color ownerColor = Color.green;
        public static readonly Color neutralColor = Color.yellow;
        public static readonly Color hostileColor = Color.red;

        [SerializeField]
        [Range(minSize, maxSize)]
        private float highlightSize = 4f;

        private ParticleSystem _particles;

        /// <inheritdoc/>
        public Color color
        {
            get
            {
                return _particles.main.startColor.color;
            }
        }
        /// <inheritdoc/>
        public float size
        {
            get
            {
                return _particles.main.startSize.constant;
            }
        }

        private void OnValidate()
        {
            if (!_particles)
            {
                if (TryGetComponent<ParticleSystem>(out var particles))
                    _particles = particles;
            }

            ValidateParticleSize();
        }

        private void ValidateParticleSize()
        {
            var particleMain = _particles.main;

            particleMain.startSize = highlightSize;

            if (particleMain.startSize.constant < minSize)
                particleMain.startSize = minSize;
            else if (particleMain.startSize.constant > maxSize)
                particleMain.startSize = maxSize;
        }

        /// <inheritdoc/>
        public void Activate()
        {
            gameObject.SetActive(true);
            _particles.Play();
        }

        /// <inheritdoc/>
        public void Deactivate()
        {
            gameObject.SetActive(false);
            _particles.Stop();
        }

        /// <inheritdoc/>
        public void SetHighlightColor(Color color)
        {
            var main = _particles.main;
            main.startColor = color;
        }
    }
}