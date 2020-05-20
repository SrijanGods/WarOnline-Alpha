using UnityEngine;
using System;
using System.Collections;

namespace Obi{

	/**
	 * Small helper class that lets you specify Obi-only properties for rigidbodies.
	 */

	[ExecuteInEditMode]
	public abstract class ObiRigidbodyBase : MonoBehaviour
	{
		public bool kinematicForParticles = false;

		protected IntPtr oniRigidbody = IntPtr.Zero;
		protected bool dirty = true;

		protected Oni.Rigidbody adaptor = new Oni.Rigidbody();
		protected Oni.RigidbodyVelocities oniVelocities = new Oni.RigidbodyVelocities();

		protected Vector3 velocity, angularVelocity;

		public IntPtr OniRigidbody {
			get{return oniRigidbody;}
		}

		public void Awake(){
			oniRigidbody = Oni.CreateRigidbody();
			UpdateIfNeeded();
		}

		public void OnDestroy(){
			Oni.DestroyRigidbody(oniRigidbody);
			oniRigidbody = IntPtr.Zero;
		}

		public abstract void UpdateIfNeeded();

		/**
		 * Reads velocities back from the solver.
		 */
		public abstract void UpdateVelocities();

	}
}

