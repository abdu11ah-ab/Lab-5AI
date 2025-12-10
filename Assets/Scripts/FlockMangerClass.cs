using System;
using UnityEngine;

namespace FlockingDemo
{
    public class FlockManagerClass : MonoBehaviour
    {

        public GameObject boidPrefab;
        public int numBoids = 20;
        public Vector3 boidLimits = new Vector3(5f, 5f, 5f);

        // LAB 5 ADDITION: Boundary for the tank
        public Vector3 flockBoundary = new Vector3(5f, 5f, 5f);

        public GameObject[] allBoids;

        [Header("Boid Settings")]
        [Range(0f, 5f)]
        public float minSpeed;
        [Range(0f, 5f)]
        public float maxSpeed;
        [Range(1f, 10f)]
        public float neighbourDistance; // local proximity to consider as flock.
        [Range(0f, 3f)]
        public float neighbourCollision; // local proximity to consider as collision.
        [Range(0f, 5f)]
        public float rotationSpeed;


        // Instantiate All Boids
        void Start()
        {
            allBoids = new GameObject[numBoids];
            for (int i = 0; i < numBoids; i++)
            {
                // FIX: Explicitly use UnityEngine.Random
                Vector3 pos = this.transform.position + new Vector3(
                        UnityEngine.Random.Range(-boidLimits.x, boidLimits.x),
                        UnityEngine.Random.Range(-boidLimits.y, boidLimits.y),
                        UnityEngine.Random.Range(-boidLimits.z, boidLimits.z)
                    );
                // [Skill 24] Populate class properties in an instantiated object from the instantiating class
                //instantiated the boid, grabbed its script component, and set the myManager variable immediately.
                allBoids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
                allBoids[i].GetComponent<FlockClass>().myManager = this;
            }

        }
    }
}