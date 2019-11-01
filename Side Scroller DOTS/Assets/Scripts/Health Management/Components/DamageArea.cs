using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;


namespace JeffreyDufseth.HealthManagement
{
    public struct DamageArea : IComponentData
    {
        //TODO this should be expanded to have more nuanced functionality
        public float DamagePerFrame;
    }
}