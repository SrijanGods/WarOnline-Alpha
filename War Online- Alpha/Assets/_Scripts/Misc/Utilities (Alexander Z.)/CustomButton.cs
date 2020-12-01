using UnityEngine;
using System.Collections;

public class CustomButton {
    public string name;
    public bool enabled;
    public bool IsVisible;
    private Vector2 myDimensions = new Vector2(1, 1);
    public Vector2 Dimensions
    {
        get
        {
            return myDimensions;
        }

        set
        {
            myDimensions = value;
        }
    }

    private Vector2 myPosition = new Vector2(0,0); // in rectangle coordinates see Rect
    public Vector2 Position
    {
        get
        {
            return myPosition;
        }

        set
        {
            if (value.x > 1 || value.x < 0 || value.y > 1 || value.y < 0) //if invalid input

            {
                //find and correct invalid input
                Debug.Log("Incalid usage: Position must have a vector with values [0,1] ");

                float x = value.x;
                float y = value.y;

                if (value.x > 1){
                    x = 1;
                } 
                else if (value.x < 0)
                    x = 0;

                if (value.y > 1)
                    y = 1;
                else if (value.y < 0)
                    y = 0;

                myPosition = new Vector2(x, y);

            }
            else //if valid input
            {
                myPosition = value;
            }
            
            switch (Anchor)
            {
                case 0: //Upper Left
                    area = new Rect(Position.x * Screen.width,
                                    Position.y * Screen.width,
                                    Dimensions.x * unit,
                                    Dimensions.y * unit);
                    break;
                case 1: //Upper Right
                    area = new Rect((Screen.width - Dimensions.x * unit) - Position.x * Screen.width,
                                    Position.y * Screen.width,
                                    Dimensions.x * unit,
                                    Dimensions.y * unit);
                    break;
                case 2: //Lower Right
                    area = new Rect((Screen.width - Dimensions.x * .1f * Screen.width) - Position.x * Screen.width,
                                    (Screen.height - Dimensions.y * .1f * Screen.width) - Position.y * Screen.width,
                                    Dimensions.x * unit,
                                    Dimensions.y * unit);
                    break;
                case 3: //Lower Left
                    area = new Rect(Position.x * Screen.width,
                                    (Screen.height - Dimensions.y * .1f * Screen.width) - Position.y * Screen.width,
                                    Dimensions.x * unit,
                                    Dimensions.y * unit);
                    break;
                default:    //ERROR
                    Debug.LogError("ERROR: Invalid Anchor value.");
                    break;
            }
        }
    }

    private float unit = Screen.width * .1f; // This is the unit of choice, Buttons will be relative to this unit. Which is 1/10 the width of the screen.

    private int tiling_anchor; //The anchor is the origin of the coordinates. Moving both horizontally and vertically ON screen is in positive direction.
    public int Anchor
    {
        get
        {
            return tiling_anchor;
        }

        set
        {
            if (value < 0 || value > 3)
            {
                Debug.LogError("Invalid: Anchor value set a value from 0-3.");
                if (value < 0)
                    tiling_anchor = 0;
                else if (value > 3)
                    tiling_anchor = 3;
            }
            else
                tiling_anchor = value;
        }
    }

    //public Vector2 dimensions; // Vector2(1,1) is a squared button of 1/10*Screen.width by 1/10*Screen.width (is in multiples of 1/10 screen width.)
    private Texture2D img;
    public Texture2D image
    {
        get { return img; }
        set {  img=value;
            active_image = img;
        }
    }

    public Texture2D pressed_image;
    private Texture2D active_image;
    private Rect area; // This is the CustomButton.area

    public delegate void ButtonAction();
    public event ButtonAction OnPress;
    public event ButtonAction OnHold;


	// Use this for initialization
	public void Start () {
        active_image = image;
        state = states.WAITING_TOUCH;
        fingerID = -1; // undefined
	}

    public CustomButton()
    {
        active_image = image;
        state = states.WAITING_TOUCH;
        fingerID = -1; // undefined
    }

