using UnityEngine;
using System.Collections;

public class ChooseLevel : MonoBehaviour
{
	public static GameObject[] menuButtons;
	//public static GameObject[] menuArrows;
	
	public GameObject levelOne;
	public GameObject levelTwo;
	public GameObject backButton;

	public static int currentIndex;
	public static int numButtons = 3;
	
	bool axisInUse;
	public static bool movementLocked;
	
	
	// Use this for initialization
	void Start ()
	{
		
		menuButtons = new GameObject[numButtons];
		//menuArrows = new GameObject[numButtons];
		currentIndex = 0;
		
		menuButtons [0] = levelOne;
		menuButtons [1] = levelTwo;
		menuButtons [2] = backButton;

		
		menuButtons [0].GetComponent<MenuButton> ().IsUnlocked = true;
		menuButtons [0].GetComponent<MenuButton> ().IsSelected = true;
		//menuArrows [0].GetComponent<SpriteRenderer> ().enabled = true;
		
		menuButtons [1].GetComponent<MenuButton> ().IsUnlocked = true;
		menuButtons [2].GetComponent<MenuButton> ().IsUnlocked = true;
		
		//fill array here with menu button objects

		movementLocked = true;
		enabled = false;
	}
	
	// Update is called once per frame
	void Update ()                         //TODO cursor movement happens here!!!
	{
		Debug.Log (movementLocked);
		if (!movementLocked) {
			if (Input.GetAxisRaw ("Horizontal") == -1) { //NOTE: the parameter passed to cycle is reversed due to this being a horizontal menu
				if (!axisInUse) {
					Cycle (1);
					
					axisInUse = true;
				}
				
			} else if (Input.GetAxisRaw ("Horizontal") == 1) {
				if (!axisInUse) {
					Cycle (-1);
					
					axisInUse = true;
				}
				
			} else if (Input.GetAxisRaw ("Horizontal") == 0) {
				axisInUse = false;
			}
			
			if (Input.GetButtonDown ("Jump")) {
				
				menuButtons [currentIndex].GetComponent<MenuButton> ().OnSelectButton ();
			}
			
		}
	}
	
	public  void Cycle (int direction)
	{
		menuButtons [currentIndex].GetComponent<MenuButton> ().IsSelected = false;
		menuButtons [currentIndex].GetComponent<Animator> ().SetBool ("isHighlighted", false);
		//TODO menuArrows [currentIndex].GetComponent<SpriteRenderer> ().enabled = false;
		
		do {
			
			currentIndex -= direction;
			
			
			if (currentIndex < 0) {
				currentIndex = menuButtons.Length - 1;
				
				
				
				
			} else if (currentIndex > menuButtons.Length - 1) {
				currentIndex = 0;
				
				
				
			}
			
			
			
		} while(!menuButtons[currentIndex].GetComponent<MenuButton>().IsUnlocked);
		
		menuButtons [currentIndex].GetComponent<MenuButton> ().IsSelected = true;
		menuButtons [currentIndex].GetComponent<Animator> ().SetBool ("isHighlighted", true);
		//menuArrows [currentIndex].GetComponent<SpriteRenderer> ().enabled = true;
		
		Debug.Log ("Current index:" + currentIndex);
	}

	public static void LeavingScreen ()
	{
		menuButtons [currentIndex].GetComponent<MenuButton> ().IsSelected = false;
		menuButtons [currentIndex].GetComponent<Animator> ().SetBool ("isHighlighted", false);
		currentIndex = 0;
		movementLocked = true;
	}

	public static void EnteringScreen ()
	{
		//currentIndex = 0;
		menuButtons [currentIndex].GetComponent<MenuButton> ().IsSelected = true;
		menuButtons [currentIndex].GetComponent<Animator> ().SetBool ("isHighlighted", true);
		movementLocked = false;
	}
}
