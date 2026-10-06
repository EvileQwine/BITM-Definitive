using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class MMRScript : MonoBehaviour
{
    Dictionary<string, float> _damages;
    Dictionary<string, Vector3> _knockback;
    void Awake()
    {
        _damages = new() {
            { "Match", 1 },
            { "GasCan", 3 },
            { "GasExplosion", 10 },
            { "AxeNeutralGrounded", 5 },
            { "AxeNeutralAir", 5 },
            { "AxePush", 7 },
            { "AxePullGround", 7 },
            { "AxePullAir", 7 },
        };
    }
    public float DamageReturn(string id)
    {
        return _damages[id];
    }
    public Vector3 KnockbackReturn(string id)
    {
        _knockback = new()
        {
            { "Match", (transform.up * 20) },
            { "GasCan", Vector3.zero },
            { "GasExplosion", transform.up * 30},
            { "AxeNeutralGrounded",  transform.up * 20  },
            { "AxeNeutralAir",  (transform.up * 5) + (transform.forward * -5)},
            { "AxePush",  (transform.up * 3) + (transform.forward * -15)},
            { "AxePullGround",  (transform.up * 3) + (transform.forward * 15)},
            { "AxePullAir",  (transform.up * -1) + (transform.forward * 5)},
        };
        return _knockback[id];
    }
}