    Touch? GetTouchByID(int fingerId)
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == fingerId)
                {
                    return touch;
                }
            }
        }

        return null;
    }

    public void OnGUI()
    {
        if (enabled)
        {
            DrawButton();
            Behavioral();
        }
        else// disabled showing. Still asociates finge.
        {
            if (IsVisible == true)
            {
                DrawButton();
                Behavioral();
                DrawButtonBackground();
            }
        }
        
    }

    public void DrawButton(){
        GUI.backgroundColor = new Color(1.0f, 1.00f, 1.0f, 1.0f);
        GUI.Box(area, active_image);
 
       // Graphics.DrawTexture(area,active_image);
    //        Debug.Log(area);
    }

    public void DrawButtonBackground()
    {
        GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1f);
        GUI.Box(area, "");
        GUI.backgroundColor = new Color(1.0f, 0.0f, 0.0f, 0.0f);
    }

    private enum states { WAITING_TOUCH, BEGAN_INSIDE, CONTINUE_INSIDE, ENDED_INSIDE, CONTINUE_OUTSIDE, ENDED_OUTSIDE, ENDED }
    private states state;
    public int fingerID;
    private Touch? touch_handle; // The ? after the type [Touch] signifies nullable. It can hold a touch or null.
    //Action on transition
    public void Behavioral()
    {
        //transitions
        switch(state){
            case states.WAITING_TOUCH:
                //Debug.Log("WAITING_TOUCH");
                foreach (Touch touch in Input.touches) { 
                    if( area.Contains( new Vector2(touch.position.x, Screen.height - touch.position.y)) && touch.phase == TouchPhase.Began ) 
                    {
                        Debug.Log("WAITING_TOUCH -> BEGAN_INSIDE");
                        state = states.BEGAN_INSIDE;
                        fingerID = touch.fingerId;
                        touch_handle = touch;
                        active_image = pressed_image;
                    }
                }
                break;
            case states.BEGAN_INSIDE:
                //Debug.Log("BEGAN_INSIDE");
                //Debug.Log("touch_handle.phase: " + touch_handle.Value.phase);
                touch_handle = GetTouchByID(fingerID);
                if (touch_handle.Value.phase == TouchPhase.Canceled || touch_handle.Value.phase == TouchPhase.Ended)
                {
                    fingerID = -1;
                   // Debug.Log("BEGAN_INSIDE -> WAITING_OUCH");
                    active_image = image;
                    state = states.WAITING_TOUCH;
                }else if (touch_handle.Value.phase == TouchPhase.Moved || touch_handle.Value.phase == TouchPhase.Stationary)
                {
                    //Debug.Log("BEGAN_INSIDE -> CONTINUE_INSIDE");
                    state = states.CONTINUE_INSIDE;
                }   
                break;
            case states.CONTINUE_INSIDE:
                //Debug.Log("CONTINUE_INSIDE");
                touch_handle = GetTouchByID(fingerID);
                if (touch_handle.Value.phase == TouchPhase.Canceled )
                {
                    fingerID = -1;
                    //Debug.Log("CONTINUE_INSIDE -> WAITING_TOUCH");
                    active_image = image;
                    state = states.WAITING_TOUCH;
                }
                else if (touch_handle.Value.phase == TouchPhase.Ended)
                {
                    //Debug.Log("CONTINUE_INSIDE -> ENDED_INSIDE");
                    state = states.ENDED_INSIDE;
                }else if( area.Contains( new Vector2(touch_handle.Value.position.x, Screen.height - touch_handle.Value.position.y)) == false && 
                       ( touch_handle.Value.phase == TouchPhase.Stationary || touch_handle.Value.phase == TouchPhase.Moved) ) 
                {
                   // Debug.Log("CONTINUE_INSIDE -> CONTINUE_OUTSIDE");
                    active_image = image;
                    state = states.CONTINUE_OUTSIDE;
                }
                if (enabled && OnHold != null)
                    OnHold();
                break;
            case states.ENDED_INSIDE:
                //Execute the event associated.
                if (enabled && OnPress != null)
                    OnPress();

                //Debug.Log("ENDED_INSIDE -> WAITING_TOUCH");
                active_image = image;
                fingerID = -1;
                state = states.WAITING_TOUCH;
                break;
            case states.CONTINUE_OUTSIDE:
                //Debug.Log("CONTINUE_OUTSIDE");
                touch_handle = GetTouchByID(fingerID);
                if (touch_handle.Value.phase == TouchPhase.Canceled)
                {
                    fingerID = -1;
                   // Debug.Log("CONTINUE_OUTSIDE -> WAITING_TOUCH");
                    active_image = image;
                    state = states.WAITING_TOUCH;
                }
                else if (touch_handle.Value.phase == TouchPhase.Ended)
                {
                    //Debug.Log("CONTINUE_OUTSIDE -> ENDED_OUTSIDE");
                    state = states.ENDED_OUTSIDE;
                }
                else if (area.Contains(new Vector2(touch_handle.Value.position.x, Screen.height - touch_handle.Value.position.y)) == true &&
                      (touch_handle.Value.phase == TouchPhase.Stationary || touch_handle.Value.phase == TouchPhase.Moved))
                {
                   // Debug.Log("CONTINUE_OUTSIDE -> CONTINUE_INSIDE");
                    active_image = pressed_image;
                    state = states.CONTINUE_INSIDE;
                }
                if (enabled && OnHold != null)
                    OnHold();
                break;
            case states.ENDED_OUTSIDE:
                //Debug.Log("ENDED_OUTSIDE");
                fingerID = -1;
                //Debug.Log("ENDED_OUTSIDE -> WAITING_TOUCH");
                active_image = image;
                state = states.WAITING_TOUCH;
                break;
            default:
                break;
        }
    }

    public bool point_touches(Vector2 point)
    {
        return area.Contains( point ); 
    }

    //returns null if none
    public Touch? get_associated_touch()
    {
        return touch_handle;
    }

    public void visible_disable()
    {
        enabled = false;
        IsVisible = true;
    }

    public void invisible_disable()
    {
        enabled = false;
        IsVisible = false;
    }

    public void visible_enabled()
    {
        enabled = true;
        IsVisible = true;
    }
}
