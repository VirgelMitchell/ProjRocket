using UnityEngine;

namespace PB.Core
{
    public class BonusItem : MonoBehaviour
    {
        [SerializeField] float bonusTime;
        [SerializeField] float bonusFuel;
        [SerializeField] float bonusLifeSupport;

        public Vector3 GetBonus()
        {
            Vector3 bonus = new Vector3();
            bonus.x = bonusTime;
            bonus.y = bonusFuel;
            bonus.z = bonusLifeSupport;
            return bonus;
        }
    }
}