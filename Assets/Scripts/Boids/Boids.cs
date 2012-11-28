using UnityEngine;
using System.Collections;

public class Boids : MonoBehaviour {
	
	public Boid boid;
	public int number_of_boids = 10;  
	
	public float cohesionFactor = 0.5f;
	public float repulsionFactor = 3.0f;
	public float velocitySimilarityFactor = 0.01f;

	public float speed = 2.0f;

	public bool	maintainConstantHeight = false;
	public bool flockHasLeader = false;
	public bool stationaryCenter = true;
	public bool fixYValue = false;
	
	public Vector3 center = Vector3.zero;
	
	private Boid[] boidsarray;
	
	// Use this for initialization
	void Start () {

		boidsarray = new Boid[number_of_boids];
		if (!stationaryCenter) {
			center = Vector3.zero;	
		} else {
			center = transform.position;
		}
		
		for (int i=0; i < number_of_boids; i++) {
			Boid b = Instantiate(boid, new Vector3( Random.value, Random.value, Random.value), Quaternion.identity) as Boid; 
			b.transform.parent = transform;
			b.transform.localScale = Vector3.one;
			boidsarray[i] = b; 
		}	
	}
	
	void Update () {
				
		if (!stationaryCenter) {
			center = Vector3.zero;
			for (int i = 0; i < number_of_boids; i++) {		
				center += boidsarray[i].transform.position / number_of_boids;
			}	
		}
		
		for (int i = 0; i < number_of_boids; i++) {

				boidsarray[i].velocity += getCenterAttractor(i); // boids will all fly towards center
				boidsarray[i].velocity += getRepulsion(i); // repulse from nearby boids
				boidsarray[i].velocity += matchVelocity(i); // match velocity
	
				if (fixYValue) {
					boidsarray[i].velocity.y = 0.0f;
				}
			
				boidsarray[i].velocity = Vector3.Normalize(boidsarray[i].velocity);
	
				boidsarray[i].transform.rotation = Quaternion.LookRotation(boidsarray[i].velocity);
				boidsarray[i].transform.Translate(Vector3.forward * speed * Time.deltaTime);
				boidsarray[i].transform.Rotate(Vector3.up * 90.0f);
		}
	}
		
	Vector3 matchVelocity(int boidIndex)
	{
		Vector3 perceivedVelocity = Vector3.zero;
		
		// adjust boid velocity to be nearer to neighboring velocities
		for (int i = 0; i < number_of_boids; i++)
		{
			if (i != boidIndex)
			{
				perceivedVelocity += boidsarray[i].velocity;
			}
		}
		
		perceivedVelocity /= (number_of_boids - 1);
		return perceivedVelocity * Time.deltaTime * velocitySimilarityFactor;
	}
	
	Vector3 getRepulsion(int boidIndex)
	{
	// move away from nearby boids	
		Vector3 repulsion = Vector3.zero;
		int somethreshold = 2; // eh local
		
		for (int i=0; i < number_of_boids; i++)
		{
			if (i != boidIndex) 
			{
				// add to repulsion 
				if ((boidsarray[i].transform.position - boidsarray[boidIndex].transform.position).sqrMagnitude < somethreshold)
				{
					repulsion += boidsarray[boidIndex].transform.position - boidsarray[i].transform.position;
				}
			}
		}
		
		return repulsion * Time.deltaTime * repulsionFactor;
	}

	Vector3 getCenterAttractor(int boidIndex)
	{
		// RULE THE FIRST!!
		// for boid at boidIndex in boidsarray, returns vector3 distance to center
		Vector3 attractorPoint;
		
		if (flockHasLeader) {
			attractorPoint = boidsarray[0].transform.position;
		} else {
			attractorPoint = center;
		}
		
		return (attractorPoint - boidsarray[boidIndex].transform.position) * cohesionFactor * Time.deltaTime;
	}
}