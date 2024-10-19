using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour {
    [Header("Inscribed")]
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 10; // PowerUp will exist for # seconds
    public float fadeTime = 4;  // Then it fades over # seconds

    [Header("Dynamic")]
    public eWeaponType type;           // The type of the PowerUp
    public GameObject cube;            // Reference to the PowerCube child
    public TextMesh letter;            // Reference to the TextMesh
    public Vector3 rotPerSecond;       // Euler rotation speed for PowerCube
    public float birthTime;            // The Time.time this was instantiated

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake() {
        // Find the Cube reference (there's only a single child)
        cube = transform.GetChild(0).gameObject;
        // Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        // Set a random velocity
        Vector3 vel = Random.onUnitSphere; // Get Random XYZ velocity
        vel.z = 0; // Flatten the vel to the XY plane
        vel.Normalize(); // Normalizing a Vector3 sets its length to 1m
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Set the rotation of this PowerUp GameObject to R:[0, 0, 0]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity is equal to no rotation.

        // Randomize rotPerSecond for PowerCube using rotMinMax x & y
        rotPerSecond = new Vector3( Random.Range(rotMinMax[0], rotMinMax[1]),
                                    Random.Range(rotMinMax[0], rotMinMax[1]),
                                    Random.Range(rotMinMax[0], rotMinMax[1]) );

        birthTime = Time.time;
    }

    void Update() {
        cube.transform.rotation = Quaternion.Euler( rotPerSecond * Time.time );

        // Fade out the PowerUp over time
        // Given the default values, a PowerUp will exist for 10 seconds
        // and then fade out over 4 seconds.
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // If u >= 1, destroy this PowerUp
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }
        // If u < 0, don't fade yet.
        if (u < 0) return;
        // Else, fade out this PowerUp
        Color c = cubeMat.color;
        c.a = 1f - u;
        cubeMat.color = c;
        // Fade the Letter too, just not as much
        c = letter.color;
        c.a = 1f - (u * 0.5f);
        letter.color = c;

        if (!bndCheck.isOnScreen) {
            // If the PowerUp has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }
    }

    public void SetType(eWeaponType wt) {
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        cubeMat.color = def.powerUpColor;   // We could colorize the PowerCube
        // letter.color = def.color;        // We could colorize the Letter too
        letter.text = def.letter;           // Set the letter that is shown
        type = wt;                          // Finally actually set the type
    }

    /// <summary>
    /// This function is called by the Hero class when a PowerUp is collected.
    /// </summary>
    /// <param name="target">The GameObject absorbing this PowerUp</param>
    public void AbsorbedBy(GameObject target) {
        Destroy(this.gameObject);
    }
}
