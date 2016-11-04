using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MenuBoard : MonoBehaviour
    {
        private float _rotationSpeed;
        // the audio clip on start up
        public AudioClip startAudioClip;

        // Use this for initialization
        void Start()
        {
            _rotationSpeed = -60;
            PlaySound();
        }

        // Update is called once per frame
        void Update()
        {

            if (_rotationSpeed > -.1 && (Mathf.Abs(transform.eulerAngles.x % 30) - 30) < .05)
            {
                return;
            }

            transform.Rotate(Vector3.down, _rotationSpeed);
            _rotationSpeed = _rotationSpeed * .991f;
        }

        void PlaySound()
        {
            // play the start up sounds
            var audio = GetComponent<AudioSource>();
            audio.clip = startAudioClip;
            audio.PlayDelayed(5);
        }
    }
}
