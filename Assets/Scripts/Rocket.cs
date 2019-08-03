using PB.Core;
using UnityEngine;

namespace PB.Control
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] float mainEngineThrust = 1000f;
        [SerializeField] float rcsThrust = 250f;
        [SerializeField] AudioClip[] soundClips;
        
        static bool collisionsDontKill = false;
        
        State state;
        Rigidbody rocketRB;
        Level level;
        AudioSource audioSource;
        Sounds sounds;

        enum State {Alive, Dieing, Trancending};
        struct Sounds
        {
            public AudioClip MainEngine;
            public AudioClip LevelComplete;
            public AudioClip Death;
            public AudioClip BonusItem;

            public void AssignSounds(AudioClip[] sounds)
            {
                MainEngine=sounds[0];
                LevelComplete= sounds[1];
                Death= sounds[2];
                BonusItem= sounds[3];
            }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            sounds.AssignSounds(soundClips);
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
            if      (Input.GetKey("space")) { ActivateMainEngine(); }
            else                            { audioSource.Stop(); }

            if      (Input.GetKey("left"))  { RotateClockwise(); }
            else if (Input.GetKey("right")) { RotateCounterClockwise(); }

            if      (Debug.isDebugBuild)    { CheckDebugKeys(); }
        }

        void ActivateMainEngine()
        {
            rocketRB.AddRelativeForce(new Vector3(0f,mainEngineThrust,0f) * Time.deltaTime);
            Debug.Log("Thrusting");
            if (!audioSource.isPlaying)
            {
                audioSource.clip = sounds.MainEngine;
                audioSource.Play();
            }
        }

        void RotateClockwise()
        {
            Debug.Log("Rotate Right");
            transform.Rotate(new Vector3(0f, 0f, -rcsThrust) * Time.deltaTime);
        }

        void RotateCounterClockwise()
        {
            Debug.Log("Rotate Left");
            transform.Rotate(new Vector3(0f, 0f, rcsThrust) * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other) {
            if(state == State.Alive)
            {
                switch(other.gameObject.tag)
                {
                    case "Finish":
                        audioSource.Stop();
                        audioSource.PlayOneShot(sounds.LevelComplete);
                        level.ToggleLevelComplete();
                        break;
                    case "Bonus Item":
                        BonusItem bonus = other.gameObject.GetComponent<BonusItem>();
                        audioSource.PlayOneShot(sounds.BonusItem);
                        level.AddToCounters(bonus.GetBonus().x, bonus.GetBonus().y, bonus.GetBonus().z);
                        break;
                    default:
                        print("Unknown Case");
                        break;
                }
            }
        }

        void LoadNextLevel()
        {
            //Scene currentScene = Scene.buildIndex;
            //SceneManager.LoadScene(currentScene + 1);
        }

        void CheckDebugKeys()
        {
            if  (Input.GetKeyDown(KeyCode.F1))  { LoadNextLevel(); }
            if  (Input.GetKeyDown(KeyCode.F2))  { collisionsDontKill = !collisionsDontKill; }
        }
    }

}
