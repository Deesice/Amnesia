using UnityEngine;
using System.Collections;

public class Scenario_05_wine_cellar : Scenario {
private void Start() {}
//////////////////////
//BEGIN BLOCKED DOOR//
/*Move wooden barrel to unlock door
 */
public void FuncUnlockDoor(string  asParent, string  asChild, int alState)
{
	SetSwingDoorLocked("cellar_wood01_3", false, false);
	
	/*DEBUG
	 */
	AddDebugMessage("UnlockDoor!", true);
}
//END BLOCKED DOOR//
////////////////////


////////////////////////////////
//BEGIN MOANING & CREAK HORROR//
/*Start the creak sounds + ps when entering basement, stop them when leaving the basement/area
 */
public void FuncMoanHorrors(string  asParent, string  asChild, int alState)
{
	float fMoan = RandFloat(5.5f,15.5f);
	
	AddTimer("moanTimer", 4.5f+fMoan, "FuncMoanTimer");	
	AddTimer("stepTimer", 4.5f, "FuncMoanTimer");	
	
	PlaySoundAtEntity("moanSoundx", "scare_male_terrified5.snt", "Player", 0.0f, false);
	AddTimer("sanity", 0.7f, "TimerSanity");
	
	/*DEBUG
	 */
	AddDebugMessage("Begin moaning sounds", true);
}
/*Random placment of moan at random time
 */
public void FuncMoanTimer(string  asTimer)
{
	int iMoan = RandFloat(1, 9);	
	float fMoan = RandFloat(5.5f,15.5f);
	
	if(asTimer == "moanTimer") {
		PlaySoundAtEntity("moanSound"+iMoan, "scare_male_terrified.snt", "HorrorMoan_"+iMoan, 0.0f, false);
		
		AddTimer("moanTimer", 6.5f+fMoan, "FuncMoanTimer");
		
	} else if(asTimer == "stepTimer") {
		iMoan = RandFloat(1, 6);	
	
		PlaySoundAtEntity("stepSound"+iMoan, "scare_steps_big.snt", "HorrorMoan_"+iMoan, 0.0f, false);
		
		AddTimer("stepTimer", 7.5f+fMoan, "FuncMoanTimer");
	}	

	/*DEBUG
	 */
	AddDebugMessage("Now moaning in area: "+iMoan, false);
}
/*Random placment of creak at random time
 */
public void CreakTimer(string  asTimer)
{
	int iCreak = RandFloat(1, 12);	
	float fCreak = RandFloat(1.5f,7.5f);
	
	//ADD GRUNTING FROM GRUNT ENEMY
	PlaySoundAtEntity("creakSound"+iCreak, "scare_wood_creak_mix", "HorrorCreak_"+iCreak, 0.0f, false);
	CreateParticleSystemAtEntity("creakPS"+iCreak, "ps_dust_falling_small", "HorrorCreak_"+iCreak, false);
		
	AddTimer("creak", 0.5f+fCreak, "CreakTimer");
		
	/*DEBUG
	 */
	AddDebugMessage("Now creaking in area: "+iCreak, false);
}
public void TimerSanity(string  asTimer)
{
	GiveSanityDamage(10, true);
}
//END MOANING & CREAK HORROR//
//////////////////////////////


/////////////////////////////////////////////////////////////////
//BEGIN GRUNT ACTIVATION/DEACTIVATION(GRUNTS REMOVED FROM LEVEL//
/*When picking oil in barrel room slam door !PICK CHEMICAL INSTEAD!
 */
public void PickOil(string  asEntity, string  asType)
{
	//SetSwingDoorClosed("cellar_wood01_4", false, false);
	//SetSwingDoorDisableAutoClose("cellar_wood01_4", true);
	
	//AddPropImpulse("cellar_wood01_4", 2.0f, 0, 0, "World");
	
	/*if(!GetSwingDoorClosed("cellar_wood01_1"))
		PlaySoundAtEntity("SlamDoor","scare_slam_door.snt", "cellar_wood01_1", 0, false);
		
	SetSwingDoorClosed("cellar_wood01_1", true, true);
	PlaySoundAtEntity("gruntgruff", "amb_idle", "AreaGruntSound1", 0.0f, false);
	
	AddTimer("sanity", 1, "TimerSanity");

	PlaySoundAtEntity("PlayerScare","scare_tingeling.snt", "Player", 0, false);*/
}
/*Second grunt encounter
 */
public void CollideActivateGrunt(string  asParent, string  asChild, int alState)
{
	SetSwingDoorClosed("cellar_wood01_1", false, false);
	SetSwingDoorDisableAutoClose("cellar_wood01_1", true);
	
	AddPropImpulse("cellar_wood01_1", -2.0f, 0, 0, "World");
}
//END GRUNT ACTIVATION/DEACTIVATION//
/////////////////////////////////////


//////////////////////////////////////////////////
//BEGIN DEBRIS FALL DOWN AND LOCK PLAYER IN ROOM//
/*Three areas decide which barrel on the loft should fall down on players head during debris event
 */
public void CollideBlockDoor(string  asParent, string  asChild, int alState)
{
	SetLocalVarString("WhatBarrel", asChild);
}
/*When Picking the chem jar(it was a coin bag ...) in the room begin the debris event
 */
public void PickCoin(string  asEntity, string  asType)
{
	SetSwingDoorClosed("cellar_wood01_7", true, false);
	AddPropImpulse("wood_beam_1", 1.7f, 0, 0, "World");
	AddPropImpulse("wood_beam_2", -1.7f, 0, 0, "World");
	
	SetEntityActive("AreaGiveSanity", true);
	
	AddTimer("activaterocks", 1.0f, "TimerActiveRocks");
	AddTimer("TimerRockEvents", 0.1f, "TimerRockEvents");
	
	AddGlobalVarInt("ChemJars",1);
	
	SetEntityActive("InsanityArea_2", true);
	
	SetEntityActive("AreaBlockDoor_1", false);
	SetEntityActive("AreaBlockDoor_2", false);
	SetEntityActive("AreaBlockDoor_3", false);
	
	SetPlayerCrouchDisabled(true);
	SetPlayerJumpDisabled(true);
	
	FadeLightTo("PointLight_31", 0,0,0,0, -1, 1);
	
	PlaySoundAtEntity("CaveInMonsterSound", "04_warn", "Player", 0, false);
}
public void TimerActiveRocks(string  asTimer)
{
	for(int i=1;i<=5;i++) SetEntityActive("rock_"+i, true);
}
public void TimerRockEvents(string  asTimer)
{	
	/*Configurables
	 */
	int iMaxEventStep = 16;		//How many steps there are in the switch event
	float fEventSpeed = 0.5f;	//The default time between steps in an event
	
	/*Helpers - Do not edit
	 */
	string sEvent = asTimer;	//Using first timer name for variable, timer name & callback for the timer that loops
	AddLocalVarInt(sEvent, 1);  //What step to play in the event

	Debug.Log(sEvent + " local variable is " + GetLocalVarInt(sEvent));
	switch(GetLocalVarInt(sEvent)){
		case 1:
			PlayMusic("05_event_falling.ogg", false, 0.7f, 0, 10, false);
			PlaySoundAtEntity("wind", "01_whirl", "Player", 2.0f, false);
			PlaySoundAtEntity("rumble", "general_rock_rumble", "Player", 1.0f, false);
			StartScreenShake(0.1f, 4, 0.75f,2.25f);
			CreateParticleSystemAtEntity("windPS", "ps_dust_whirl_large", "Player", false);
			SetPlayerMoveSpeedMul(0.5f);
			SetPlayerRunSpeedMul(0);
			SetPlayerLookSpeedMul(0.8f);
		break;
		case 2:
			SetPlayerMoveSpeedMul(0.25f);
			SetPlayerLookSpeedMul(0.6f);
			
		break;
		case 3:
			SetLampLit("candlestick02_1", false, true);
			SetLampLit("torch_static01_1", false, true);
			
			//FadeLightTo("PointLight_21", 0, 0, 0, 0, 0, 12);
			SetPlayerMoveSpeedMul(0.1f);
			SetPlayerLookSpeedMul(0.4f);
			PlaySoundAtEntity("breakrock", "05_rock_fall", "cellar_wood01_7", 0, false);
			PlaySoundAtEntity("breakrock2", "05_rock_fall_2", "cellar_wood01_7", 0, false);
			CreateParticleSystemAtEntity("rockPS", "ps_break_mansionbase_wall", "cellar_wood01_7", false);
			StartPlayerLookAt("cellar_wood01_7", 5, 5, "");
		break;
		case 4:
			StopSound("wind", 4);
			StopSound("rumble", 4);
			SetPlayerLookSpeedMul(0.2f);
			PlaySoundAtEntity("player_breath", "react_breath", "Player", 0, false);	
		break;
		case 5:
			SetPlayerLookSpeedMul(0.1f);
			PlaySoundAtEntity("barrelroll", "roll_wood", "Barrel"+GetLocalVarString("WhatBarrel"), 0.1f, false);
		break;
		case 6:
			StartPlayerLookAt("Barrel"+GetLocalVarString("WhatBarrel"), 4, 4, "");
			AddPropImpulse("Barrel"+GetLocalVarString("WhatBarrel"), 0, 0, -2.5f, "World");
			StopSound("barrelroll", 1);
		break;
		case 7:
			PlaySoundAtEntity("player_shock", "react_scare", "Player", 0, false);
			GiveSanityDamage(10, false);
			fEventSpeed = 0.55f;
		break;
		case 8:
			PlaySoundAtEntity("player_hit", "player_falldamage_max", "Player", 0, false);
			AddPlayerHealth(-20);
			StopMusic(6, 0);
			StopSound("amb_sound1", 6);
			StopSound("amb_sound2", 6);
			FadeOut(1.0f);
			fEventSpeed = 0.15f;
			
			SetLampLit("torch_static01_3", false, true);
			SetLampLit("candlestick02_3", false, true);
		break;
		case 9:
			PlaySoundAtEntity("player_fall_to_ground", "player_bodyfall", "Player", 0, false);
			MovePlayerHeadPos(0, -1.1f, 0, 8, 0.5f);
			StartPlayerLookAt("candlestick02_1", 3, 3, "");
			FadePlayerRollTo(-60, 5, 5);
			
			
			SetLanternActive(false, false);
			SetLanternDisabled(true);
						
			for(int i=1;i<=3;i++) AddPropImpulse("BarrelAreaBlockDoor_"+i, 1, 2, 2, "World");
			fEventSpeed = 2.0f;
		break;
		case 10:
			SetPlayerActive(false);
			PlaySoundAtEntity("blackcloud", "05_cloud_swarm", "AreaBlockDoor_2", 24, false);
			PlaySoundAtEntity("creak", "00_creak", "AreaBlockDoor_2", 4, false);
			
			PlaySoundAtEntity("CaveInMonsterSound", "04_warn", "Player", 0, false);
			
			SetEntityInteractionDisabled("BarrelAreaBlockDoor_1", false);
			SetEntityInteractionDisabled("BarrelAreaBlockDoor_2", false);
			SetEntityInteractionDisabled("BarrelAreaBlockDoor_3", false);
			
			fEventSpeed = 4.0f;
		break;
		case 11:
			FadeSepiaColorTo(0.5f, 0.025f);
			FadeRadialBlurTo(0.1f, 0.025f);
			SetRadialBlurStartDist(0.2f);
			//SetLampLit("candlestick02_1", true, false);
			StopSound("blackcloud", 12);
			StopSound("creak", 6);
			
			for(int i=1;i<=3;i++) AddPropImpulse("BarrelAreaBlockDoor_"+i, 0, 0.1f, 0, "World");	//To fix hanging in mid air
			
			fEventSpeed = 3.0f;
		break;
		case 12:
			FadeIn(1.5f);
			SetPlayerActive(true);

			SetPlayerLookSpeedMul(0.2f);
			PlaySoundAtEntity("reactpant", "react_pant", "AreaBlockDoor_2", 0.5f, false);
			fEventSpeed = 3.0f;
		break;
		case 13:
			SetPlayerLookSpeedMul(0.4f);
			PlayMusic("04_amb", true, 1, 10, 0, true);
			PlaySoundAtEntity("amb_sound1", "05_amb1", "AreaAmb", 10, true);
			PlaySoundAtEntity("amb_sound2", "05_amb2", "AreaAmb", 10, true);
			PlaySoundAtEntity("player_get_up", "player_climb", "Player", 0, false);
			FadePlayerRollTo(0, 80, 80);
			MovePlayerHeadPos(0, 0, 0, 1, 0.5f);
			FadeSepiaColorTo(0, 0.01f);
			FadeRadialBlurTo(0, 0.01f);
			SetPlayerMoveSpeedMul(0.4f);
			SetPlayerRunSpeedMul(0.4f);
			
			SetLanternDisabled(false);	
			
			SetPlayerCrouchDisabled(false);
			SetPlayerJumpDisabled(false);
			
			fEventSpeed = 1.0f;
		break;
		case 14:
			SetPlayerLookSpeedMul(0.6f);
			SetPlayerMoveSpeedMul(0.6f);
			SetPlayerRunSpeedMul(0.6f);
			fEventSpeed = 1.0f;
		break;
		case 15:
			StopPlayerLookAt();
			SetPlayerLookSpeedMul(0.8f);
			SetPlayerMoveSpeedMul(0.8f);
			SetPlayerRunSpeedMul(0.8f);
			fEventSpeed = 1.0f;
		break;
		case 16:
			SetPlayerLookSpeedMul(1.0f);
			SetPlayerMoveSpeedMul(1.0f);
			SetPlayerRunSpeedMul(1.0f);
		break;
	}
	
	if(GetLocalVarInt(sEvent) <= iMaxEventStep) AddTimer(sEvent, fEventSpeed, sEvent);
}
/*The player can attach a ladder to get up on the loft
 */
public void CollideStickLadder(string  asArea, string  asBody)
{
	return; //Disable event as you can't crouch on ladder any longer.
	
	SetEntityActive("LadderArea_1", true);
}
/*When on the loft begin a strange event with light
 */
public void CollideLoft(string  asParent, string  asChild, int alState)
{
	return; //Disable event as you can't crouch on ladder any longer.
	
	AddTimer("L1", 1, "TimerLoft");
	
	StartScreenShake(0.01f, 6, 1.5f,2.5f);
	PlaySoundAtEntity("cloud", "05_cloud", "Player", 6, false);
	PlaySoundAtEntity("orb", "03_orb_loop", "AreaLoftEnd", 6, false);
	
	/*DEBUG
	 */
	AddDebugMessage("Collide "+asChild, false);
}
public void TimerLoft(string  asTimer)
{	
	if(asTimer == "L1"){
		FadeRadialBlurTo(0.1f, 0.025f);
		SetRadialBlurStartDist(0.2f);
		FadeLightTo("LoftLight_1", -1, -1, -1, -1, 7, 1.25f);
		AddTimer("L2", 1, "TimerLoft");
	}
	else if(asTimer == "L2"){
		FadeLightTo("LoftLight_2", -1, -1, -1, -1, 7, 1.25f);
		FadeLightTo("LoftLight_1", -1, -1, -1, -1, 0,8);
		AddTimer("L3", 1, "TimerLoft");
	}
	else if(asTimer == "L3"){
		FadeLightTo("LoftLight_3", -1, -1, -1, -1, 7, 1.25f);
		FadeLightTo("LoftLight_2", -1, -1, -1, -1, 0, 6);
		AddTimer("L4", 1, "TimerLoft");
	}
	else if(asTimer == "L4"){
		StopSound("cloud", 6);
		StopSound("orb", 8);
		FadeLightTo("LoftLight_4", -1, -1, -1, -1, 7, 1.25f);
		FadeLightTo("LoftLight_3", -1, -1, -1, -1, 0, 8);
		AddTimer("L5", 1, "TimerLoft");
	}
	else if(asTimer == "L5"){
		FadeLightTo("LoftLight_4", -1, -1, -1, -1, 0, 8);
		FadeRadialBlurTo(0, 0.04f);

		CreateParticleSystemAtEntity("bottlePS", "ps_ghost_release", "AreaLoftEnd", false);
		PlaySoundAtEntity("bottle", "03_in_a_bottle", "AreaLoftEnd", 0, false);
	}
	
	/*DEBUG
	 */
	AddDebugMessage("Loft Timer "+asTimer, false);
}
/*Give a hint about push force when touching debris
 */
public void GiveHintPush(string  asEntity)
{
	GiveHint("PushHint", "Hints", "PushHint", 0);
}
public void CollideGiveSanity(string  asParent, string  asChild, int alState)
{
	GiveSanityBoostSmall();
}
//END DEBRIS FALL DOWN AND LOCK PLAYER IN ROOM//
////////////////////////////////////////////////


public void PickChemical(string  asEntity, string  asType)
{
	//Turn off a light used to make the chemical more visible.
	if(asEntity == "Chemical_1") FadeLightTo("PointLight_32", 0,0,0,0, -1, 2);
	if(asEntity == "Chemical_3") FadeLightTo("PointLight_33", 0,0,0,0, -1, 2);
	if(asEntity == "Chemical_4") FadeLightTo("PointLight_34", 0,0,0,0, -1, 2);
	
	
	if(asEntity == "Chemical_3")
	{
		//if(!GetSwingDoorClosed("cellar_wood01_1"))
			//PlaySoundAtEntity("SlamDoor","scare_slam_door.snt", "cellar_wood01_1", 0, false);
		
		//SetSwingDoorClosed("cellar_wood01_1", true, true);
		
		if(!GetSwingDoorClosed("cellar_wood01_1")){
			PlaySoundAtEntity("SlamDoor","joint_door_move.snt", "cellar_wood01_1", 0.5f, false);
			
			AddTimer("doorforce1", 0.2f, "TimerDoorForce");
			AddTimer("doorforce2", 0.4f, "TimerDoorForce");
			AddTimer("doorforce3", 0.6f, "TimerDoorForce");
			AddTimer("doorforce4", 0.8f, "TimerDoorForce");
			AddTimer("doorforce5", 1.0f, "TimerDoorForce");
		}
		
		PlaySoundAtEntity("gruntgruff", "amb_idle", "AreaGruntSound1", 0.0f, false);
	
		AddTimer("sanity", 1, "TimerSanity");

		PlaySoundAtEntity("PlayerScare","scare_tingeling.snt", "Player", 0, false);
	}
	
	AddGlobalVarInt("ChemJars",1);
	
	CompleteQuest("04ChemicalsMoved", "04ChemicalsMoved");
	
	if(GetGlobalVarInt("ChemJars") == 4){
		PlayMusic("02_puzzle.ogg", false, 0.7f, 0, 10, false);
		GiveSanityBoost();
	}
}
public void TimerDoorForce(string  asTimer)
{
	AddPropForce("cellar_wood01_1", 200.0f, 0.0f, 200.0f, "World");

	AddDebugMessage("Hey!", false);
	if(asTimer == "doorforce5")
		StopSound("SlamDoor", 1.0f);
}
public void PickNoteTrans(string  asEntity, string  asType)
{
	PlayMusic("05_paper_transformation.ogg", false, 0.7f, 0, 10, false);
}


////////////////////////////////////////////////////////////
//BEGIN CHAINS AND HANGING CORPSES(CARCASS AS PLACEHOLDERS//
/*Create a blood drip particle and play a blood drip sound a bit later
 */
public void TimerBloodDrop(string  asTimer)
{
	if(asTimer == "AreaEndBlood_1" || asTimer == "AreaEndBlood_2" || asTimer == "AreaEndBlood_3" ){
		PlaySoundAtEntity("blooddrop", "general_blood_drop", asTimer, 0, false);
		AddTimer("AreaBeginBlood_"+GetLocalVarInt("Blood"), 0.1f, "TimerBloodDrop");
	}
	else{
		CreateParticleSystemAtEntity("PSblood", "ps_blood_drop.ps", asTimer, false);
		AddTimer("AreaEndBlood_"+GetLocalVarInt("Blood"), 2.5f, "TimerBloodDrop");
	}

	int iBlood = RandFloat(1,3);
	
	SetLocalVarInt("Blood", iBlood);
}
/*Give the chains a bit of a push to make them swagel in air
*/
public void TimerChain(string  asTimer)
{	
	for(int i=1;i<=6;i++) 
		AddPropForce("invisible_body_"+i, RandFloat(100,300), 0, RandFloat(100,300), "Local");
		
	AddTimer(asTimer, 5, asTimer);
}
/*An area to activate the lookat area in ceiling
 */
public void CollideActiveCeiling(string  asParent, string  asChild, int alState)
{
	if(alState == 1) SetEntityActive("AreaCeiling", true);
	else if(alState == -1) SetEntityActive("AreaCeiling", false);
}
/*When looking at ceiling and seeing corpses, loose sanity
 */
public void LookAtCeiling(string  entity, int alState) 
{
	if(alState == 1){
		FadeSepiaColorTo(0.75f, 0.3f);
		GiveSanityDamage(10, false);
		PlaySoundAtEntity("whine", "05_whine", "Player", 4, false);
	} 
	else if(alState == -1){
		FadeSepiaColorTo(0, 0.3f);
		StopSound("whine", 4);
		SetEntityActive("AreaCeiling", false);
		SetEntityActive("AreaCeilingActivate", false);
	} 
}
//END CHAINS AND HANGING CORPSES(CARCASS AS PLACEHOLDERS//
//////////////////////////////////////////////////////////


//////////////////////////////////////
//BEGIN EVENT WITH BOTTLES AND STEPS//
public void CollideAreaRemoveDoorEvent(string  asParent, string  asChild, int alState)
{
	SetEntityPlayerInteractCallback("cellar_wood01_3", "", false);
}
/*Interact door and rumble is played behind it
 */
public void InteractDoorWithGruntBehind(string  asEntity)
{
	if(GetLocalVarInt("HeardRumble") >= 1){
		SetEntityActive("AreaOtherEscape", true);
		SetEntityActive("AreaHoleEvents", false);
		AddEntityCollideCallback("Player", "AreaOtherEscape", "CollideAreaOtherEscape", true, 1);
	}
	
	AddLocalVarInt("HeardRumble", 1);
	
	PlaySoundAtEntity("gruntmumble", "amb_idle", "AreaBottleEvent", 0, false);
	PlaySoundAtEntity("bottlevent", "05_event_bottles", "AreaBottleEvent", 0, false);
	AddTimer("sanity", 0.75f, "TimerGiveSanityDamage");
}
/*When going the other way, some rumble for the room on other side
 */
public void InteractDoorToHole(string  asEntity)
{
	AddLocalVarInt("HeardRumble", 1);
	
	if(GetLocalVarInt("HeardRumble") <= 1) return;
	
	PlaySoundAtEntity("steps1", "scare_wood_creak_walk", "HoleParticle", 0, false);
	
	AddTimer("step1", 0.3f, "TimerPlaySteps");
	AddTimer("step2", 0.6f, "TimerPlaySteps");
	AddTimer("step3", 0.9f, "TimerPlaySteps");
}
public void TimerPlaySteps(string  asTimer)
{	
	if(asTimer == "step1") SetEntityActive("wood_box01_19", true);
	else if(asTimer == "step2") SetEntityActive("wood_box01_20", true);
	else SetEntityActive("wood_box01_21", true);
}
/*As looking in the room with a hole in the ceiling, debris fall down
 */
public void CollideHoleEvents(string  asParent, string  asChild, int alState)
{	
	AddLocalVarInt("HeardRumble", 1);
	
	if(GetLocalVarInt("HeardRumble") <= 2) return;
	
	for(int i=1;i<=3;i++) SetEntityActive("rock_small_"+i, true);
	CreateParticleSystemAtEntity("PShole", "ps_dust_falling_hole.ps", "HoleParticle", false);
	PlaySoundAtEntity("scratches", "05_wall_scratch", "HoleParticle", 0, false);
	
	SetEntityPlayerInteractCallback("cellar_wood01_3", "", true);
	
	AddTimer("sanity", 1.5f, "TimerGiveSanityDamage");
}
public void TimerGiveSanityDamage(string  asTimer)
{	
	GiveSanityDamage(10, true);
}
//Alternate escape route
public void CollideAreaOtherEscape(string  asParent, string  asChild, int alState)
{
	RemoveEntityCollideCallback("AreaUnlockDoor", "BarrelBlock");
	
	SetSwingDoorClosed("cellar_wood01_6", false, false);
	SetSwingDoorDisableAutoClose("cellar_wood01_6", true);
	
	SetSwingDoorLocked("cellar_wood01_3", false, false);
	SetSwingDoorDisableAutoClose("cellar_wood01_3", true);
	SetSwingDoorClosed("cellar_wood01_3", false, false);
	
	AddTimer("cellar_wood01_3", 0.1f, "TimerPushTheDumbDoor02"); AddTimer("cellar_wood01_3", 0.2f, "TimerPushTheDumbDoor02");
	
	AddPropImpulse("cellar_wood01_6", 2.0f, 0, 0, "World");
	AddPropImpulse("BarrelBlock", 0.0f, 16.0f, -16.0f, "World");
	
	PlaySoundAtEntity("closedoorsound", "close_door.snt", "cellar_wood01_6", 0, false);	
}
public void TimerPushTheDumbDoor02(string  asTimer)
{
	AddPropImpulse(asTimer, 0, 0, -2.0f, "World");
	AddDebugMessage("Push door!", false);
}
//END EVENT WITH BOTTLES AND STEPS//
////////////////////////////////////


//////////////////////////////
//BEGIN BANG AND PAIN SOUNDS//
public void CollideBangDoor(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("bangs", "05_event_door_bang", "cellar_wood01_5", 1, false);
	PlaySoundAtEntity("whineaa", "scare_whine_loop", "Player", 0.5f, false);
		
	FadeSepiaColorTo(0.75f, 0.3f);
	FadeImageTrailTo(1.5f, 0.5f);
	
	AddTimer("bang1", 0.6f, "TimerBangDoor");
	AddTimer("bang2", 1.6f, "TimerBangDoor");
	AddTimer("bang3", 2.5f, "TimerBangDoor");
	AddTimer("bang4", 3.0f, "TimerBangDoor");
}
public void TimerBangDoor(string  asTimer)
{	
	if(asTimer == "bang1"){ 
		AddPropImpulse("cellar_wood01_5", 0, 0, -5, "World");
		PlaySoundAtEntity("scare", "react_scare", "Player", 0.25f, false);
		CreateParticleSystemAtEntity("bang1", "ps_dust_impact_vert.ps", "cellar_wood01_5", false);
		StartScreenShake(0.008f, 0.5f, 0.1f,0.3f);
		GiveSanityDamage(10, true);
	}
	else if(asTimer == "bang2") { 
		AddPropImpulse("cellar_wood01_5", 0, 0, -5, "World");
		CreateParticleSystemAtEntity("bang1", "ps_dust_impact_vert.ps", "cellar_wood01_5", false);
		StartScreenShake(0.008f, 0.5f, 0.1f,0.3f);
	}
	else if(asTimer == "bang3") { 
		AddPropImpulse("cellar_wood01_5", 0, 0, -5, "World");
		PlaySoundAtEntity("scare", "react_breath", "Player", 0.5f, false);
		CreateParticleSystemAtEntity("bang1", "ps_dust_impact_vert.ps", "cellar_wood01_5", false);
		StartScreenShake(0.008f, 0.5f, 0.1f,0.3f);
	}
	else {
		SetSwingDoorLocked("cellar_wood01_5", false, false);
		FadeSepiaColorTo(0, 0.3f);
		FadeImageTrailTo(0, 0.1f);
		StopSound("whineaa", 4);
	}
}
//END BANG AND PAIN SOUNDS//
////////////////////////////


/////////////////////
//BEGIN ENEMY EVENT//
public void PickEnemy(string  asEntity, string  asType)
{
	if(GetLocalVarInt("EnemyEventDone") == 1) return;
	if(GetGlobalVarInt("IGF")==1) return;
	
	SetEntityActive("AreaOtherEscape", false);
	
	AddEntityCollideCallback("Player", "AreaEnemyEvent_2", "CollideActivateEnemy", true, 1);
	
	SetSwingDoorDisableAutoClose("cellar_wood01_4", true);
	SetSwingDoorClosed("cellar_wood01_4", false, false);
	
	AddPropImpulse("cellar_wood01_4", 2.0f, 0, 0, "World");
	
	PlaySoundAtEntity("monster", "grunt/amb_idle", "torch_static01_7", 0.25f, false);
	
	SetEntityActive("grunt_normal_1", true);

	AddTimer("enemy1", 0.2f, "TimerEnemy");

	SetEntityPlayerLookAtCallback("AreaHoleEvents", "LookAtEnemy", true);

	SetPlayerRunSpeedMul(0.6f);
	SetPlayerMoveSpeedMul(0.8f);
	
	SetLocalVarInt("EnemyEventDone", 1);
}
public void LookAtEnemy(string  asEntity, int alState)
{
	RemoveEntityCollideCallback("Player", "AreaEnemyEvent_2");
	
	AddTimer("enemy2", 1.5f, "TimerEnemy");
	AddTimer("enemy3", 5, "TimerEnemy");
	AddTimer("enemy4", 10, "TimerEnemy");
	AddEnemyPatrolNode("grunt_normal_1", "PathNodeArea_3", 0, "");
	
	SetEntityPlayerLookAtCallback("AreaHoleEvents", "", true);
}
public void CollideActivateEnemy(string  asParent, string  asChild, int alState)
{
	SetEntityPlayerLookAtCallback("AreaHoleEvents", "", true);
	
	AddTimer("enemy2", 1.5f, "TimerEnemy");
	AddTimer("enemy3", 5, "TimerEnemy");
	AddTimer("enemy4", 10, "TimerEnemy");
	AddEnemyPatrolNode("grunt_normal_1", "PathNodeArea_3", 0, "");
}

public void TimerEnemy(string  asTimer)
{
	if(asTimer == "speed"){
		SetPlayerRunSpeedMul(1);
		SetPlayerMoveSpeedMul(1);
		//FadePlayerFOVMulTo(1, 0.01f);
		//FadeRadialBlurTo(0, 0.1f);
	}
	else if(asTimer == "enemy1"){
		PlayMusic("05_event_steps.ogg", false, 0.8f, 1, 10, false);
		PlaySoundAtEntity("scare", "react_scare", "Player", 0.5f, false);
		//StartPlayerLookAt("grunt_normal_1", 5, 5, "");
		//FadePlayerFOVMulTo(2, 0.01f);
		//FadeRadialBlurTo(0.1f, 0.025f);
		//SetRadialBlurStartDist(0.2f);
		SetPlayerRunSpeedMul(0.4f);
		SetPlayerMoveSpeedMul(0.6f);
	}
	else if(asTimer == "enemy2"){
		//StopPlayerLookAt();
		PlaySoundAtEntity("react", "react_breath", "Player", 0.6f, false);	
		SetPlayerRunSpeedMul(0.2f);
		SetPlayerMoveSpeedMul(0.4f);
	}
	else if(asTimer == "enemy3"){
		SetPlayerRunSpeedMul(0.4f);
		SetPlayerMoveSpeedMul(0.6f);
	}
	else{
		SetEntityActive("AreaEnemyEvent_3", false);	//If player throw box at enemy, enemy will chase player until player leaves level
		SetPlayerRunSpeedMul(1);
		SetPlayerMoveSpeedMul(1);
		//FadePlayerFOVMulTo(1, 0.01f);
		//FadeRadialBlurTo(0, 0.1f);
	}
}
public void CollideDeactivateEnemy(string  asParent, string  asChild, int alState)
{
	SetPlayerRunSpeedMul(0.6f);
	SetPlayerMoveSpeedMul(0.8f);
	
	SetEntityActive("grunt_normal_1", false);
	RemoveEntityCollideCallback("AreaUnlockDoor", "BarrelBlock");
	
	SetSwingDoorClosed("cellar_wood01_4", true, false);
	PlaySoundAtEntity("cellar_wood01_4slam", "scare_slam_door", "cellar_wood01_4", 0.0f, false);
	GiveSanityDamage(10, false);
	
	SetSwingDoorClosed("cellar_wood01_6", false, false);
	SetSwingDoorDisableAutoClose("cellar_wood01_6", true);
	
	SetSwingDoorLocked("cellar_wood01_3", false, false);
	SetSwingDoorDisableAutoClose("cellar_wood01_3", true);
	SetSwingDoorClosed("cellar_wood01_3", false, false);
	
	AddTimer("speed", 2, "TimerEnemy");
	
	RemoveTimer("enemy4");
	
	AddTimer("cellar_wood01_3", 0.1f, "TimerPushTheDumbDoor"); AddTimer("cellar_wood01_3", 0.2f, "TimerPushTheDumbDoor");
	
	AddPropImpulse("cellar_wood01_6", 2.0f, 0, 0, "World");
	AddPropImpulse("BarrelBlock", 0.0f, 16.0f, -16.0f, "World");
	
	PlaySoundAtEntity("closedoorsound", "close_door.snt", "cellar_wood01_6", 0, false);	
	PlaySoundAtEntity("react2", "react_breath", "Player", 0.8f, false);
}
public void TimerPushTheDumbDoor(string  asTimer)
{
	AddPropImpulse(asTimer, 0, 0, -2.0f, "World");
	AddDebugMessage("Push door!", false);
}
public void CollideEnemyHint(string  asParent, string  asChild, int alState)
{
	GiveHint("EnemyTip01", "Hints", "EnemyTip01", 0);
}

//END ENEMY EVENT//
///////////////////


//BEING MISC//
public void InteractTouchIt(string  asEntity)
{
	PlayGuiSound("impact_organic_low2.ogg", 0.5f);
	
	//SetMessage("Ch01Level05", "TouchIt", 0);
	
	AddTimer(asEntity, 1.0f, "TimerTouchIt");
}
public void TimerTouchIt(string  asTimer)
{
	SetEntityPlayerInteractCallback(asTimer, "InteractTouchIt", true);
}
//END MISC//

////////////////////////////
// Run first time starting map
public override void OnStart()
{
	SetMapDisplayNameEntry("WineCellar");
	
	//----COLLIDE CALLBACKS----//
	AddEntityCollideCallback("Player", "AreaRemoveDoorEvent", "CollideAreaRemoveDoorEvent", false, 1);
	AddEntityCollideCallback("AreaUnlockDoor", "BarrelBlock", "FuncUnlockDoor", true, -1);	//Barrel blocking door, move it out area to unlock door
	AddEntityCollideCallback("Player", "AreaBeginMoan", "FuncMoanHorrors", true, 1);	//Big area in first room to trigger moans
	for(int i=1;i<=3;i++) AddEntityCollideCallback("Player", "AreaBlockDoor_"+i, "CollideBlockDoor", false, 1);	//Three areas for barrel selection 
	AddEntityCollideCallback("Player", "AreaLoft", "CollideLoft", true, 1);	//Trigger event on loft
	AddEntityCollideCallback("Player", "AreaCeilingActivate", "CollideActiveCeiling", false, 0);	//Activate lookat area for corpses in ceiling
	AddEntityCollideCallback("Player", "AreaHoleEvents", "CollideHoleEvents", true, 1);	//Debris and particles from ceiling hole
	AddEntityCollideCallback("Player", "AreaBangDoor", "CollideBangDoor", true, 1);	//Begin the door bang and pain event
	AddEntityCollideCallback("Player", "AreaEnemyHint", "CollideEnemyHint", false, 1);	//Display hint about enemies
	AddEntityCollideCallback("Player", "AreaGiveSanity", "CollideGiveSanity", true, 1);	//Display hint about enemies

	AddEntityCollideCallback("grunt_normal_1", "AreaEnemyEvent_3", "CollideDeactivateEnemy", true, 1);	//Begin the door bang and pain event
	
	SetEntityPlayerLookAtCallback("AreaCeiling", "LookAtCeiling", false);
	SetEntityPlayerInteractCallback("cellar_wood01_3", "InteractDoorWithGruntBehind", true);
	SetEntityPlayerInteractCallback("cellar_wood01_4", "InteractDoorToHole", true);
	
	// ENTITY INIT
	SetEntityInteractionDisabled("BarrelAreaBlockDoor_1", true);
	SetEntityInteractionDisabled("BarrelAreaBlockDoor_2", true);
	SetEntityInteractionDisabled("BarrelAreaBlockDoor_3", true);
	
	//---- ENEMY INIT ----//
	
	
	
	//----SOUND INIT----//
	PlaySoundAtEntity("amb_sound1", "05_amb1", "AreaAmb", 10, true);	//So that the sound can be faded out/in
	PlaySoundAtEntity("amb_sound2", "05_amb2", "AreaAmb", 10, true);	//So that the sound can be faded out/in

															
	//----ROPE CREATION----//
	/*Hanging chains in room with corpses in ceiling
	 */
	/*for(int i=1;i<=6;i++) CreateRope("Rope0"+i, "RopeStart_"+i, "RopeEnd_"+i, "", "invisible_body
	 * 
	 * 
	 * _"+i+"_Body_1", //StartBody, EndBody
		0.5f, 4.5f, 0.2f, 0.001f, 100, 1, 					//Min total length, Max total length, Segment size, Damping, Strength, Stiffness
		"textures/ropes/chain.mat", 0.025f, 0.5f, 0.5f, //Material, Radius, LengthTileAmount, LengthTileSize
		"",	0.49f, 0.45f,									//Sound, Sound start speed, Sound stop speed
		true, 5, 3 										//Automove, Automove acc, Automove max speed
		);*/
	
	//---- TIMER INIT ----//
	AddTimer("TimerChain", 1, "TimerChain");	//Push chains to make them swagel a bit
	AddTimer("AreaBeginBlood_1", 0.1f, "TimerBloodDrop");	//Drip blood from corpses
	AddTimer("creak", 1, "CreakTimer");		//Creaking from the floor above
	
	//----QUEST INIT----//
	SetNumberOfQuestsInMap(0);
	
	//----VARIABLES ----//
	SetLocalVarString("WhatBarrel", "AreaBlockDoor_2");	//A default barrel selected for debris event, incase player does not touch an area
	SetGlobalVarInt("PlayerBeenInLevel05",1);
	
	//---- FOG SETUP ----//
	SetFogActive(true);
	SetFogColor(0.15f, 0.16f, 0.18f, 1);
	SetFogProperties(6, 22, 1, false);
	
	//Remove the dakrness hint, so the player can get it once more!
	RemoveHint("DarknessDecrease");
	
	/* TODO: This stays commented just in case it is needed in the future (read comment below)
	// Disable interaction with barrels, just in case the player tries to bring em down before triggering the event (Luis: I managed to do this and we really dont want the player to watch in fear a barrel lying still on the floor :P)
	SetEntityInteractionDisabled("BarrelAreaBlockDoor_1", true);
	SetEntityInteractionDisabled("BarrelAreaBlockDoor_2", true);
	SetEntityInteractionDisabled("BarrelAreaBlockDoor_3", true);
	*/
	 
	if(ScriptDebugOn())
	{
		GiveItemFromFile("lantern", "lantern.ent");
		for(int i=0;i<10;i++) GiveItemFromFile("tinderbox_"+i, "tinderbox.ent");
		
		//SetEntityActive("FlashbackArea_1", false);
	}
}


////////////////////////////
// Run when entering map
public override void OnEnter()
{
    Finder.Bufferize("cellar_wood01_3", "UnlockDoor!", "moanTimer", "FuncMoanTimer", "stepTimer", "moanSoundx", "Player", "sanity", "TimerSanity", "Begin moaning sounds", "moanSound", "HorrorMoan_0", "HorrorMoan_1", "HorrorMoan_2", "HorrorMoan_3", "HorrorMoan_4", "HorrorMoan_5", "HorrorMoan_6", "HorrorMoan_7", "HorrorMoan_8", "HorrorMoan_9", "HorrorMoan_10", "HorrorMoan_11", "HorrorMoan_12", "HorrorMoan_13", "HorrorMoan_14", "HorrorMoan_15", "HorrorMoan_16", "HorrorMoan_17", "HorrorMoan_18", "HorrorMoan_19", "HorrorMoan_20", "HorrorMoan_21", "HorrorMoan_22", "HorrorMoan_23", "HorrorMoan_24", "HorrorMoan_25", "HorrorMoan_26", "HorrorMoan_27", "HorrorMoan_28", "HorrorMoan_29", "HorrorMoan_30", "HorrorMoan_31", "HorrorMoan_32", "HorrorMoan_33", "HorrorMoan_34", "HorrorMoan_35", "HorrorMoan_36", "HorrorMoan_37", "HorrorMoan_38", "HorrorMoan_39", "HorrorMoan_40", "HorrorMoan_41", "HorrorMoan_42", "HorrorMoan_43", "HorrorMoan_44", "HorrorMoan_45", "HorrorMoan_46", "HorrorMoan_47", "HorrorMoan_48", "HorrorMoan_49", "HorrorMoan_50", "HorrorMoan_51", "HorrorMoan_52", "HorrorMoan_53", "HorrorMoan_54", "HorrorMoan_55", "HorrorMoan_56", "HorrorMoan_57", "HorrorMoan_58", "HorrorMoan_59", "HorrorMoan_60", "HorrorMoan_61", "HorrorMoan_62", "HorrorMoan_63", "HorrorMoan_64", "HorrorMoan_65", "HorrorMoan_66", "HorrorMoan_67", "HorrorMoan_68", "HorrorMoan_69", "HorrorMoan_70", "HorrorMoan_71", "HorrorMoan_72", "HorrorMoan_73", "HorrorMoan_74", "HorrorMoan_75", "HorrorMoan_76", "HorrorMoan_77", "HorrorMoan_78", "HorrorMoan_79", "HorrorMoan_80", "HorrorMoan_81", "HorrorMoan_82", "HorrorMoan_83", "HorrorMoan_84", "HorrorMoan_85", "HorrorMoan_86", "HorrorMoan_87", "HorrorMoan_88", "HorrorMoan_89", "HorrorMoan_90", "HorrorMoan_91", "HorrorMoan_92", "HorrorMoan_93", "HorrorMoan_94", "HorrorMoan_95", "HorrorMoan_96", "HorrorMoan_97", "HorrorMoan_98", "HorrorMoan_99", "stepSound", "Now moaning in area: ", "creakSound", "scare_wood_creak_mix", "HorrorCreak_0", "HorrorCreak_1", "HorrorCreak_2", "HorrorCreak_3", "HorrorCreak_4", "HorrorCreak_5", "HorrorCreak_6", "HorrorCreak_7", "HorrorCreak_8", "HorrorCreak_9", "HorrorCreak_10", "HorrorCreak_11", "HorrorCreak_12", "HorrorCreak_13", "HorrorCreak_14", "HorrorCreak_15", "HorrorCreak_16", "HorrorCreak_17", "HorrorCreak_18", "HorrorCreak_19", "HorrorCreak_20", "HorrorCreak_21", "HorrorCreak_22", "HorrorCreak_23", "HorrorCreak_24", "HorrorCreak_25", "HorrorCreak_26", "HorrorCreak_27", "HorrorCreak_28", "HorrorCreak_29", "HorrorCreak_30", "HorrorCreak_31", "HorrorCreak_32", "HorrorCreak_33", "HorrorCreak_34", "HorrorCreak_35", "HorrorCreak_36", "HorrorCreak_37", "HorrorCreak_38", "HorrorCreak_39", "HorrorCreak_40", "HorrorCreak_41", "HorrorCreak_42", "HorrorCreak_43", "HorrorCreak_44", "HorrorCreak_45", "HorrorCreak_46", "HorrorCreak_47", "HorrorCreak_48", "HorrorCreak_49", "HorrorCreak_50", "HorrorCreak_51", "HorrorCreak_52", "HorrorCreak_53", "HorrorCreak_54", "HorrorCreak_55", "HorrorCreak_56", "HorrorCreak_57", "HorrorCreak_58", "HorrorCreak_59", "HorrorCreak_60", "HorrorCreak_61", "HorrorCreak_62", "HorrorCreak_63", "HorrorCreak_64", "HorrorCreak_65", "HorrorCreak_66", "HorrorCreak_67", "HorrorCreak_68", "HorrorCreak_69", "HorrorCreak_70", "HorrorCreak_71", "HorrorCreak_72", "HorrorCreak_73", "HorrorCreak_74", "HorrorCreak_75", "HorrorCreak_76", "HorrorCreak_77", "HorrorCreak_78", "HorrorCreak_79", "HorrorCreak_80", "HorrorCreak_81", "HorrorCreak_82", "HorrorCreak_83", "HorrorCreak_84", "HorrorCreak_85", "HorrorCreak_86", "HorrorCreak_87", "HorrorCreak_88", "HorrorCreak_89", "HorrorCreak_90", "HorrorCreak_91", "HorrorCreak_92", "HorrorCreak_93", "HorrorCreak_94", "HorrorCreak_95", "HorrorCreak_96", "HorrorCreak_97", "HorrorCreak_98", "HorrorCreak_99", "creakPS", "ps_dust_falling_small", "creak", "CreakTimer", "Now creaking in area: ", "cellar_wood01_4", "World", "cellar_wood01_1", "SlamDoor", "gruntgruff", "amb_idle", "AreaGruntSound1", "PlayerScare", "WhatBarrel", "cellar_wood01_7", "wood_beam_1", "wood_beam_2", "AreaGiveSanity", "activaterocks", "TimerActiveRocks", "TimerRockEvents", "ChemJars", "InsanityArea_2", "AreaBlockDoor_1", "AreaBlockDoor_2", "AreaBlockDoor_3", "PointLight_31", "CaveInMonsterSound", "04_warn", "Rock_0", "Rock_1", "Rock_2", "Rock_3", "Rock_4", "Rock_5", "Rock_6", "Rock_7", "Rock_8", "Rock_9", "Rock_10", "Rock_11", "Rock_12", "Rock_13", "Rock_14", "Rock_15", "Rock_16", "Rock_17", "Rock_18", "Rock_19", "Rock_20", "Rock_21", "Rock_22", "Rock_23", "Rock_24", "Rock_25", "Rock_26", "Rock_27", "Rock_28", "Rock_29", "Rock_30", "Rock_31", "Rock_32", "Rock_33", "Rock_34", "Rock_35", "Rock_36", "Rock_37", "Rock_38", "Rock_39", "Rock_40", "Rock_41", "Rock_42", "Rock_43", "Rock_44", "Rock_45", "Rock_46", "Rock_47", "Rock_48", "Rock_49", "Rock_50", "Rock_51", "Rock_52", "Rock_53", "Rock_54", "Rock_55", "Rock_56", "Rock_57", "Rock_58", "Rock_59", "Rock_60", "Rock_61", "Rock_62", "Rock_63", "Rock_64", "Rock_65", "Rock_66", "Rock_67", "Rock_68", "Rock_69", "Rock_70", "Rock_71", "Rock_72", "Rock_73", "Rock_74", "Rock_75", "Rock_76", "Rock_77", "Rock_78", "Rock_79", "Rock_80", "Rock_81", "Rock_82", "Rock_83", "Rock_84", "Rock_85", "Rock_86", "Rock_87", "Rock_88", "Rock_89", "Rock_90", "Rock_91", "Rock_92", "Rock_93", "Rock_94", "Rock_95", "Rock_96", "Rock_97", "Rock_98", "Rock_99", "wind", "01_whirl", "rumble", "general_rock_rumble", "windPS", "ps_dust_whirl_large", "candlestick02_1", "torch_static01_1", "PointLight_21", "breakrock", "05_rock_fall", "breakrock2", "05_rock_fall_2", "rockPS", "ps_break_mansionbase_wall", "", "player_breath", "react_breath", "barrelroll", "roll_wood", "Barrel", "player_shock", "react_scare", "player_hit", "player_falldamage_max", "amb_sound1", "amb_sound2", "torch_static01_3", "candlestick02_3", "player_fall_to_ground", "player_bodyfall", "BarrelAreaBlockDoor_0", "BarrelAreaBlockDoor_1", "BarrelAreaBlockDoor_2", "BarrelAreaBlockDoor_3", "BarrelAreaBlockDoor_4", "BarrelAreaBlockDoor_5", "BarrelAreaBlockDoor_6", "BarrelAreaBlockDoor_7", "BarrelAreaBlockDoor_8", "BarrelAreaBlockDoor_9", "BarrelAreaBlockDoor_10", "BarrelAreaBlockDoor_11", "BarrelAreaBlockDoor_12", "BarrelAreaBlockDoor_13", "BarrelAreaBlockDoor_14", "BarrelAreaBlockDoor_15", "BarrelAreaBlockDoor_16", "BarrelAreaBlockDoor_17", "BarrelAreaBlockDoor_18", "BarrelAreaBlockDoor_19", "BarrelAreaBlockDoor_20", "BarrelAreaBlockDoor_21", "BarrelAreaBlockDoor_22", "BarrelAreaBlockDoor_23", "BarrelAreaBlockDoor_24", "BarrelAreaBlockDoor_25", "BarrelAreaBlockDoor_26", "BarrelAreaBlockDoor_27", "BarrelAreaBlockDoor_28", "BarrelAreaBlockDoor_29", "BarrelAreaBlockDoor_30", "BarrelAreaBlockDoor_31", "BarrelAreaBlockDoor_32", "BarrelAreaBlockDoor_33", "BarrelAreaBlockDoor_34", "BarrelAreaBlockDoor_35", "BarrelAreaBlockDoor_36", "BarrelAreaBlockDoor_37", "BarrelAreaBlockDoor_38", "BarrelAreaBlockDoor_39", "BarrelAreaBlockDoor_40", "BarrelAreaBlockDoor_41", "BarrelAreaBlockDoor_42", "BarrelAreaBlockDoor_43", "BarrelAreaBlockDoor_44", "BarrelAreaBlockDoor_45", "BarrelAreaBlockDoor_46", "BarrelAreaBlockDoor_47", "BarrelAreaBlockDoor_48", "BarrelAreaBlockDoor_49", "BarrelAreaBlockDoor_50", "BarrelAreaBlockDoor_51", "BarrelAreaBlockDoor_52", "BarrelAreaBlockDoor_53", "BarrelAreaBlockDoor_54", "BarrelAreaBlockDoor_55", "BarrelAreaBlockDoor_56", "BarrelAreaBlockDoor_57", "BarrelAreaBlockDoor_58", "BarrelAreaBlockDoor_59", "BarrelAreaBlockDoor_60", "BarrelAreaBlockDoor_61", "BarrelAreaBlockDoor_62", "BarrelAreaBlockDoor_63", "BarrelAreaBlockDoor_64", "BarrelAreaBlockDoor_65", "BarrelAreaBlockDoor_66", "BarrelAreaBlockDoor_67", "BarrelAreaBlockDoor_68", "BarrelAreaBlockDoor_69", "BarrelAreaBlockDoor_70", "BarrelAreaBlockDoor_71", "BarrelAreaBlockDoor_72", "BarrelAreaBlockDoor_73", "BarrelAreaBlockDoor_74", "BarrelAreaBlockDoor_75", "BarrelAreaBlockDoor_76", "BarrelAreaBlockDoor_77", "BarrelAreaBlockDoor_78", "BarrelAreaBlockDoor_79", "BarrelAreaBlockDoor_80", "BarrelAreaBlockDoor_81", "BarrelAreaBlockDoor_82", "BarrelAreaBlockDoor_83", "BarrelAreaBlockDoor_84", "BarrelAreaBlockDoor_85", "BarrelAreaBlockDoor_86", "BarrelAreaBlockDoor_87", "BarrelAreaBlockDoor_88", "BarrelAreaBlockDoor_89", "BarrelAreaBlockDoor_90", "BarrelAreaBlockDoor_91", "BarrelAreaBlockDoor_92", "BarrelAreaBlockDoor_93", "BarrelAreaBlockDoor_94", "BarrelAreaBlockDoor_95", "BarrelAreaBlockDoor_96", "BarrelAreaBlockDoor_97", "BarrelAreaBlockDoor_98", "BarrelAreaBlockDoor_99", "blackcloud", "05_cloud_swarm", "00_creak", "reactpant", "react_pant", "04_amb", "05_amb1", "AreaAmb", "05_amb2", "player_get_up", "player_climb", "LadderArea_1", "L1", "TimerLoft", "cloud", "05_cloud", "orb", "03_orb_loop", "AreaLoftEnd", "Collide ", "LoftLight_1", "L2", "LoftLight_2", "L3", "LoftLight_3", "L4", "LoftLight_4", "L5", "bottlePS", "ps_ghost_release", "bottle", "03_in_a_bottle", "Loft Timer ", "PushHint", "Hints", "Chemical_1", "PointLight_32", "Chemical_3", "PointLight_33", "Chemical_4", "PointLight_34", "Hey!", "doorforce1", "TimerDoorForce", "doorforce2", "doorforce3", "doorforce4", "doorforce5", "04ChemicalsMoved", "AreaEndBlood_1", "AreaEndBlood_2", "AreaEndBlood_3", "blooddrop", "general_blood_drop", "AreaBeginBlood_0", "AreaBeginBlood_1", "AreaBeginBlood_2", "AreaBeginBlood_3", "AreaBeginBlood_4", "AreaBeginBlood_5", "AreaBeginBlood_6", "AreaBeginBlood_7", "AreaBeginBlood_8", "AreaBeginBlood_9", "AreaBeginBlood_10", "AreaBeginBlood_11", "AreaBeginBlood_12", "AreaBeginBlood_13", "AreaBeginBlood_14", "AreaBeginBlood_15", "AreaBeginBlood_16", "AreaBeginBlood_17", "AreaBeginBlood_18", "AreaBeginBlood_19", "AreaBeginBlood_20", "AreaBeginBlood_21", "AreaBeginBlood_22", "AreaBeginBlood_23", "AreaBeginBlood_24", "AreaBeginBlood_25", "AreaBeginBlood_26", "AreaBeginBlood_27", "AreaBeginBlood_28", "AreaBeginBlood_29", "AreaBeginBlood_30", "AreaBeginBlood_31", "AreaBeginBlood_32", "AreaBeginBlood_33", "AreaBeginBlood_34", "AreaBeginBlood_35", "AreaBeginBlood_36", "AreaBeginBlood_37", "AreaBeginBlood_38", "AreaBeginBlood_39", "AreaBeginBlood_40", "AreaBeginBlood_41", "AreaBeginBlood_42", "AreaBeginBlood_43", "AreaBeginBlood_44", "AreaBeginBlood_45", "AreaBeginBlood_46", "AreaBeginBlood_47", "AreaBeginBlood_48", "AreaBeginBlood_49", "AreaBeginBlood_50", "AreaBeginBlood_51", "AreaBeginBlood_52", "AreaBeginBlood_53", "AreaBeginBlood_54", "AreaBeginBlood_55", "AreaBeginBlood_56", "AreaBeginBlood_57", "AreaBeginBlood_58", "AreaBeginBlood_59", "AreaBeginBlood_60", "AreaBeginBlood_61", "AreaBeginBlood_62", "AreaBeginBlood_63", "AreaBeginBlood_64", "AreaBeginBlood_65", "AreaBeginBlood_66", "AreaBeginBlood_67", "AreaBeginBlood_68", "AreaBeginBlood_69", "AreaBeginBlood_70", "AreaBeginBlood_71", "AreaBeginBlood_72", "AreaBeginBlood_73", "AreaBeginBlood_74", "AreaBeginBlood_75", "AreaBeginBlood_76", "AreaBeginBlood_77", "AreaBeginBlood_78", "AreaBeginBlood_79", "AreaBeginBlood_80", "AreaBeginBlood_81", "AreaBeginBlood_82", "AreaBeginBlood_83", "AreaBeginBlood_84", "AreaBeginBlood_85", "AreaBeginBlood_86", "AreaBeginBlood_87", "AreaBeginBlood_88", "AreaBeginBlood_89", "AreaBeginBlood_90", "AreaBeginBlood_91", "AreaBeginBlood_92", "AreaBeginBlood_93", "AreaBeginBlood_94", "AreaBeginBlood_95", "AreaBeginBlood_96", "AreaBeginBlood_97", "AreaBeginBlood_98", "AreaBeginBlood_99", "Blood", "TimerBloodDrop", "PSblood", "AreaEndBlood_0", "AreaEndBlood_4", "AreaEndBlood_5", "AreaEndBlood_6", "AreaEndBlood_7", "AreaEndBlood_8", "AreaEndBlood_9", "AreaEndBlood_10", "AreaEndBlood_11", "AreaEndBlood_12", "AreaEndBlood_13", "AreaEndBlood_14", "AreaEndBlood_15", "AreaEndBlood_16", "AreaEndBlood_17", "AreaEndBlood_18", "AreaEndBlood_19", "AreaEndBlood_20", "AreaEndBlood_21", "AreaEndBlood_22", "AreaEndBlood_23", "AreaEndBlood_24", "AreaEndBlood_25", "AreaEndBlood_26", "AreaEndBlood_27", "AreaEndBlood_28", "AreaEndBlood_29", "AreaEndBlood_30", "AreaEndBlood_31", "AreaEndBlood_32", "AreaEndBlood_33", "AreaEndBlood_34", "AreaEndBlood_35", "AreaEndBlood_36", "AreaEndBlood_37", "AreaEndBlood_38", "AreaEndBlood_39", "AreaEndBlood_40", "AreaEndBlood_41", "AreaEndBlood_42", "AreaEndBlood_43", "AreaEndBlood_44", "AreaEndBlood_45", "AreaEndBlood_46", "AreaEndBlood_47", "AreaEndBlood_48", "AreaEndBlood_49", "AreaEndBlood_50", "AreaEndBlood_51", "AreaEndBlood_52", "AreaEndBlood_53", "AreaEndBlood_54", "AreaEndBlood_55", "AreaEndBlood_56", "AreaEndBlood_57", "AreaEndBlood_58", "AreaEndBlood_59", "AreaEndBlood_60", "AreaEndBlood_61", "AreaEndBlood_62", "AreaEndBlood_63", "AreaEndBlood_64", "AreaEndBlood_65", "AreaEndBlood_66", "AreaEndBlood_67", "AreaEndBlood_68", "AreaEndBlood_69", "AreaEndBlood_70", "AreaEndBlood_71", "AreaEndBlood_72", "AreaEndBlood_73", "AreaEndBlood_74", "AreaEndBlood_75", "AreaEndBlood_76", "AreaEndBlood_77", "AreaEndBlood_78", "AreaEndBlood_79", "AreaEndBlood_80", "AreaEndBlood_81", "AreaEndBlood_82", "AreaEndBlood_83", "AreaEndBlood_84", "AreaEndBlood_85", "AreaEndBlood_86", "AreaEndBlood_87", "AreaEndBlood_88", "AreaEndBlood_89", "AreaEndBlood_90", "AreaEndBlood_91", "AreaEndBlood_92", "AreaEndBlood_93", "AreaEndBlood_94", "AreaEndBlood_95", "AreaEndBlood_96", "AreaEndBlood_97", "AreaEndBlood_98", "AreaEndBlood_99", "invisible_body_0", "invisible_body_1", "invisible_body_2", "invisible_body_3", "invisible_body_4", "invisible_body_5", "invisible_body_6", "invisible_body_7", "invisible_body_8", "invisible_body_9", "invisible_body_10", "invisible_body_11", "invisible_body_12", "invisible_body_13", "invisible_body_14", "invisible_body_15", "invisible_body_16", "invisible_body_17", "invisible_body_18", "invisible_body_19", "invisible_body_20", "invisible_body_21", "invisible_body_22", "invisible_body_23", "invisible_body_24", "invisible_body_25", "invisible_body_26", "invisible_body_27", "invisible_body_28", "invisible_body_29", "invisible_body_30", "invisible_body_31", "invisible_body_32", "invisible_body_33", "invisible_body_34", "invisible_body_35", "invisible_body_36", "invisible_body_37", "invisible_body_38", "invisible_body_39", "invisible_body_40", "invisible_body_41", "invisible_body_42", "invisible_body_43", "invisible_body_44", "invisible_body_45", "invisible_body_46", "invisible_body_47", "invisible_body_48", "invisible_body_49", "invisible_body_50", "invisible_body_51", "invisible_body_52", "invisible_body_53", "invisible_body_54", "invisible_body_55", "invisible_body_56", "invisible_body_57", "invisible_body_58", "invisible_body_59", "invisible_body_60", "invisible_body_61", "invisible_body_62", "invisible_body_63", "invisible_body_64", "invisible_body_65", "invisible_body_66", "invisible_body_67", "invisible_body_68", "invisible_body_69", "invisible_body_70", "invisible_body_71", "invisible_body_72", "invisible_body_73", "invisible_body_74", "invisible_body_75", "invisible_body_76", "invisible_body_77", "invisible_body_78", "invisible_body_79", "invisible_body_80", "invisible_body_81", "invisible_body_82", "invisible_body_83", "invisible_body_84", "invisible_body_85", "invisible_body_86", "invisible_body_87", "invisible_body_88", "invisible_body_89", "invisible_body_90", "invisible_body_91", "invisible_body_92", "invisible_body_93", "invisible_body_94", "invisible_body_95", "invisible_body_96", "invisible_body_97", "invisible_body_98", "invisible_body_99", "Local", "AreaCeiling", "whine", "05_whine", "AreaCeilingActivate", "HeardRumble", "AreaOtherEscape", "AreaHoleEvents", "CollideAreaOtherEscape", "gruntmumble", "AreaBottleEvent", "bottlevent", "05_event_bottles", "TimerGiveSanityDamage", "steps1", "scare_wood_creak_walk", "HoleParticle", "step1", "TimerPlaySteps", "step2", "step3", "wood_box01_19", "wood_box01_20", "wood_box01_21", "rock_small_0", "rock_small_1", "rock_small_2", "rock_small_3", "rock_small_4", "rock_small_5", "rock_small_6", "rock_small_7", "rock_small_8", "rock_small_9", "rock_small_10", "rock_small_11", "rock_small_12", "rock_small_13", "rock_small_14", "rock_small_15", "rock_small_16", "rock_small_17", "rock_small_18", "rock_small_19", "rock_small_20", "rock_small_21", "rock_small_22", "rock_small_23", "rock_small_24", "rock_small_25", "rock_small_26", "rock_small_27", "rock_small_28", "rock_small_29", "rock_small_30", "rock_small_31", "rock_small_32", "rock_small_33", "rock_small_34", "rock_small_35", "rock_small_36", "rock_small_37", "rock_small_38", "rock_small_39", "rock_small_40", "rock_small_41", "rock_small_42", "rock_small_43", "rock_small_44", "rock_small_45", "rock_small_46", "rock_small_47", "rock_small_48", "rock_small_49", "rock_small_50", "rock_small_51", "rock_small_52", "rock_small_53", "rock_small_54", "rock_small_55", "rock_small_56", "rock_small_57", "rock_small_58", "rock_small_59", "rock_small_60", "rock_small_61", "rock_small_62", "rock_small_63", "rock_small_64", "rock_small_65", "rock_small_66", "rock_small_67", "rock_small_68", "rock_small_69", "rock_small_70", "rock_small_71", "rock_small_72", "rock_small_73", "rock_small_74", "rock_small_75", "rock_small_76", "rock_small_77", "rock_small_78", "rock_small_79", "rock_small_80", "rock_small_81", "rock_small_82", "rock_small_83", "rock_small_84", "rock_small_85", "rock_small_86", "rock_small_87", "rock_small_88", "rock_small_89", "rock_small_90", "rock_small_91", "rock_small_92", "rock_small_93", "rock_small_94", "rock_small_95", "rock_small_96", "rock_small_97", "rock_small_98", "rock_small_99", "PShole", "scratches", "05_wall_scratch", "AreaUnlockDoor", "BarrelBlock", "cellar_wood01_6", "TimerPushTheDumbDoor02", "closedoorsound", "Push door!", "bangs", "05_event_door_bang", "cellar_wood01_5", "whineaa", "scare_whine_loop", "bang1", "TimerBangDoor", "bang2", "bang3", "bang4", "scare", "EnemyEventDone", "IGF", "AreaEnemyEvent_2", "CollideActivateEnemy", "monster", "grunt/amb_idle", "torch_static01_7", "grunt_normal_1", "enemy1", "TimerEnemy", "LookAtEnemy", "enemy2", "enemy3", "enemy4", "PathNodeArea_3", "speed", "react", "AreaEnemyEvent_3", "cellar_wood01_4slam", "scare_slam_door", "TimerPushTheDumbDoor", "react2", "EnemyTip01", "Ch01Level05", "TouchIt", "TimerTouchIt", "InteractTouchIt", "WineCellar", "AreaRemoveDoorEvent", "CollideAreaRemoveDoorEvent", "FuncUnlockDoor", "AreaBeginMoan", "FuncMoanHorrors", "AreaBlockDoor_0", "AreaBlockDoor_4", "AreaBlockDoor_5", "AreaBlockDoor_6", "AreaBlockDoor_7", "AreaBlockDoor_8", "AreaBlockDoor_9", "AreaBlockDoor_10", "AreaBlockDoor_11", "AreaBlockDoor_12", "AreaBlockDoor_13", "AreaBlockDoor_14", "AreaBlockDoor_15", "AreaBlockDoor_16", "AreaBlockDoor_17", "AreaBlockDoor_18", "AreaBlockDoor_19", "AreaBlockDoor_20", "AreaBlockDoor_21", "AreaBlockDoor_22", "AreaBlockDoor_23", "AreaBlockDoor_24", "AreaBlockDoor_25", "AreaBlockDoor_26", "AreaBlockDoor_27", "AreaBlockDoor_28", "AreaBlockDoor_29", "AreaBlockDoor_30", "AreaBlockDoor_31", "AreaBlockDoor_32", "AreaBlockDoor_33", "AreaBlockDoor_34", "AreaBlockDoor_35", "AreaBlockDoor_36", "AreaBlockDoor_37", "AreaBlockDoor_38", "AreaBlockDoor_39", "AreaBlockDoor_40", "AreaBlockDoor_41", "AreaBlockDoor_42", "AreaBlockDoor_43", "AreaBlockDoor_44", "AreaBlockDoor_45", "AreaBlockDoor_46", "AreaBlockDoor_47", "AreaBlockDoor_48", "AreaBlockDoor_49", "AreaBlockDoor_50", "AreaBlockDoor_51", "AreaBlockDoor_52", "AreaBlockDoor_53", "AreaBlockDoor_54", "AreaBlockDoor_55", "AreaBlockDoor_56", "AreaBlockDoor_57", "AreaBlockDoor_58", "AreaBlockDoor_59", "AreaBlockDoor_60", "AreaBlockDoor_61", "AreaBlockDoor_62", "AreaBlockDoor_63", "AreaBlockDoor_64", "AreaBlockDoor_65", "AreaBlockDoor_66", "AreaBlockDoor_67", "AreaBlockDoor_68", "AreaBlockDoor_69", "AreaBlockDoor_70", "AreaBlockDoor_71", "AreaBlockDoor_72", "AreaBlockDoor_73", "AreaBlockDoor_74", "AreaBlockDoor_75", "AreaBlockDoor_76", "AreaBlockDoor_77", "AreaBlockDoor_78", "AreaBlockDoor_79", "AreaBlockDoor_80", "AreaBlockDoor_81", "AreaBlockDoor_82", "AreaBlockDoor_83", "AreaBlockDoor_84", "AreaBlockDoor_85", "AreaBlockDoor_86", "AreaBlockDoor_87", "AreaBlockDoor_88", "AreaBlockDoor_89", "AreaBlockDoor_90", "AreaBlockDoor_91", "AreaBlockDoor_92", "AreaBlockDoor_93", "AreaBlockDoor_94", "AreaBlockDoor_95", "AreaBlockDoor_96", "AreaBlockDoor_97", "AreaBlockDoor_98", "AreaBlockDoor_99", "CollideBlockDoor", "AreaLoft", "CollideLoft", "CollideActiveCeiling", "CollideHoleEvents", "AreaBangDoor", "CollideBangDoor", "AreaEnemyHint", "CollideEnemyHint", "CollideGiveSanity", "CollideDeactivateEnemy", "LookAtCeiling", "InteractDoorWithGruntBehind", "InteractDoorToHole", "Rope0", "RopeStart_0", "RopeStart_1", "RopeStart_2", "RopeStart_3", "RopeStart_4", "RopeStart_5", "RopeStart_6", "RopeStart_7", "RopeStart_8", "RopeStart_9", "RopeStart_10", "RopeStart_11", "RopeStart_12", "RopeStart_13", "RopeStart_14", "RopeStart_15", "RopeStart_16", "RopeStart_17", "RopeStart_18", "RopeStart_19", "RopeStart_20", "RopeStart_21", "RopeStart_22", "RopeStart_23", "RopeStart_24", "RopeStart_25", "RopeStart_26", "RopeStart_27", "RopeStart_28", "RopeStart_29", "RopeStart_30", "RopeStart_31", "RopeStart_32", "RopeStart_33", "RopeStart_34", "RopeStart_35", "RopeStart_36", "RopeStart_37", "RopeStart_38", "RopeStart_39", "RopeStart_40", "RopeStart_41", "RopeStart_42", "RopeStart_43", "RopeStart_44", "RopeStart_45", "RopeStart_46", "RopeStart_47", "RopeStart_48", "RopeStart_49", "RopeStart_50", "RopeStart_51", "RopeStart_52", "RopeStart_53", "RopeStart_54", "RopeStart_55", "RopeStart_56", "RopeStart_57", "RopeStart_58", "RopeStart_59", "RopeStart_60", "RopeStart_61", "RopeStart_62", "RopeStart_63", "RopeStart_64", "RopeStart_65", "RopeStart_66", "RopeStart_67", "RopeStart_68", "RopeStart_69", "RopeStart_70", "RopeStart_71", "RopeStart_72", "RopeStart_73", "RopeStart_74", "RopeStart_75", "RopeStart_76", "RopeStart_77", "RopeStart_78", "RopeStart_79", "RopeStart_80", "RopeStart_81", "RopeStart_82", "RopeStart_83", "RopeStart_84", "RopeStart_85", "RopeStart_86", "RopeStart_87", "RopeStart_88", "RopeStart_89", "RopeStart_90", "RopeStart_91", "RopeStart_92", "RopeStart_93", "RopeStart_94", "RopeStart_95", "RopeStart_96", "RopeStart_97", "RopeStart_98", "RopeStart_99", "RopeEnd_0", "RopeEnd_1", "RopeEnd_2", "RopeEnd_3", "RopeEnd_4", "RopeEnd_5", "RopeEnd_6", "RopeEnd_7", "RopeEnd_8", "RopeEnd_9", "RopeEnd_10", "RopeEnd_11", "RopeEnd_12", "RopeEnd_13", "RopeEnd_14", "RopeEnd_15", "RopeEnd_16", "RopeEnd_17", "RopeEnd_18", "RopeEnd_19", "RopeEnd_20", "RopeEnd_21", "RopeEnd_22", "RopeEnd_23", "RopeEnd_24", "RopeEnd_25", "RopeEnd_26", "RopeEnd_27", "RopeEnd_28", "RopeEnd_29", "RopeEnd_30", "RopeEnd_31", "RopeEnd_32", "RopeEnd_33", "RopeEnd_34", "RopeEnd_35", "RopeEnd_36", "RopeEnd_37", "RopeEnd_38", "RopeEnd_39", "RopeEnd_40", "RopeEnd_41", "RopeEnd_42", "RopeEnd_43", "RopeEnd_44", "RopeEnd_45", "RopeEnd_46", "RopeEnd_47", "RopeEnd_48", "RopeEnd_49", "RopeEnd_50", "RopeEnd_51", "RopeEnd_52", "RopeEnd_53", "RopeEnd_54", "RopeEnd_55", "RopeEnd_56", "RopeEnd_57", "RopeEnd_58", "RopeEnd_59", "RopeEnd_60", "RopeEnd_61", "RopeEnd_62", "RopeEnd_63", "RopeEnd_64", "RopeEnd_65", "RopeEnd_66", "RopeEnd_67", "RopeEnd_68", "RopeEnd_69", "RopeEnd_70", "RopeEnd_71", "RopeEnd_72", "RopeEnd_73", "RopeEnd_74", "RopeEnd_75", "RopeEnd_76", "RopeEnd_77", "RopeEnd_78", "RopeEnd_79", "RopeEnd_80", "RopeEnd_81", "RopeEnd_82", "RopeEnd_83", "RopeEnd_84", "RopeEnd_85", "RopeEnd_86", "RopeEnd_87", "RopeEnd_88", "RopeEnd_89", "RopeEnd_90", "RopeEnd_91", "RopeEnd_92", "RopeEnd_93", "RopeEnd_94", "RopeEnd_95", "RopeEnd_96", "RopeEnd_97", "RopeEnd_98", "RopeEnd_99", "_Body_1", "TimerChain", "PlayerBeenInLevel05", "DarknessDecrease", "lantern", "tinderbox_0", "tinderbox_1", "tinderbox_2", "tinderbox_3", "tinderbox_4", "tinderbox_5", "tinderbox_6", "tinderbox_7", "tinderbox_8", "tinderbox_9", "tinderbox_10", "tinderbox_11", "tinderbox_12", "tinderbox_13", "tinderbox_14", "tinderbox_15", "tinderbox_16", "tinderbox_17", "tinderbox_18", "tinderbox_19", "tinderbox_20", "tinderbox_21", "tinderbox_22", "tinderbox_23", "tinderbox_24", "tinderbox_25", "tinderbox_26", "tinderbox_27", "tinderbox_28", "tinderbox_29", "tinderbox_30", "tinderbox_31", "tinderbox_32", "tinderbox_33", "tinderbox_34", "tinderbox_35", "tinderbox_36", "tinderbox_37", "tinderbox_38", "tinderbox_39", "tinderbox_40", "tinderbox_41", "tinderbox_42", "tinderbox_43", "tinderbox_44", "tinderbox_45", "tinderbox_46", "tinderbox_47", "tinderbox_48", "tinderbox_49", "tinderbox_50", "tinderbox_51", "tinderbox_52", "tinderbox_53", "tinderbox_54", "tinderbox_55", "tinderbox_56", "tinderbox_57", "tinderbox_58", "tinderbox_59", "tinderbox_60", "tinderbox_61", "tinderbox_62", "tinderbox_63", "tinderbox_64", "tinderbox_65", "tinderbox_66", "tinderbox_67", "tinderbox_68", "tinderbox_69", "tinderbox_70", "tinderbox_71", "tinderbox_72", "tinderbox_73", "tinderbox_74", "tinderbox_75", "tinderbox_76", "tinderbox_77", "tinderbox_78", "tinderbox_79", "tinderbox_80", "tinderbox_81", "tinderbox_82", "tinderbox_83", "tinderbox_84", "tinderbox_85", "tinderbox_86", "tinderbox_87", "tinderbox_88", "tinderbox_89", "tinderbox_90", "tinderbox_91", "tinderbox_92", "tinderbox_93", "tinderbox_94", "tinderbox_95", "tinderbox_96", "tinderbox_97", "tinderbox_98", "tinderbox_99", "FlashbackArea_1", "scare_male_terrified5", "scare_male_terrified", "scare_steps_big", "scare_tingeling", "close_door", "ps_blood_drop", "ps_dust_falling_hole", "ps_dust_impact_vert", "LoadingText", "Ch01_Diary02_0", "Ch01_Diary02_1", "Ch01_Diary02_2", "Ch01_Diary02_3", "Ch01_Diary02_4", "Ch01_Diary02_5", "Ch01_Diary02_6", "Ch01_Diary02_7", "Ch01_Diary02_8", "Ch01_Diary02_9", "Ch01_Diary02_10", "Ch01_Diary02_11", "Ch01_Diary02_12", "Ch01_Diary02_13", "Ch01_Diary02_14", "Ch01_Diary02_15", "Ch01_Diary02_16", "Ch01_Diary02_17", "Ch01_Diary02_18", "Ch01_Diary02_19", "Ch01_Diary02_20", "Ch01_Diary02_21", "Ch01_Diary02_22", "Ch01_Diary02_23", "Ch01_Diary02_24", "Ch01_Diary02_25", "Ch01_Diary02_26", "Ch01_Diary02_27", "Ch01_Diary02_28", "Ch01_Diary02_29", "Ch01_Diary02_30", "Ch01_Diary02_31", "Ch01_Diary02_32", "Ch01_Diary02_33", "Ch01_Diary02_34", "Ch01_Diary02_35", "Ch01_Diary02_36", "Ch01_Diary02_37", "Ch01_Diary02_38", "Ch01_Diary02_39", "Ch01_Diary02_40", "Ch01_Diary02_41", "Ch01_Diary02_42", "Ch01_Diary02_43", "Ch01_Diary02_44", "Ch01_Diary02_45", "Ch01_Diary02_46", "Ch01_Diary02_47", "Ch01_Diary02_48", "Ch01_Diary02_49", "Ch01_Diary02_50", "Ch01_Diary02_51", "Ch01_Diary02_52", "Ch01_Diary02_53", "Ch01_Diary02_54", "Ch01_Diary02_55", "Ch01_Diary02_56", "Ch01_Diary02_57", "Ch01_Diary02_58", "Ch01_Diary02_59", "Ch01_Diary02_60", "Ch01_Diary02_61", "Ch01_Diary02_62", "Ch01_Diary02_63", "Ch01_Diary02_64", "Ch01_Diary02_65", "Ch01_Diary02_66", "Ch01_Diary02_67", "Ch01_Diary02_68", "Ch01_Diary02_69", "Ch01_Diary02_70", "Ch01_Diary02_71", "Ch01_Diary02_72", "Ch01_Diary02_73", "Ch01_Diary02_74", "Ch01_Diary02_75", "Ch01_Diary02_76", "Ch01_Diary02_77", "Ch01_Diary02_78", "Ch01_Diary02_79", "Ch01_Diary02_80", "Ch01_Diary02_81", "Ch01_Diary02_82", "Ch01_Diary02_83", "Ch01_Diary02_84", "Ch01_Diary02_85", "Ch01_Diary02_86", "Ch01_Diary02_87", "Ch01_Diary02_88", "Ch01_Diary02_89", "Ch01_Diary02_90", "Ch01_Diary02_91", "Ch01_Diary02_92", "Ch01_Diary02_93", "Ch01_Diary02_94", "Ch01_Diary02_95", "Ch01_Diary02_96", "Ch01_Diary02_97", "Ch01_Diary02_98", "Ch01_Diary02_99");
    FakeDatabase.FindMusic("05_event_steps.ogg");
    FakeDatabase.FindMusic("05_paper_transformation.ogg");
    FakeDatabase.FindMusic("02_puzzle.ogg");
    FakeDatabase.FindMusic("04_amb");
    FakeDatabase.FindMusic("05_event_falling.ogg");	
	//----PRELOADING----//
	PreloadSound("scare_male_terrified5"); PreloadSound("scare_male_terrified"); PreloadSound("scare_steps_big"); PreloadSound("scare_wood_creak_mix"); 
	PreloadSound("scare_tingeling"); PreloadSound("01_whirl"); PreloadSound("general_rock_rumble"); PreloadSound("05_rock_fall"); 
	PreloadSound("05_rock_fall_2"); PreloadSound("scare_slam_door"); PreloadSound("react_breath"); PreloadSound("roll_wood"); 
	PreloadSound("react_scare"); PreloadSound("player_falldamage_max"); PreloadSound("player_bodyfall"); PreloadSound("05_cloud_swarm"); 
	PreloadSound("00_creak"); PreloadSound("react_pant"); PreloadSound("05_amb1"); PreloadSound("05_amb2"); 
	PreloadSound("player_climb"); PreloadSound("05_cloud"); PreloadSound("03_orb_loop"); PreloadSound("03_in_a_bottle"); 
	PreloadSound("general_blood_drop"); PreloadSound("05_whine"); PreloadSound("05_event_bottles"); PreloadSound("scare_wood_creak_walk"); 
	PreloadSound("05_event_door_bang"); PreloadSound("05_wall_scratch"); PreloadSound("scare_whine_loop"); PreloadSound("close_door"); 
	PreloadSound("grunt/amb_idle"); 

	PreloadParticleSystem("ps_dust_falling_small"); PreloadParticleSystem("ps_dust_whirl_large"); PreloadParticleSystem("ps_break_mansionbase_wall"); 
	PreloadParticleSystem("ps_ghost_release"); PreloadParticleSystem("ps_blood_drop"); PreloadParticleSystem("ps_dust_falling_hole"); 
	PreloadParticleSystem("ps_dust_impact_vert"); 
	      
	
	//----AUDIO----//
	PlayMusic("04_amb", true, 1, 5, 0, true);
	AutoSave();
}

////////////////////////////
// Run when leaving map
public override void OnLeave()
{
	SetEntityActive("grunt_normal_1", false);	//Just incase player would be very very fast and throw a box at enemy during event.
	
	//////////////////////
	//Load Screen Setup
	SetupLoadScreen("LoadingText", "Ch01_Diary02_", 4, "game_loading_desert.jpg");
}
}
