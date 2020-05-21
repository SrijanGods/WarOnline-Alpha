import System.Collections.Generic;

class Trajectory {
	private var curve : List.<Vector3> = new List.<Vector3>();
	private var maxHit : int;
	private var currentHit : int;
	private var resolution : int;
	private var targetHit : RaycastHit;
	
	function calc(start : Transform, maxLength : float, maxHit : int, resolution : int, decrement : float, collider : Collider) {
		this.maxHit = maxHit;
		this.currentHit = 0;
		this.resolution = resolution;
		
		var pos = start.position;
		var fwd = start.forward;
		
		this.curve = new List.<Vector3>();
		this.curve.Add(pos);
		subCalc(pos, fwd, maxLength / this.resolution, decrement, collider);
		
		return this.curve;
	}
	
	private function subCalc(pos : Vector3, fwd : Vector3, maxLength : float, decrement : float, collider : Collider) {
		var hit : RaycastHit;
		var ray : Ray;
		
		if(this.currentHit <= this.maxHit) {
			ray = new Ray(pos, fwd);
			if(!Physics.SphereCast(ray, collider.bounds.extents.x, hit, maxLength)) {
				this.curve.Add(ray.GetPoint(maxLength));
				maxLength -= 0.001 / this.resolution;
				fwd.y -= decrement / this.resolution;
				if(this.curve[this.curve.Count-1].y >= 0 && maxLength > 0) {
					subCalc(this.curve[this.curve.Count-1], fwd, maxLength, decrement, collider);
				}
			} else {
				if(hit.collider != collider
				&& !collider.bounds.Intersects (hit.collider.bounds)) {
					this.currentHit++;
					//this.curve.Add(ray.GetPoint(maxLength));
					targetHit = hit;
					fwd.y += 1.0 / this.resolution;
					subCalc(this.curve[this.curve.Count-1], Vector3.Reflect(fwd, hit.normal), maxLength, decrement, collider);
				}
			}
		}
	}
	
	function getTarget() {
		return targetHit;
	}
}

class TrajectoryMode {
	var name : String = "Mode";
	var decrement : float = 0.05;
	var startAngle : int = 340;
}