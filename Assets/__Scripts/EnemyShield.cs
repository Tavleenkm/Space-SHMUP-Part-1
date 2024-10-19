using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent( typeof(BlinkColorOnHit) )]
public class EnemyShield : MonoBehaviour {
    [Header("Inscribed")]
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;

    void Start() {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ignoreOnCollisionEnter = true;  // This will not yet compile

        if (transform.parent == null) return;
        EnemyShield shieldParent = transform.parent.GetComponent<EnemyShield>();
        if (shieldParent != null) {
            shieldParent.AddProtector( this );
        }
    }

    /// <summary>
    /// Called by another EnemyShield to join the protectors of this EnemyShield
    /// </summary>
    /// <param name="shieldChild">The EnemyShield that will protect this</param>
    public void AddProtector( EnemyShield shieldChild ) {
        protectors.Add( shieldChild );
    }

    /// <summary>
    /// Shortcut for gameObject.activeInHierarchy and gameObject.SetActive()
    /// </summary>
    public bool isActive {
        get { return gameObject.activeInHierarchy; }
        private set { gameObject.SetActive(value); }
    }

    /// <summary>
    /// Called by Enemy_4.OnCollisionEnter() & parent's EnemyShields.
    /// to distribute damage to EnemyShield protectors.
    /// </summary>
    /// <param name="dmg">The amount of damage to be handled</param>
    /// <returns>Any damage not handled by this shield</returns>
    public float TakeDamage( float dmg ) {
        // Can we pass damage to a protector EnemyShield?
        foreach ( EnemyShield es in protectors ) {  // d
            if ( es.isActive ) {
                dmg = es.TakeDamage( dmg );
            }
            if (dmg == 0) return 0;  // e
        }

        // If the code gets here, then this EnemyShield will blink & take damage
        // Make the blinker blink
        blinker.SetColors();  // This will appear underlined in red for now

        health -= dmg;
        if ( health <= 0 ) {
            // Deactivate this EnemyShield GameObject
            isActive = false;  // g
            // Return any damage that was not absorbed by this EnemyShield
            return -health;  // h
        }

        return 0;
    }
}
