using System;
using UnityEngine;

namespace FlockingDemo
{
    public class FlockClass : MonoBehaviour
    {
        public FlockManagerClass myManager;
        private float speed;

        // LAB 5 ADDITION: Class level variables for turning logic
        private bool turning = false;
        private Vector3 direction = Vector3.zero;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // SAFETY CHECK: If I don't have a manager, I shouldn't exist!
            if (myManager == null)
            {
                Debug.LogError("I AM THE PROBLEM! Destroying " + gameObject.name);
                Destroy(this.gameObject);
                return;
            }

            // FIX: Explicitly use UnityEngine.Random
            speed = UnityEngine.Random.Range(myManager.minSpeed, myManager.maxSpeed);
        }

        // LAB 5 ADDITION: FixedUpdate for Physics and Boundary checks
        void FixedUpdate()
        {
            // 1. Boundary Detection
            // Create bounds based on the Manager's position and the flockBoundary size (multiplied by 2 for total size)
            Bounds b = new Bounds(myManager.transform.position, myManager.flockBoundary * 2);

            // If we are OUTSIDE the bounds, we must turn back
            //[Skill31] Programmatically use Bounds to limit GameObject movement
            //created a Bounds object and checked .Contains()
            if (!b.Contains(transform.position))
            {
                turning = true;
                // Direction towards the center of the flock manager
                direction = myManager.transform.position - transform.position;
            }
            else
            {
                turning = false;
            }

            // 2. Obstacle Avoidance (SphereCast)
            RaycastHit hit;
            // Cast a sphere forward to detect obstacles
            //[Skill 33] Determine an impending collision and execute code to avoid it.
            if (Physics.SphereCast(transform.position, 0.2f, transform.forward, out hit, 2.0f))
            {
                turning = true;
                // Reflect our current forward vector off the hit normal to find the new direction
                direction = Vector3.Reflect(transform.forward, hit.normal);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // LAB 5 ADDITION: Handling the Turning state
            if (turning)
            {
                // Smoothly rotate towards the direction calculated in FixedUpdate
                Quaternion dirToFace = Quaternion.LookRotation(direction);
                this.transform.rotation = Quaternion.Slerp(
                                            this.transform.rotation,
                                            dirToFace,
                                            myManager.rotationSpeed * Time.deltaTime);
            }
            else
            {
                // If not turning, apply standard flocking rules
                ApplyRules();
            }

            // Natural movement probability
            // Recalculate speed 10% of the time to make it look less robotic
            if (UnityEngine.Random.Range(0, 100) < 10)
            {
                speed = UnityEngine.Random.Range(myManager.minSpeed, myManager.maxSpeed);
            }

            this.transform.Translate(0f, 0f, Time.deltaTime * speed);
        }

        // apply boid rules
        void ApplyRules()
        {
            GameObject[] gos = myManager.allBoids;

            Vector3 vCenter = Vector3.zero; // center of the flock: attraction
            Vector3 vAvoid = Vector3.zero; // direction to move to avoid collision

            float gSpeed = 0f; // group speed
            float nDistance = 0f; // distance to neighbour

            int groupSize = 0; // size of the local group.

            foreach (GameObject go in gos)
            {
                // [Skill 26] Skip an item in a foreach loop
                // if this boid is me, ignore it.
                if (go == this.gameObject) continue;

                // how far away is this boid from me?
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                // is the boid close enough for me to care?
                if (nDistance < myManager.neighbourDistance)
                {
                    // [Skill 22] Add one variable to another by only typing each of their names once
                    vCenter += go.transform.position;
                    groupSize++;

                    // am I too close?
                    if (nDistance < myManager.neighbourCollision)
                    {
                        // go the opposite way.
                        // [Skill 22] Add one variable to another by only typing each of their names once
                        vAvoid += (this.transform.position - go.transform.position);
                    }

                    FlockClass anotherFlock = go.GetComponent<FlockClass>();
                    gSpeed += anotherFlock.speed;
                }
            }
            // we've visited all the boids, now do the adjustment JUST ONCE!

            if (groupSize > 0)
            {
                //[Skill32] Calculate the average of a transform property.
                vCenter = vCenter / groupSize;
                speed = gSpeed / groupSize;

                Vector3 direction = (vCenter + vAvoid) - this.transform.position;

                if (direction != Vector3.zero)
                {
                    Quaternion dirToFace = Quaternion.LookRotation(direction);
                    this.transform.rotation = Quaternion.Slerp(
                                                this.transform.rotation,
                                                dirToFace,
                                                myManager.rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}