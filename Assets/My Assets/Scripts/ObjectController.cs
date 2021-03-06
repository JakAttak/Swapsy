﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : HelperFunctions {
	
	// Variables that our objects need
	public string message;
	
	public float interactionRange = 20f;

	public Vector3 messagePosition = new Vector3(0.5f, 1f, 0.5f);
	
	public bool actionable = false;
	public bool nearPlayer = false;
	
	private bool displayingText = false;
	private GameObject textObj;
	
	public string playerNearbyMessage;
	
	public virtual void Start() {
		message = message.Replace("\\\\n", "\n");
		playerNearbyMessage = playerNearbyMessage.Replace("\\\\n", "\n");

		if (playerNearbyMessage == "") {
			setNearbyMessage(defaultNearbyMessage());
		}
		
		createTextObj();
		
		handleLocking();
	}

	// Returns what the default message should be if one isnt already set
	public virtual string defaultNearbyMessage() {
		return "I am a " + gameObject.tag;
	}

	// Updates the object's nearby message
	public void setNearbyMessage(string message) {
		playerNearbyMessage = message;
	}

	// Instantiate the sign that goes above our head
	void createTextObj() {
		textObj = Instantiate(Resources.Load("Sign", typeof(GameObject))) as GameObject;
		textObj.GetComponent<TextMesh>().fontSize = 100 * (int) (transform.localScale.y * ((BoxCollider) GetComponent<Collider>()).size.y);
	}
	
	// Lock the physics of non-player objects so they can't be pushed around
	public virtual void handleLocking() {
		if (gameObject.tag == "Player") {
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
			GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		} else if (gameObject.tag == "PlayerChoice") {
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
			GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
	}
	
	// Update
	public virtual void Update() {
		if (displayingText) { 
			Vector3 sV = new Vector3(0f, 0f, 0f);
			sV.x = transform.localScale.x * ((BoxCollider) GetComponent<Collider>()).size.x;
			sV.y = transform.localScale.y * ((BoxCollider) GetComponent<Collider>()).size.y;
			sV.z = transform.localScale.z * ((BoxCollider) GetComponent<Collider>()).size.z;

			sV = transform.rotation * sV;

			float xP = transform.position.x - sV.x / 2 + sV.x * messagePosition.x;
			float yP = transform.position.y - sV.y / 2 + sV.y * messagePosition.y;
			float zP = transform.position.z - sV.z / 2 + sV.z * messagePosition.z;

			textObj.transform.position = new Vector3(xP, yP + textObj.GetComponent<TextMesh>().GetComponent<Renderer>().bounds.size.y / 2, zP);
			textObj.transform.LookAt(textObj.transform.position + Camera.main.GetComponent<CameraController>().lookOffset()); // rotate text to face the camera
		}
	}

	// Helper function that checks if you are in range of a player
	public void setNearPlayer(bool near) {
		nearPlayer = near;
	}

	// Handles interactions (default is no interactions)
	public virtual void handleInteractions(GameObject cause) {

	}
	
	// Displays message above head when player is nearby.  
	public void displayInteractions() {
		if (!displayingText) {
			displayText(playerNearbyMessage);
		}
	}
	
	// Changes the text to be displayed above head
	public void displayText(string txt) {
		displayingText = true;
		textObj.GetComponent<TextMesh>().text = txt;
		textObj.GetComponent<Renderer>().enabled = true;
	}
	
	// Color the text
	public void colorText(Color col) {
		textObj.GetComponent<TextMesh>().color = col;
	}
	
	// Hides the text above the head, by clearing it
	public void hideText() {
		displayingText = false;
		textObj.GetComponent<Renderer>().enabled = false;
	}

	// Change the font size of our text
	public void setFontSize(int fs) {
		textObj.GetComponent<TextMesh>().fontSize = fs;
	}

	// Some objects will have a message they can display to you when prompted (like players)
	public void speak() {
		if (message != "") {
			displayText(message);
		}
	}
}
