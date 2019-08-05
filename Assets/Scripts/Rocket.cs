using System.Collections;
using PB.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PB.Control
{
    struct Sounds
    {
        public AudioClip MainEngine;
        public AudioClip LevelComplete;
        public AudioClip Death;
        public AudioClip BonusItem;

        public void AssignSounds(AudioClip[] sounds)
        {
            MainEngine = sounds[0];
            LevelComplete = sounds[1];
            Death = sounds[2];
            BonusItem = sounds[3];
        }
    }

    struct Particles
    {
        public ParticleSystem Thrust;
        public ParticleSystem LevelComplete;
        public ParticleSystem Explosion;

        public void AssignParticles(ParticleSystem[] item)
        {
            Thrust = item[0];
            LevelComplete = item[1];
            Explosion = item[2];
        }
    }

    public class Rocket : MonoBehaviour
    {
        [SerializeField] float mainEngineThrust = 3;
        [SerializeField] float rcsThrust = 250f;
        
        // [0] = Main Engine, [1] = Level Complete, [2] = Death, [3] = Bonus Item
        [SerializeField] AudioClip[] soundClips;

        // [0] = Thrust, [1] = Level Complete, [2] = Explosion
        [SerializeField] ParticleSystem[] particleSystems;

        static bool collisionsDontKill = false;
        const float earthGrav = 9.812f;

        State state;
        Rigidbody rocketRB;
        Level level;
        AudioSource audioSource;
        Sounds sounds;
        Particles particles;

        enum State {Alive, Dieing, Trancending};

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            sounds.AssignSounds(soundClips);
            particles.AssignParticles(particleSystems);
            rocketRB = GetComponent<Rigidbody>();
            level = GameObject.FindWithTag("Level").GetComponent<Level>();
            state = State.Alive;
        }

        void Update()
        {
            if  (!level.GetHasStarted() && Input.GetKeyDown(KeyCode.Space))
            {
                level.ToggleHasStarted();
            }

            GetInput(); 
        }

        void GetInput()
        {
            if(state == State.Alive)
            {
                if      (Input.GetKey("space")) { ActivateMainEngine(); }
                else                            { StopEngineSound(); }

                if      (Input.GetKey("left"))  { RotateClockwise(); }
                else if (Input.GetKey("right")) { RotateCounterClockwise(); }
            }

            if      (Debug.isDebugBuild)    { CheckDebugKeys(); }
        }

        void ActivateMainEngine()
        {
            float thrust = mainEngineThrust * earthGrav;
            rocketRB.AddRelativeForce(new Vector3(0f, thrust, 0f) * Time.deltaTime);
            Debug.Log("Thrusting");
            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(sounds.MainEngine);
            }
        }

        private void StopEngineSound()
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.volume = 1f;
        }

        void RotateClockwise()
        {
            Debug.Log("Rotate Right");
            rocketRB.angularVelocity = rocketRB.angularVelocity * 0f; 
            transform.Rotate(new Vector3(0f, 0f, rcsThrust) * Time.deltaTime);
        }

        void RotateCounterClockwise()
        {
            Debug.Log("Rotate Left");
            rocketRB.angularVelocity = rocketRB.angularVelocity * 0f;
            transform.Rotate(new Vector3(0f, 0f, -rcsThrust) * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other) {
            if(state == State.Alive)
            {
                switch(other.gameObject.tag)
                {
                    case "Finish":
                        state = State.Trancending;
                        PlayFinishSound();
                        level.ToggleLevelComplete();
                        break;
                    case "Friendly":
                        break;
                    default:
                        state = State.Dieing;
                        print("You have Crashed and Killed everyone Aboard");
                        break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (state == State.Alive)
            {
                switch (other.gameObject.tag)
                {
                    case "BonusItem":
                        PlayBonusSound();
                        BonusItem bonus = other.gameObject.GetComponent<BonusItem>();
                        level.AddToCounters(bonus.GetBonus().x, bonus.GetBonus().y, bonus.GetBonus().z);
                        print(other.gameObject.name + " collected.");
                        break;
                    default:
                        Physics.gravity = new Vector3(0f, 0f, 50f);
                        rocketRB.AddRelativeTorque(0f, 0f, 500f);
                        state = State.Dieing;
                        Invoke("LoseRoutine", 10f);
                        break;
                }
            }
        }

        private void PlayBonusSound()
        {
            StopEngineSound();
            audioSource.PlayOneShot(sounds.BonusItem);
        }

        private void PlayFinishSound()
        {
            StopEngineSound();
            audioSource.PlayOneShot(sounds.LevelComplete);
        }

        void LoadNextLevel()
        {
            //Scene currentScene = Scene.buildIndex;
            //SceneManager.LoadScene(currentScene + 1);
        }

        void LoseRoutine()
        {
            audioSource.Stop();
            SceneManager.LoadScene(0);
        }

        void CheckDebugKeys()
        {
            if  (Input.GetKeyDown(KeyCode.F1))  { LoadNextLevel(); }
            if  (Input.GetKeyDown(KeyCode.F2))  { collisionsDontKill = !collisionsDontKill; }
        }
    }

}
