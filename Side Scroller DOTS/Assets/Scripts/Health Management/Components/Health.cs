using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.HealthManagement
{
    public struct Health : IComponentData
    {
        public float CurrentHealth;
        public float CurrentDamage;
    }
}