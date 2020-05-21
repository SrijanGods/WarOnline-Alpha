import System.Collections.Generic;

var trajectoryStart : Transform;
var trajectoryStrength : float = 1; //length of each line segment
var trajectoryMaxCollisions : int = 10;
var trajectoryResolution : int = 5;
var trajectoryMode : List.<TrajectoryMode> = new List.<TrajectoryMode>();
var trajectoryCurve : List.<Vector3> = new List.<Vector3>();
var throwThing : GameObject;

private var isMovingToTarget : boolean = false;
private var isReachedTarget : boolean = false;
private var traj : Trajectory = new Trajectory();
private var movePath : MovePath = new MovePath();
private var trajectoryDecrement : float;

function Start () {

	SetTrajectoryMode(0);

}

function Update () {

	if(!isMovingToTarget) {
		Show(trajectoryStrength, trajectoryMaxCollisions, throwThing.GetComponent.<Collider>());
	} else {
		hasReachedTarget();
	}

}

function OnGUI () {

	if(GUI.Button(Rect(10,10,150,50), "Set Grenade Mode")) {
		SetTrajectoryMode(0);
	}
	
	if(GUI.Button(Rect(10,70,150,50), "Set Straight Mode")) {
		SetTrajectoryMode(1);
	}
	
	if(GUI.Button(Rect(10,130,150,50), "Throw it!")) {
		Throw();
	}

}

function SetTrajectoryMode(index : int) {
	trajectoryDecrement = trajectoryMode[index].decrement;
	trajectoryStart.localEulerAngles.x = trajectoryMode[index].startAngle;
}

function Show(strength : float, bounces : int, collider : Collider) {
	trajectoryStrength = strength;
	trajectoryMaxCollisions = bounces;
	trajectoryCurve = traj.calc(trajectoryStart, trajectoryStrength, trajectoryMaxCollisions, trajectoryResolution, trajectoryDecrement, collider);
	
	GetComponent(LineRenderer).SetVertexCount(trajectoryCurve.Count);
	var i : int = 0;
	for (var vec : Vector3 in trajectoryCurve) {
		GetComponent(LineRenderer).SetPosition(i, vec);
		i++;
	}
}

function Throw() {
	movePath = new MovePath();
	movePath.importNodesVector3(trajectoryCurve[0], trajectoryCurve);
	movePath.setGameObject(throwThing);
	movePath.setMovingSpeed(150.0);
	movePath.applyAnimation();
	Debug.Log("flies ...");
	isMovingToTarget = true;
	isReachedTarget = false;
}

function getTarget() {
	return traj.getTarget();
}

function hasReachedTarget() {

	if(isMovingToTarget)
	{
		if(movePath.hasFinished()) {
			Debug.Log("hits target");
			isMovingToTarget = false;
			isReachedTarget = true;
			//Destroy(throwThing, 0);
			//Hide();
		}
	}
	
	return isReachedTarget;
}

function Hide() {
	trajectoryCurve = new List.<Vector3>();
	GetComponent(LineRenderer).SetVertexCount(0);
}