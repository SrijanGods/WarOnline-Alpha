class MovePath {

	private var nodes : GameObject[];
	private var nodesVector3 : List.<Vector3> = new List.<Vector3>();
	private var animationCurvePosX : AnimationCurve = new AnimationCurve();
	private var animationCurvePosY : AnimationCurve = new AnimationCurve();
	private var animationCurvePosZ : AnimationCurve = new AnimationCurve();
	private var gameObject : GameObject;
	private var gameObjectPath : String;
	private var animation : Animation;
	private var clip : AnimationClip;
	private var movingSpeed : float = 1.0;
	
	function importNodes(firstNode : GameObject, arr : GameObject[])
	{
		nodes = arr;
		//Debug.Log(nodes.length);
	
		this.animationCurvePosX = new AnimationCurve();
		this.animationCurvePosY = new AnimationCurve();
		this.animationCurvePosZ = new AnimationCurve();
		
		var f = 0; //frame
		
		this.animationCurvePosX.AddKey(Keyframe(f, firstNode.transform.localPosition.x));
		this.animationCurvePosY.AddKey(Keyframe(f, firstNode.transform.localPosition.y));
		this.animationCurvePosZ.AddKey(Keyframe(f, firstNode.transform.localPosition.z));
		
		animationCurvePosX.SmoothTangents(f, 0.0);
		animationCurvePosY.SmoothTangents(f, 0.0);
		animationCurvePosZ.SmoothTangents(f, 0.0);
		
		for (var g : GameObject in arr)
		{
			f++;
			
			this.animationCurvePosX.AddKey(Keyframe(f, g.transform.localPosition.x));
			this.animationCurvePosY.AddKey(Keyframe(f, g.transform.localPosition.y));
			this.animationCurvePosZ.AddKey(Keyframe(f, g.transform.localPosition.z));
			
			animationCurvePosX.SmoothTangents(f, 0.0);
			animationCurvePosY.SmoothTangents(f, 0.0);
			animationCurvePosZ.SmoothTangents(f, 0.0);
		}
	}
	
	function importNodesVector3(firstNode : Vector3, arr : List.<Vector3>)
	{
		nodesVector3 = arr;
		//Debug.Log(nodes.length);
	
		this.animationCurvePosX = new AnimationCurve();
		this.animationCurvePosY = new AnimationCurve();
		this.animationCurvePosZ = new AnimationCurve();
		
		var f = 0; //frame
		
		this.animationCurvePosX.AddKey(Keyframe(f, firstNode.x));
		this.animationCurvePosY.AddKey(Keyframe(f, firstNode.y));
		this.animationCurvePosZ.AddKey(Keyframe(f, firstNode.z));
		
		animationCurvePosX.SmoothTangents(f, 0.0);
		animationCurvePosY.SmoothTangents(f, 0.0);
		animationCurvePosZ.SmoothTangents(f, 0.0);
		
		for (var g : Vector3 in arr)
		{
			f++;
			
			this.animationCurvePosX.AddKey(Keyframe(f, g.x));
			this.animationCurvePosY.AddKey(Keyframe(f, g.y));
			this.animationCurvePosZ.AddKey(Keyframe(f, g.z));
			
			animationCurvePosX.SmoothTangents(f, 0.0);
			animationCurvePosY.SmoothTangents(f, 0.0);
			animationCurvePosZ.SmoothTangents(f, 0.0);
		}
	}
	
	function setGameObject(g : GameObject)
	{
		this.gameObject = g;
		this.gameObjectPath = ""; //so lassen!
		this.animation = g.GetComponent.<Animation>();
	}
	
	function setMovingSpeed(speed : float)
	{
		this.movingSpeed = speed;
	}
	
	function applyAnimation()
	{
		this.clip = new AnimationClip();
		this.clip.SetCurve(this.gameObjectPath, Transform, "localPosition.x", this.animationCurvePosX);
		this.clip.SetCurve(this.gameObjectPath, Transform, "localPosition.y", this.animationCurvePosY);
		this.clip.SetCurve(this.gameObjectPath, Transform, "localPosition.z", this.animationCurvePosZ);
		this.clip.wrapMode = WrapMode.Once;
		this.clip.name = "movepath";
		this.animation.AddClip(this.clip, this.clip.name);
		this.animation[this.clip.name].speed = this.movingSpeed;
		this.animation[this.clip.name].wrapMode = WrapMode.Once;
		this.animation.Play(this.clip.name);
	}
	
	function hasFinished()
	{
		return !this.animation[this.clip.name].enabled;
	}
	
	function getNextKeyframe()
	{
		return Mathf.Round(this.animation[this.clip.name].time);
	}
	
	function getNextKeyframeObject()
	{
		if(Mathf.Round(this.animation[this.clip.name].time) < nodes.length
		&& nodes[Mathf.Round(this.animation[this.clip.name].time)] != null)
		{
			return nodes[Mathf.Round(this.animation[this.clip.name].time)];
		}
		else
		{
			return nodes[nodes.length-1];
		}
	}
	
	function getNextKeyframeObjectVector3()
	{
		if(Mathf.Round(this.animation[this.clip.name].time) < nodesVector3.Count
		&& nodesVector3[Mathf.Round(this.animation[this.clip.name].time)] != null)
		{
			return nodesVector3[Mathf.Round(this.animation[this.clip.name].time)];
		}
		else
		{
			return nodesVector3[nodesVector3.Count-1];
		}
	}
	
}