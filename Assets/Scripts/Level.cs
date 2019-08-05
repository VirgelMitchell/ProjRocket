using UnityEngine;
using PB.Control;

namespace PB.Core
{
    public class Level : MonoBehaviour
    {
        [SerializeField] float timeForLevel = 60f;
        [SerializeField] float fuelForLevel = 60f;
        [SerializeField] float lifeSupportForLevel = 60f;
        [SerializeField][Range(0f,4f)] float levelGravity = 1;

        bool hasStarted;
        bool levelComplete;

        Rocket player;
        Counters counters;

        struct Counters
        {
            float TimeRemaining;
            float Fuel;
            float LifeSupport;

            public void UpdateCounters()
            {
                TimeRemaining -= Time.deltaTime;
                Fuel -= Time.deltaTime;
                LifeSupport -= Time.deltaTime;
            }

            public bool CountersExpired()
            {
                if (
                    TimeRemaining <= Mathf.Epsilon || Fuel <= Mathf.Epsilon || LifeSupport <= Mathf.Epsilon
                    )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public void SetCounters(float time, float fuel, float lifeSupport)
            {
                TimeRemaining = time;
                Fuel = fuel;
                LifeSupport = lifeSupport;
            }

            public void AddToCounters(float time, float fuel, float lifeSupport)
            {
                TimeRemaining += time;
                Fuel += fuel;
                LifeSupport += lifeSupport;
            }
        }

        private void Start() {
            counters = new Counters();
            Physics.gravity = levelGravity * new Vector3(0f, -9.812f, 0f);
            Rocket player = GameObject.FindWithTag("Player").GetComponent<Rocket>();
            hasStarted = false;
            counters.SetCounters(timeForLevel, fuelForLevel, lifeSupportForLevel);
        }

        private void Update()
        {
            // if(hasStarted) { counters.UpdateCounters(); }
            // if(counters.CountersExpired()) { print("Game Over"); }
            // if(levelComplete) { print("Congratulations!!\nLevel Complete"); }
        }

        public bool GetHasStarted()         { return hasStarted; }
        public bool GetLevelComplete()      { return levelComplete; }
        public void ToggleHasStarted()      { hasStarted = !hasStarted; }
        public void ToggleLevelComplete()   { levelComplete = !levelComplete; }

        public void AddToCounters(float time, float fuel, float lifesupport)
        {
            counters.AddToCounters(time, fuel, lifesupport);
        }
    }
}