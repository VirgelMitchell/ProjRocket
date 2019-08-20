using PB.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PB.Control
{
    [System.Serializable]
    public struct Sounds
    {
        public AudioClip Thrust;
        public AudioClip LevelComplete;
        public AudioClip Explosion;
        public AudioClip BonusItem;
    }

    [System.Serializable]
    public struct Particles
    {
        public ParticleSystem Thrust;
        public ParticleSystem LevelComplete;
        public ParticleSystem Explosion;
    }

    public class Rocket : MonoBehaviour
    {
        [SerializeField] float mainEngineThrust = 3;
        [SerializeField] float rcsThrust = 250f;
        [SerializeField] float loadWaitTime = 0.75f;
        
        [SerializeField] Sounds sounds;
        [SerializeField] Particles particles;

        static bool collisionsDontKill = false;
        const float earthGrav = 9.812f;

        State state;
        Rigidbody rocketRB;
        Level level;
        AudioSource audioSource;

        enum State {Alive, Dieing, Trancending};

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            rocketRB = GetComponent<Rigidbody>();
            level = GameObject.FindWithTag("Level").GetComponent<Level>();
            state = State.Alive;
        }

        void Update()
        {
            if  (!level.GetHasStarted() && Input.GetKeyDown(KeyCode.Space))
            {
                level.ToggleHasStarted();  // TODO track bug
            }

            GetInput(); 
        }

        void GetInput()
        {
            if(state == State.Alive)
            {
                if      (Input.GetKey("space")) { ActivateMainEngine(); }
                else                            { StopMainEngines(); }

                if      (Input.GetKey("left"))  { RotateClockwise(); }
                else if (Input.GetKey("right")) { RotateCounterClockwise(); }
            }

            if      (Debug.isDebugBuild)    { CheckDebugKeys(); }
        }

        void ActivateMainEngine()
        {
            float thrust = mainEngineThrust * earthGrav;
            
            rocketRB.AddRelativeForce(new Vector3(0f, thrust, 0f) * Time.deltaTime);
            if (!audioSource.isPlaying) { PlayEngineSound(); }
            particles.Thrust.Play();
        }

        void RotateClockwise()
        {
            rocketRB.angularVelocity = rocketRB.angularVelocity * 0f; 
            transform.Rotate(new Vector3(0f, 0f, rcsThrust) * Time.deltaTime);
        }

        void RotateCounterClockwise()
        {
            rocketRB.angularVelocity = rocketRB.angularVelocity * 0f;
            transform.Rotate(new Vector3(0f, 0f, -rcsThrust) * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (state != State.Alive) { return; }
            switch(other.gameObject.tag)
            {
                case "Finish":
                    FinishRoutine();
                    break;
                case "Friendly":
                    break;
                default:
                    if (collisionsDontKill) { return; }
                    state = State.Dieing;
                    LoseRoutine();
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            float boundryWaitTime = 9f;
            
            if (state != State.Alive) { return; }
            switch (other.gameObject.tag)
            {
                case "BonusItem":
                    PlayBonusSound();   // TODO: Does not Play Consistantly
                    BonusItem bonus = other.gameObject.GetComponent<BonusItem>();
                    level.AddToCounters(bonus.GetBonus().x, bonus.GetBonus().y, bonus.GetBonus().z);
                    Destroy(other.gameObject);
                    break;
                default:
                    AtBoundryChangePhysics();
                    state = State.Dieing;
                    Invoke("LoseRoutine", boundryWaitTime);
                    print("Out of Bounds!");
                    break;
            }
        }

        void LoadNextLevel()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            if ((currentScene+1) >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(currentScene + 1);
            }
        }

        void LoadStart() { SceneManager.LoadScene(0); }

        private void FinishRoutine()
        {
            state = State.Trancending;
            PlayFinishSound();
            particles.LevelComplete.Play();
            level.ToggleLevelComplete();  // Track possible bug
            Invoke("LoadNextLevel", loadWaitTime);
        }

        private void AtBoundryChangePhysics()
        {
            rocketRB.constraints = RigidbodyConstraints.None;
            Physics.gravity = new Vector3(0f, 0f, -5f);
            rocketRB.AddRelativeTorque(-75f, 100f, 1000f);
        }

        void LoseRoutine()
        {
            audioSource.Stop();
            PlayExplosion();
            DestroyShip();
            Invoke("LoadStart", loadWaitTime);
        }

        private void PlayEngineSound()
        {
            audioSource.volume = 0.3f;
            audioSource.PlayOneShot(sounds.Thrust);
        }

        private void StopMainEngines()
        {
            if(audioSource.clip == sounds.Thrust)
            {
                audioSource.Stop();
                audioSource.volume = 1f;
                particles.Thrust.Stop();
            }
        }

        private void PlayBonusSound()   // TODO: Does not Play Consistantly
        {
            audioSource.PlayOneShot(sounds.BonusItem);
        }

        private void PlayFinishSound()
        {
            StopMainEngines();
            audioSource.PlayOneShot(sounds.LevelComplete);
        }

        void PlayExplosion()
        {
            particles.Explosion.Play();
            audioSource.PlayOneShot(sounds.Explosion);
        }

        void DestroyShip()
        {
            // Destroy the ship without losing the game object
        }

        void CheckDebugKeys()
        {
            if  (Input.GetKeyDown(KeyCode.F1))  { LoadNextLevel(); }
            if  (Input.GetKeyDown(KeyCode.F2))  { collisionsDontKill = !collisionsDontKill; }
        }
    }
}