using UnityEngine;
namespace PB.Core
{
    public class GameTracker : MonoBehaviour
    {
        float fuelRemaining;
        float timeRemaining;
        float airRemaining;

        public void StartLevel(float levelTime)
        {
            fuelRemaining += levelTime;
            timeRemaining += levelTime;
            airRemaining += levelTime;
        }

        public void AddTime(float bonus)   { timeRemaining += bonus; }
        public void AddAir(float bonus)    { airRemaining += bonus ;}
        public void AddFuel(float bonus)   { fuelRemaining += bonus; }
    }
}
