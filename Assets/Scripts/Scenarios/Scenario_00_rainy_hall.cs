using UnityEngine;
using System.Collections;

public class Scenario_00_rainy_hall : Scenario {
private void Start() {}

//---------------------------------------------

////////////////////////
// GENERAL
////////////////////////

//---------------------------------------------

public void PlayEffectVoice(string  asEntryBase,int alStartIdx, int alEndIdx, string  asCallback)
{	
	for(int i=alStartIdx; i<=alEndIdx; ++i)
	{
		string sEntry = asEntryBase;
		if(i<10) sEntry += "0";
		sEntry += i;
		
		AddEffectVoice(sEntry, "", "Flashbacks", sEntry, false, "", 0,0 );
	}

	SetEffectVoiceOverCallback(asCallback);
}

//---------------------------------------------

////////////////////////
// INTRO FLASHBACKS
////////////////////////

//---------------------------------------------

float gfIntroSequenceGlobalVolume = 0.3f;
float gfIntroSequenceFadeOutTime = 3;

//---------------------------------------------

public void TimerStartGame(string  asTimer)
{
	PlayMusic("23_amb.ogg", true, 0.7f, 1, 0, false);
			
	AddTimer("IntroSequence", 2, "TimerIntroSequence");
}

public void ResumeIntroSequence()
{
	AddTimer("IntroSequence", 0.1f, "TimerIntroSequence");
}

//---------------------------------------------

public void TimerIntroSequence(string  asTimer)
{
	int lState = GetLocalVarInt("IntroSequenceState");
	AddLocalVarInt("IntroSequenceState",1);
	float fNextEventTime = 1.0f;
	bool bPause = false;
	
	////////////////////////
	// 0: Fade In #1
	if(lState ==0)
	{
		FadeIn(3);
		SetPlayerActive(true);
		
		SetPlayerMoveSpeedMul(0.35f);
		SetPlayerRunSpeedMul(0);
		//SetPlayerLookSpeedMul(0.5f);
		
		FadeImageTrailTo(2,1);
		FadeSepiaColorTo(1,1);	
		
		FadePlayerRollTo(12, 0.3f,0.7f); 
		FadePlayerFOVMulTo(0.7f, 0.05f);
		
		FadeGlobalSoundVolume(gfIntroSequenceGlobalVolume, 3);
		
		PlayGuiSound("fb_sfx_00_daniel.ogg", 1.0f);
		PlayEffectVoice("CH01L00_DanielsMind01_", 1, 1, "IntroSequenceVoiceOver");
		PlayEffectVoice("CH01L00_DanielsMind02_", 1, 1, "IntroSequenceVoiceOver");
		PlayEffectVoice("CH01L00_DanielsMind03_", 1, 1, "IntroSequenceVoiceOver");
		PlayEffectVoice("CH01L00_DanielsMind04_", 1, 1, "IntroSequenceVoiceOver");
		
		fNextEventTime = 5.0f;
		//bPause = true;		
	}
	////////////////////////
	// 1: Fade Out #1
	else if(lState ==1)
	{
		FadeOut(gfIntroSequenceFadeOutTime);
		FadeGlobalSoundVolume(0, gfIntroSequenceFadeOutTime);
		
		fNextEventTime = gfIntroSequenceFadeOutTime + 0.1f;
	}
	////////////////////////
	// 2: Fade In #2
	else if(lState ==2)
	{
		FadeIn(3);
		TeleportPlayer("IntroStart_2");
		
		FadePlayerRollTo(-12, 0.3f, 0.8f); 
		FadePlayerFOVMulTo(1.2f, 0.03f);
		
		FadeGlobalSoundVolume(gfIntroSequenceGlobalVolume, 3);
		
		fNextEventTime = 5.0f;
		//PlayEffectVoice("CH01L00_DanielsMind02_", 1, 1, "IntroSequenceVoiceOver");
		//bPause = true;		
	}
	////////////////////////
	// 3: Fade Out #2
	else if(lState ==3)
	{
		FadeOut(gfIntroSequenceFadeOutTime);
		FadeGlobalSoundVolume(0, gfIntroSequenceFadeOutTime);
		
		fNextEventTime = gfIntroSequenceFadeOutTime + 0.1f;
	}
	////////////////////////
	// 4: Fade In #3
	else if(lState ==4)
	{
		FadeIn(3);
		TeleportPlayer("IntroStart_3");
		
		FadePlayerRollTo(12, 0.4f, 0.5f); 
		FadePlayerFOVMulTo(0.5f, 0.05f);
		
		FadeGlobalSoundVolume(gfIntroSequenceGlobalVolume, 3);
		
		fNextEventTime = 5.0f;
		//PlayEffectVoice("CH01L00_DanielsMind03_", 1, 1, "IntroSequenceVoiceOver");
		//bPause = true;		
	}
	////////////////////////
	// 5: Fade Out #3
	else if(lState ==5)
	{
		FadeOut(gfIntroSequenceFadeOutTime);
		FadeGlobalSoundVolume(0, gfIntroSequenceFadeOutTime);
		
		fNextEventTime = gfIntroSequenceFadeOutTime + 0.1f;
	}
	////////////////////////
	// 6: Fade In #4
	else if(lState ==6)
	{
		FadeIn(3);
		TeleportPlayer("IntroStart_4");
		
		FadePlayerRollTo(-12, 0.4f, 0.8f); 
		FadePlayerFOVMulTo(1.3f, 0.1f);
		
		FadeGlobalSoundVolume(gfIntroSequenceGlobalVolume, 3);
		
		fNextEventTime = 6.0f;
		//PlayEffectVoice("CH01L00_DanielsMind04_", 1, 1, "IntroSequenceVoiceOver");
		//bPause = true;		
	}
	////////////////////////
	// 7: Fade Out #4
	else if(lState ==7)
	{
		FadeOut(gfIntroSequenceFadeOutTime);
		FadeGlobalSoundVolume(0, gfIntroSequenceFadeOutTime);
		
		AddTimer("WakeUpPlayer", gfIntroSequenceFadeOutTime+4, "TimerWakeUpPlayer");
		AddTimer("activefix", gfIntroSequenceFadeOutTime, "TimerActiveFix");
		bPause = true;
		return;
	}
	
	AddDebugMessage("Event:"+lState+" Time:"+fNextEventTime,false);
	
	if(bPause==false)
		AddTimer("IntroSequence", fNextEventTime, "TimerIntroSequence");
}

public void TimerActiveFix(string  asTimer)
{
	SetPlayerActive(false);
}

//---------------------------------------------

public void IntroSequenceVoiceOver()
{
	ResumeIntroSequence();
}

//---------------------------------------------

public void TimerWakeUpPlayer(string  asTimer)
{
	TeleportPlayer("PlayerStartArea_1");
	
	StopMusic(3, 0);
	
	PlayGuiSound("player_bodyfall5.ogg",1);
	
	FadeImageTrailTo(0,1);
	FadeSepiaColorTo(0,1);
	
	FadePlayerRollTo(0, 1, 3); 
	FadePlayerFOVMulTo(1, 1);
		
	FadeGlobalSoundVolume(1, 3);
	FadeGlobalSoundSpeed(1, 3);
	
	SetInventoryDisabled(false);
	
	SetEntityActive("narrowpassout", true);
	SetEntityActive("gallerydoor", true);
	SetEntityActive("areanarrowhalls", true);
	SetEntityActive("areaendthunder", true);
	SetEntityActive("areabeginthunder", true);
	SetEntityActive("areagustdoor", true);
	SetEntityActive("AreaDust3", true);
	SetEntityActive("AreaFaint", true);
	
	SetPlayerJumpDisabled(true);
	SetPlayerCrouchDisabled(true);
	SetPlayerRunSpeedMul(0);
	FadePlayerRollTo(75, 10, 100); 	//positivt �r roll fr�n up mot v�nster i grader. roll, speedX, maxspeed
	MovePlayerHeadPos(-0.2f, -1.3f, 0, 10, 0.5f);	//x(0 def), y(0 def), z(0 def), speed, slowdowndist
	StartPlayerLookAt("Arealook1", 10, 100, "");	//Area,speedX,maxspeed,callback()	
	
	AddTimer("blackout", 9.0f, "TimerBlackOut");
	AddTimer("forhelbeep", 5.2f, "TimerBeginSoundsIntro");
}

//---------------------------------------------

////////////////////////
// WAKE UP
////////////////////////

//---------------------------------------------

//NOTE: The following are not to be used! just letting the info be here so can use later.
/*public void TimerSFXIntro(string  asTimer)
{
	PlayGuiSound("fb_sfx_00_daniel.ogg", 1.0f);
}
public void TimerVoiceIntro(string  asTimer)
{
	AddEffectVoice("CH01L00_DanielsMind01_01.ogg", "", "Flashbacks", "CH01L00_DanielsMind01_01", false, "", 0, 0);
	AddEffectVoice("CH01L00_DanielsMind02_01.ogg", "", "Flashbacks", "CH01L00_DanielsMind02_01", false, "", 0, 0);
	AddEffectVoice("CH01L00_DanielsMind03_01.ogg", "", "Flashbacks", "CH01L00_DanielsMind03_01", false, "", 0, 0);
	AddEffectVoice("CH01L00_DanielsMind04_01.ogg", "", "Flashbacks", "CH01L00_DanielsMind04_01", false, "", 0, 0);

	SetEffectVoiceOverCallback("VoiceOverIntro");
}*/

public void TimerBeginSoundsIntro(string  asTimer)
{
	PlaySoundAtEntity("Rain", "general_rain_m.snt", "AreaThunder", 4, true);
	PlaySoundAtEntity("Eerie", "ambience_wind_eerie_no3d.snt", "AreaThunder", 4, true);
	PlaySoundAtEntity("Hollow", "ambience_hollow_tinker.snt", "AreaThunder", 4, true);
	PlaySoundAtEntity("Flow", "12_epoxy_flow.snt", "AreaFlowWater", 12, true);
}

//////////////////////////////////////////////
/*PLayer wakes up on floor and slowly gets up, when up receives first quest.
 */
public void TimerBlackOut(string  asTimer)
{
	AddLocalVarInt("BlackoutStep", 1);	//What step to play in the event
	float fEventSpeed = 0.5f;				//The default time between steps in an event

	switch(GetLocalVarInt("BlackoutStep")) {
		case 1:
			StartPlayerLookAt("Arealook2", 0.1f, 0.1f, "");
			FadeIn(4);
			FadeImageTrailTo(2,1);
			AddTimer("rose", 0.5f, "TimerRose");
			SetPlayerActive(true);
			ShowPlayerCrossHairIcons(true);
			SetPlayerMoveSpeedMul(0.05f);
			SetPlayerLookSpeedMul(0.05f);
			fEventSpeed = 3.0f;	
		break;
		case 2:
			FadePlayerRollTo(85, 1, 1); 
		break;
		case 3:
			StartPlayerLookAt("Arealook3", 0.1f, 0.1f, "");
		break;
		case 4:
			FadeImageTrailTo(0,1);
			FadePlayerRollTo(65, 1, 1); 
			AddTimer("thunder", 1, "TimerThunder");
		break;
		case 5:
			PlaySoundAtEntity("sigh", "react_sigh.snt", "Player", 1.0f / 2, false);
			FadeOut(2);
			fEventSpeed = 1.5f;	
		break;
		case 6:
			FadePlayerRollTo(85, 1, 4); 
			StartPlayerLookAt("Arealook1", 0.1f, 0.1f, "");
		break;
		case 7:
			FadeImageTrailTo(1.8f,1.5f);
			FadePlayerFOVMulTo(1.25f, 0.01f);
		break;
		case 8:
			FadePlayerRollTo(45, 1, 2); 
			FadeIn(2);
			fEventSpeed = 1.5f;	
		break;
		case 9:
			StartPlayerLookAt("Arealook2", 0.1f, 0.1f, "");
		break;
		case 10:
			FadePlayerRollTo(15, 1, 2); 
			FadePlayerFOVMulTo(0.75f, 0.01f);
		break;
		case 11:
			PlaySoundAtEntity("sigh", "react_sigh.snt", "Player", 1.0f / 1.5f, false);
			FadeOut(1);
			StartPlayerLookAt("Arealook3", 1, 1, "");
			FadePlayerRollTo(50, 1, 2); 
			fEventSpeed = 2.0f;	
		break;
		case 12:
			SetPlayerMoveSpeedMul(0.1f);
			SetPlayerLookSpeedMul(0.1f);
			StartPlayerLookAt("Arealook1", 1, 1, "");
			FadePlayerFOVMulTo(1.1f, 0.01f);
			FadeImageTrailTo(0,1.5f);
			fEventSpeed = 1.5f;	
		break;
		case 13:
			SetPlayerMoveSpeedMul(0.2f);
			SetPlayerLookSpeedMul(0.2f);
			FadePlayerRollTo(-15, 1, 2); 
			FadeIn(1);
			StartPlayerLookAt("Arealook4", 2, 2, "");
			fEventSpeed = 2.0f;	
		break;
		case 14:
			SetPlayerMoveSpeedMul(0.3f);
			SetPlayerLookSpeedMul(0.4f);
			FadePlayerRollTo(-30, 10, 60); 	
			MovePlayerHeadPos(0, 0, 0, 1, 0.5f);	
			StartPlayerLookAt("Arealook3", 1, 1, "");
			FadePlayerFOVMulTo(0.9f, 0.01f);
			FadeImageTrailTo(1.5f,2);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
		break;
		case 15:
			SetPlayerMoveSpeedMul(0.4f);
			SetPlayerLookSpeedMul(0.6f);
			FadePlayerRollTo(10, 10, 20); 	
			MovePlayerHeadPos(0, -0.5f, 0, 1, 0.5f);
		break;
		case 16:
			SetPlayerMoveSpeedMul(0.5f);
			SetPlayerLookSpeedMul(0.8f);
			FadePlayerRollTo(0, 10, 60); 	
			MovePlayerHeadPos(0, 0, 0, 1, 0.5f);
			StartPlayerLookAt("Arealook4", 2, 2, "");
			FadePlayerFOVMulTo(1, 0.01f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
			fEventSpeed = 2.0f;	
		break;
		case 17:
			SetPlayerJumpDisabled(false);
			SetPlayerCrouchDisabled(false);
			SetPlayerMoveSpeedMul(0.6f);
			SetPlayerLookSpeedMul(1.0f);
			FadeImageTrailTo(0,0.2f);
			StopPlayerLookAt();
			PlaySoundAtEntity("sigh", "react_sigh.snt", "Player", 1.0f / 1, false);
			AddTimer("lookloop", RandFloat(2.0f,6.0f), "TimerRandomLook");	//Activate the spinning head
			fEventSpeed = 1.0f;		
		break;
		case 18:
			SetEntityActive("AreaQuest", true);
			//AddQuest("00Trail","00Trail"); In LookAtQuest instead
		break;
	}
	
	if(GetLocalVarInt("BlackoutStep") < 19)  AddTimer("blackout", fEventSpeed, "TimerBlackOut");
}
public void TimerRose(string  asTimer)
{
	CreateParticleSystemAtEntity("rose", "ps_rose_petals.ps", "AreaRose", false);
}

//---------------------------------------------

////////////////////////////////////
//BEGIN THUNDER
////////////////////////////////////

//---------------------------------------------

/*Random lightning and thunder at start of level
 */
public void TimerThunder(string  asTimer)
{
	AddLocalVarInt("ThunderStep", 1);				//What step to play in the event
	float fEventSpeed = RandFloat(0.05f, 0.15f);	//The default time between steps in an event

	switch(GetLocalVarInt("ThunderStep")) {
		case 1:
			ThunderLights(2,0.05f);
		break;
		case 2:
			ThunderLights(1,0.05f);
		break;
		case 3:
			ThunderLights(3,0.05f);
		break;
		case 4:
			ThunderLights(1,0.05f);
		break;
		case 5:
			ThunderLights(2,0.05f);
		break;
		case 6:
			ThunderLights(3,0.05f);
		break;
		case 7:
			ThunderLights(1,0.3f);
			PlaySoundAtEntity("Thunder", "general_thunder.snt", "AreaThunder", 0.0f, false);
		break;
	}
	
	if(GetLocalVarInt("ThunderStep") < 8)  AddTimer("thunder", fEventSpeed, "TimerThunder");
	else { 
		SetLocalVarInt("ThunderStep", 0); 
		
		AddTimer("thunder", RandFloat(10.0f, 30.0f), "TimerThunder"); 
	}
}
public void ThunderLights(int alIntensity, float afFade)
{
	/*Skip parts of a flash everynow and then but not the first strong light 
	**and not the last "back to normal" light
	 */
	if(RandFloat(1, 3) == 1 && (GetLocalVarInt("ThunderStep") != 3 || GetLocalVarInt("ThunderStep") != 7)) return;
	
	float fF = 0.2f;
	
	switch(alIntensity) {
		case 1:
			for(int i=0;i<=5;i++) FadeLightTo("spotthunder_"+i,0.52f,0.55f,0.6f,0.45f,-1,afFade-0.04f);
			for(int i=0;i<=7;i++) FadeLightTo("pointthunder_"+i,0.32f,0.35f,0.4f,0.2f,-1,afFade-0.025f);
			for(int i=0;i<=3;i++) FadeLightTo("ambthunder_"+i,0.2f,0.25f,0.35f,-1,-1,afFade);
		break;
		case 2:
			for(int i=0;i<=5;i++) FadeLightTo("spotthunder_"+i,0.82f+fF,0.85f+fF,0.9f+fF,0.9f+fF,-1,afFade-0.04f);
			for(int i=0;i<=7;i++) FadeLightTo("pointthunder_"+i,0.72f+fF,0.75f+fF,0.8f+fF,0.4f+fF,-1,afFade-0.025f);
			for(int i=0;i<=3;i++) FadeLightTo("ambthunder_"+i,0.25f+fF,0.3f+fF,0.4f+fF,-1,-1,afFade);
		break;
		case 3:
			for(int i=0;i<=5;i++) FadeLightTo("spotthunder_"+i,0.92f+fF,0.95f+fF,1+fF,1+fF,-1,afFade-0.04f);
			for(int i=0;i<=7;i++) FadeLightTo("pointthunder_"+i,0.82f+fF,0.85f+fF,0.9f+fF,0.5f+fF,-1,afFade-0.025f);
			for(int i=0;i<=3;i++) FadeLightTo("ambthunder_"+i,0.3f+fF,0.35f+fF,0.45f+fF,-1,-1,afFade);
		break;
	}
}
/*End the thunder loop when in hall 3
 */
public void CollideAreaThunder(string  asParent, string  asChild, int alState)
{
	if(asChild == "areabeginthunder") {
		AddTimer("thunder", RandFloat(1, 5), "TimerThunder"); 
		
		AddEntityCollideCallback("Player", "areaendthunder", "CollideAreaThunder", true, 1);
		
	} else {
		RemoveTimer("thunder");
		
		ThunderLights(1,0.3f);
		
		AddEntityCollideCallback("Player", "areabeginthunder", "CollideAreaThunder", true, 1);
	}
}

//---------------------------------------------

//////////////////////////////////////
// BEGIN NARROW HALLS
//////////////////////////////////////

//---------------------------------------------

/*Begin events in narrow hallway
 */
public void CollideNarrowHalls(string  asParent, string  asChild, int alState)
{
	if(alState==1){
		PlaySoundAtEntity("creak", "00_creak.snt", "Player", 3, false);
		
		for(int i=1;i<=9;i++) CreateParticleSystemAtEntity("AreaHallPS_"+i, "ps_dust_falling_narrow", "AreaHallPS_"+i, false);

		FadePlayerAspectMulTo(1.7f, 0.03f); 
		
		FadeImageTrailTo(1.5f,1.25f);
		
		AddTimer("hallway", RandFloat(0.1f,1.5f), "HallwayEvents");
		RemoveTimer("lookloop");
		StopRandomLook();
		
		if(GetLocalVarInt("HallwaySteps") == 0)
			AddTimer("steps", 0.5f, "HallwaySteps");
		
	} else {
		StopSound("creak", 3);
		
		for(int i=1;i<=9;i++) DestroyParticleSystem("AreaHallPS_"+i);
		
		SetPlayerMoveSpeedMul(1);
			
		FadePlayerAspectMulTo(1,0.3f); 
		
		FadeImageTrailTo(0.0f,1.5f);
		
		AddTimer("lookloop", RandFloat(2.0f,6.0f), "TimerRandomLook");
		RemoveTimer("hallway");
		
		SetLocalVarInt("HallwayStep", 0); 
		SetLocalVarInt("InitUniqueRandom", 0);
	}
}
/*Footsteps that run away when entering hallway
 */
float fStep=0.0f;
float fLoop=0.4f;
public void HallwaySteps(string  asTimer)
{	
	AddLocalVarInt("HallwaySteps", 1);
	
	if(GetLocalVarInt("HallwaySteps") <= 5) {
		PlaySoundAtEntity("step", "scare_walk_hallway.snt", "Arealookhall3", fStep+=0.03f, false);
		AddTimer("steps", fLoop-=0.04f , "HallwaySteps");
		
	} else if(GetLocalVarInt("HallwaySteps") >= 6 && GetLocalVarInt("HallwaySteps") <= 10) {
		PlaySoundAtEntity("step", "scare_walk_hallway.snt", "Arealookhall3", fStep+=0.01f, false);
		AddTimer("steps", fLoop+=0.1f , "HallwaySteps");
	}
	
	AddDebugMessage("Step!" + fStep, false);	
}
/*Random events before passing out, swinging candeliers
 */
bool bSwing = true;
public void HallwayEvents(string  asTimer)
{	
	SetLocalVarInt("HallwayStep", RandFloat(1,4));	//What step to play in the event
	
	float fEventSpeed = 2.0f;		//The default time between steps in an event
	
	bSwing = bSwing == false ? true : false;

	if(bSwing){
		for(int i=0;i<=110;i++) {
			AddBodyForce("chand1_Body_3", i*0.04f, 0, i*0.04f, "World"); 
			AddBodyForce("chand2_Body_3", i*0.04f, 0, i*0.04f, "World");
			AddBodyForce("chandelier_simple_3_Body_3", i*0.04f, 0, i*0.04f, "World");
		}
	
	} else {
		for(int i=0;i<=110;i++) {
			AddBodyForce("chand1_Body_3", -i*0.04f, 0, -i*0.04f, "World"); 
			AddBodyForce("chand2_Body_3", -i*0.04f, 0, -i*0.04f, "World");
			AddBodyForce("chandelier_simple_3_Body_3", -i*0.04f, 0, -i*0.04f, "World");
		}
	}
		
	switch(GetLocalVarInt("HallwayStep")){
		case 1:
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.3f,false);
			SetPlayerMoveSpeedMul(0.85f);
			StartScreenShake(0.002f,1, 0.5f,0.5f);
		break;
		case 2:
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f /  0.4f,false);
			SetPlayerMoveSpeedMul(0.8f);
			StartScreenShake(0.004f,1, 0.5f,0.5f);
		break;
		case 3:
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.7f,false);
			SetPlayerMoveSpeedMul(0.75f);
			StartScreenShake(0.005f,1, 0.5f,0.5f);
		break;
		case 4:
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.5f,false);
			SetPlayerMoveSpeedMul(0.7f);
			StartScreenShake(0.006f,1, 0.5f,0.5f);
		break;
	}
	
	 AddTimer("hallway", fEventSpeed, "HallwayEvents");
}
/*In middle of passage, super scary event occurs and players passes out
 */
public void CollideAreaNarrowPassOut(string  asParent, string  asChild, int alState)
{
	SetEntityActive("areanarrowhalls", false);	//Turn of the area that triggers the hallway spooks
	
	StopRandomLook();	//De-activate the spinning head
	
	SetPlayerJumpDisabled(true);
	SetPlayerCrouchDisabled(true);

	PlaySoundAtEntity("scare", "react_scare.snt", "Player", 1.0f / 1,false);
	//PlaySoundAtEntity("cuts", "00_cuts.snt", "Player", 1.0f / 4,false);
	PlaySoundAtEntity("cuts", "00_cuts_3d.snt", "Arealookhall3_1", 1.0f / 4,false);
	StopSound("creak", 3);
	
	StartScreenShake(0.01f,0.45f, 0.1f,0.75f);
	
	FadeImageTrailTo(1.7f,1.0f);
	FadePlayerFOVMulTo(1.5f, 0.5f);
	
	AddTimer("narrowpass", 3, "NarrowPassOutEvents");

	/*Turn off hallway events
	 */
	FadePlayerAspectMulTo(1,0.1f); 
	RemoveTimer("hallway");
}
public void NarrowPassOutEvents(string  asTimer)
{
	AddLocalVarInt("HallPassOutStep", 1);	//What step to play in the event
	float fEventSpeed = 0.5f;		//The default time between steps in an event
	
	switch(GetLocalVarInt("HallPassOutStep")){
		case 1:
			PlayMusic("00_event_hallway", false, 1,1, 10, false);
			PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0,false);
			StartPlayerLookAt("Arealookhall1", 0.5f, 0.5f, "");
			SetPlayerMoveSpeedMul(0.7f);
			SetPlayerLookSpeedMul(0.7f);
			fEventSpeed = 1.0f;
		break;
		case 2:
			PlaySoundAtEntity("loop", "00_loop.snt", "Player", 1.0f / 0.05f, false);
			SetPlayerMoveSpeedMul(0.6f);
			SetPlayerLookSpeedMul(0.6f);
			SetPlayerCrouching(false);
			MovePlayerHeadPos(0, -1.3f, 0, 0.75f, 0.5f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0,false);
		break;
		case 3:
			PlaySoundAtEntity("scare", "react_scare.snt", "Player", 1.0f / 0.5f,false);
			SetPlayerMoveSpeedMul(0.5f);
			SetPlayerLookSpeedMul(0.5f);
			FadePlayerRollTo(30, 2, 10);
		break;
		case 4:
			FadeImageTrailTo(0.0f,0.5f);
			SetPlayerMoveSpeedMul(0.8f);
			SetPlayerLookSpeedMul(0.8f);
			FadePlayerFOVMulTo(1, 1);
			MovePlayerHeadPos(0, 0, 0, 2, 0.5f);
			fEventSpeed = 1.0f;
		break;
		case 5:
			SetPlayerMoveSpeedMul(0.4f);
			SetPlayerLookSpeedMul(0.4f);
			StartPlayerLookAt("Arealookhall2", 1, 1, "");
			MovePlayerHeadPos(0, -1.3f, 0, 5, 0.25f);
			//PlaySoundAtEntity("bodyfall", "player_bodyfall.snt", "Player", 0,false);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0,false);
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.75f,false);
			fEventSpeed = 1.0f;
		break;
		case 6:
			PlaySoundAtEntity("scare", "react_scare.snt", "Player", 1.0f / 1,false);
			SetPlayerMoveSpeedMul(0.3f);
			SetPlayerLookSpeedMul(0.3f);
			FadePlayerRollTo(-30, 10, 20);
		break;
		case 7:
			SetPlayerMoveSpeedMul(0.6f);
			SetPlayerLookSpeedMul(0.6f);
			StartPlayerLookAt("Arealookhall1", 2, 2, "");
			FadePlayerRollTo(30, 20, 40);
			MovePlayerHeadPos(-1, -1.3f, 0, 1, 0.25f);
			//StopSound("Eerie", 0.25f);
			//StopSound("Hollow", 0.25f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0,false);
		break;
		case 8:
			SetPlayerMoveSpeedMul(0.4f);
			SetPlayerLookSpeedMul(0.4f);
			FadeOut(3);
			FadePlayerRollTo(75, 15, 30);
			StopPlayerLookAt();
		break;
		case 9:
			for(int i=1;i<=9;i++) DestroyParticleSystem("AreaHallPS_"+i);
			GiveSanityDamage(10, false);
			PlaySoundAtEntity("faint", "00_faint.snt", "Player", 1.0f / 1,false);
			SetPlayerMoveSpeedMul(0.2f);
			SetPlayerLookSpeedMul(0.2f);
			//StopPlayerLookAt();
			//AddTimer("inactive", 1.0f, "TimerPlayerInactive");
			AddTimer("narrowpass2", 0.1f, "NarrowPassOutEvents02");
			SetLocalVarInt("HallPassOutStep", 0);
			return;
		break;
	}
	
	if(GetLocalVarInt("HallPassOutStep") < 10)   AddTimer("narrowpass", fEventSpeed, "NarrowPassOutEvents");
}
/*Player wakes up again and can continue onwards
 */
public void NarrowPassOutEvents02(string  asTimer)
{
	AddLocalVarInt("HallPassOutStep", 1);	//What step to play in the event
	float fEventSpeed = 0.5f;		//The default time between steps in an event
	
	switch(GetLocalVarInt("HallPassOutStep")){
		case 1:
			//SetPlayerActive(true);
			FadeImageTrailTo(1.8f,2.0f);
			//PlaySoundAtEntity("Pant!", "react_pant.snt", "Player", 0,false);
			StopSound("loop", 3);
			FadeIn(1);
			FadePlayerRollTo(30, 2, 10);
		break;
		case 2:
			//PlaySoundAtEntity("Eerie", "ambience_wind_eerie_no3d.snt", "AreaThunder", 1.0f / 0.1f,false);
			//PlaySoundAtEntity("Hollow", "ambience_hollow_tinker.snt", "AreaThunder", 1.0f / 0.1f, false);
			StartPlayerLookAt("Arealookhall1", 0.1f, 0.1f, "");
			MovePlayerHeadPos(0, -1.0f, 0, 0.5f, 0.5f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
		break;
		case 3:
			SetPlayerLookSpeedMul(0.3f);
			SetPlayerMoveSpeedMul(0.3f);
		break;
		case 4:
			SetPlayerLookSpeedMul(0.4f);
			SetPlayerMoveSpeedMul(0.4f);
			MovePlayerHeadPos(0, 0, 0, 2, 0.5f);
		break;
		case 5:
			SetPlayerLookSpeedMul(0.5f);
			SetPlayerMoveSpeedMul(0.5f);
			MovePlayerHeadPos(0, -1.3f, 0, 3, 0.5f);
			PlaySoundAtEntity("bodyfall", "player_bodyfall.snt", "Player", 1.0f / 1, false);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
			StartPlayerLookAt("Arealookhall3", 1.8f, 1.8f, "");
			FadePlayerRollTo(-30, 3, 10);
		break;
		case 6:
			SetPlayerLookSpeedMul(0.6f);
			SetPlayerMoveSpeedMul(0.7f);
			FadePlayerRollTo(0, 14, 35);
		break;
		case 7:
			SetPlayerLookSpeedMul(0.7f);
			SetPlayerMoveSpeedMul(0.7f);
			StartPlayerLookAt("Arealookhall1", 5.1f, 5.1f, "");
			
		break;
		case 8:
			SetPlayerLookSpeedMul(0.8f);
			SetPlayerMoveSpeedMul(0.8f);
			MovePlayerHeadPos(0, 0, 0, 4, 0.5f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
			StartPlayerLookAt("Arealookhall3", 5.1f, 5.1f, "");
		break;
		case 9:
			SetPlayerLookSpeedMul(0.9f);
			SetPlayerMoveSpeedMul(0.9f);
			FadeImageTrailTo(0,0.2f);
			StopPlayerLookAt();
		break;
		case 10:
			SetPlayerLookSpeedMul(1.0f);
			SetPlayerMoveSpeedMul(1.0f);
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 1, false);
			fEventSpeed = 2.0f;
		break;
		case 11:
			//SetPlayerLookSpeedMul(0.9f);
			//SetPlayerMoveSpeedMul(0.9f);
			AddTimer("lookloop", RandFloat(3.0f,6.0f), "TimerRandomLook");	//Re-activate the spinning head
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.75f, false);
			fEventSpeed = 2.0f;
		break;
		case 12:
			//SetPlayerLookSpeedMul(1.0f);
			//SetPlayerMoveSpeedMul(1.0f);
			SetPlayerJumpDisabled(false);
			SetPlayerCrouchDisabled(false);
			PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.5f, false);
			fEventSpeed = 2.0f;
		break;
	}
	if(GetLocalVarInt("HallPassOutStep") < 13)   AddTimer("narrowpass2", fEventSpeed, "NarrowPassOutEvents02");
}
public void TimerPlayerInactive(string  asTimer)
{
	SetPlayerActive(false);
}

//---------------------------------------------

///////////////////////////////////
// BEGIN GALLERY DOOR SWING OPEN
///////////////////////////////////

//---------------------------------------------

/*When in hall4 gallery door swings open slowly
 */
public void CollideAreaGalleryDoor(string  asParent, string  asChild, int alState)
{
	SetSwingDoorClosed("door_gallery", false, false);
	SetSwingDoorDisableAutoClose("door_gallery", true);
	
	PlaySoundAtEntity("creaking_door", "joint_door_move_special.snt", "door_gallery", 1.0f / 0.7f, false);
	
	AddTimer("door_gallery", 0.01f, "TimerSwingDoor");
	
	AddDebugMessage("Boho, the gallery door creaks open.", false);	
	
	AddTimer("stopcreak", 2.0f, "TimerStopCreak");
}
public void TimerSwingDoor(string  asTimer)
{
	if(GetLocalVarInt("SwingDoor") == 10){
		SetLocalVarInt("SwingDoor", 0);
		return;
	}
	
	if(asTimer == "door_gallery") AddPropForce(asTimer, 70.0f, 0, 0, "World"); 
	else AddPropForce(asTimer, -95.0f, 0, 0, "World"); 
	
	AddLocalVarInt("SwingDoor", 1);
	
	AddTimer(asTimer, 0.03f, "TimerSwingDoor");
	
	AddDebugMessage("Swing: "+GetLocalVarInt("SwingDoor"), false);
}
public void TimerStopCreak(string  asTimer)
{
	if(asTimer == "scare2"){
		PlaySoundAtEntity("scare", "react_scare.snt", "Player", 1.0f / 1, false);
		return;
	}
	if(asTimer == "scare"){
		PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0, false);
		//PlaySoundAtEntity("laugh", "00_laugh.snt", "door_gallery", 1.0f / 1, false);
		PlaySoundAtEntity("loop", "00_loop.snt", "Player", 1.0f / 0.1f, false);
		return;
	}
	if(asTimer == "breath1"){
		PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 1, false);
		return;
	}
	if(asTimer == "breath2"){
		PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.9f, false);
		return;
	}
	if(asTimer == "breath3"){
		//SetSwingDoorLocked("door_gallery", false, true);
		PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.8f, false);
		AddTimer("lookloop", RandFloat(2.0f,3.0f), "TimerRandomLook");	//Re-activate the spinning head
		FadePlayerFOVMulTo(1.0f, 0.1f);
		StopSound("loop", 1);
		StartScreenShake(0.01f,0.25f, 0.01f,0.5f);
		return;
	}
	if(asTimer == "lightsout"){
		SetLampLit("chandelier_nice_1", false, true);
		SetLampLit("candlestick_floor_2", false, true);
		FadeLightTo("PointLight_25", 0, 0, 0, 0, 0, 1);
		FadePlayerFOVMulTo(1.5f, 0.25f);
		return;
	}
	
	if(asTimer == "sanity"){
		//SetSwingDoorLocked("door_gallery", true, false);
		GiveSanityDamage(10, true);
		return;
	}
	
	StopSound("creaking_door", 0.4f);
}
/*When in the gallery the lights go out and the door slams shut
**When leaving the gallery the door slams shut and locks behind the player.
 */
public void CollideAreaGallery(string  asParent, string  asChild, int alState)
{
	if(asChild == "gallery_2"){
		//if(!GetSwingDoorClosed("door_gallery")) 
			//PlaySoundAtEntity("slam_door", "scare_slam_door.snt", "door_gallery", 0, false);
		//SetSwingDoorLocked("door_gallery", true, true);
		//PlaySoundAtEntity("easter?", "scare_breath.snt", "door_gallery", 1.0f / 0.5f, false);
		//AddTimer("scare2", 0.1f, "TimerStopCreak");
		return;
	}
	
	UnBlockHint("DarknessDecrease"); 
	
	StopRandomLook();	//De-activate the spinning head
	
	//if(!GetSwingDoorClosed("door_gallery")) 
		//PlaySoundAtEntity("slam_door", "scare_slam_door.snt", "door_gallery", 0, false);
		
	//SetSwingDoorClosed("door_gallery", true, true);
	//SetSwingDoorDisableAutoClose("door_gallery", false);
	
	PlayMusic("00_event_gallery", false, 1,1, 10, false);
	PlaySoundAtEntity("windgal", "general_wind_whirl.snt", "gallery_1", 0.0f, false);
	CreateParticleSystemAtEntity("psgal", "ps_dust_whirl.ps", "gallery_1", false);
	
	StartScreenShake(0.01f,0.25f, 0.01f,0.5f);
	
	AddEntityCollideCallback("Player", "gallery_2", "CollideAreaGallery", true, 1);
	
	AddTimer("sanity", 0.7f, "TimerStopCreak");
	AddTimer("lightsout", 0.8f, "TimerStopCreak");
	AddTimer("scare", 1.2f, "TimerStopCreak");
	AddTimer("breath1", 2.0f, "TimerStopCreak");
	AddTimer("breath2", 5.0f, "TimerStopCreak");
	AddTimer("breath3", 8.0f, "TimerStopCreak");
}

//---------------------------------------------	
	
////////////////////////
//GUST DOOR OPEN
////////////////////////

//---------------------------------------------

/*When entering hall 3 the door to the couch room is opened by a gust of wind
 */
public void CollideAreaGustDoor(string  asParent, string  asChild, int alState)
{
	StartScreenShake(0.003f,0.25f, 1,1);
	
	PlaySoundAtEntity("gust_door", "general_wind_whirl.snt", "door_gust", 1.0f / 3, false);
	
	CreateParticleSystemAtEntity("gust", "ps_dust_push", "AreaDust_4", false);
	
	SetSwingDoorClosed("door_gust", false, false);
	SetSwingDoorDisableAutoClose("door_gust", true);
	
	AddTimer("gusttimer", 0.4f, "TimerGustDoor");
	
	AddDebugMessage("Uhm, the door gust opened?", false);	
}
public void TimerGustDoor(string  asTimer)
{
	PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0, false);
	PlaySoundAtEntity("creaking_door", "joint_door_move_special.snt", "door_gust", 1.0f / 1.8f, false);
	
	GiveSanityDamage(5, true);
	
	AddTimer("breath1", 2.0f, "TimerStopCreak");
	AddTimer("breath2", 5.0f, "TimerStopCreak");
	AddTimer("breath3", 8.0f, "TimerStopCreak");
	
	AddTimer("door_gust", 0.3f, "TimerSwingDoor");
	
	StopSound("creaking_door", 0.4f);
}
//END GUST DOOR OPEN//
//////////////////////

//---------------------------------------------	

/////////////////////////////////
//BEGIN GUST FLOWS//
/////////////////////////////////

//---------------------------------------------	

/*Some gusts will appear in the early parts of the level and one at the very end
 */
public void CollideAreaGust(string  asParent, string  asChild, int alState)
{
	if(asChild == "AreaDust1") {
		int makeyourmagicpick = RandFloat(2,3);
		if(makeyourmagicpick == 2) AddEntityCollideCallback("Player", "AreaDust2", "CollideAreaGust", true, 1);
		else AddEntityCollideCallback("Player", "AreaDust3", "CollideAreaGust", true, 1);
	}
	
	if(asChild == "AreaDustEnd"){ 
		CreateParticleSystemAtEntity(asChild, "ps_dust_whirl", asChild+"_ps", false);
		UnBlockHint("DarknessDecrease");
	}
	else CreateParticleSystemAtEntity(asChild, "ps_dust_push", asChild+"_ps", false);
	
	PlaySoundAtEntity(asChild+"s", "general_wind_whirl.snt", asChild+"_ps", 1.0f / 0.5f, false);
}

//---------------------------------------------	

/////////////////////////////
// PLAYER ALMOST FAINT
/////////////////////////////

//---------------------------------------------	

public void CollideAreaFaint(string  asParent, string  asChild, int alState)
{
	StopRandomLook();	//De-activate the spinning head
	
	SetPlayerJumpDisabled(true);
	SetPlayerCrouchDisabled(true);
	
	AddTimer("TimerFaintEvents", 0.5f, "TimerFaintEvents");
	
	FadePlayerFOVMulTo(1.5f, 0.25f);
	
	AddDebugMessage("Oh my, I think I'm almost going to pass out, almost...", false);		
}
public void TimerFaintEvents(string  asTimer)
{	
	/*Configurables
	 */
	int iMaxEventStep = 8;		//How many steps there are in the switch event
	float fEventSpeed = 1.0f;	//The default time between steps in an event
	
	/*Helpers - Do not edit
	 */
	string sEvent = asTimer;	//Using first timer name for variable, timer name & callback for the timer that loops
	AddLocalVarInt(sEvent, 1);	//What step to play in the event
	
	switch(GetLocalVarInt(sEvent)){
		case 1:
			StartPlayerLookAt("Areafaintlook_1", 0.1f, 0.1f, "");
			SetPlayerMoveSpeedMul(0.85f);
			StartScreenShake(0.002f,1, 0.5f,0.5f);
			PlaySoundAtEntity("loop", "00_loop.snt", "Player", 1.0f / 0.1f, false);
			PlaySoundAtEntity("faint", "00_faint.snt", "Player", 1.0f / 0.08f, false);
		break;
		case 2:
			MovePlayerHeadPos(-0.25f, -0.5f, 0, 1, 0.5f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
			FadePlayerRollTo(-50, 3, 5);
			SetPlayerMoveSpeedMul(0.75f);
			StartScreenShake(0.004f,1, 0.5f,0.5f);
		break;
		case 3:
			FadeOut(3);
			MovePlayerHeadPos(0.25f, -0.3f, 0, 1, 0.5f);
			StartPlayerLookAt("Areafaintlook_2", 0.3f, 0.3f, "");
			SetPlayerMoveSpeedMul(0.7f);
			StartScreenShake(0.005f,1, 0.5f,0.5f);
		break;
		case 4:
			FadePlayerRollTo(50, 5, 5);
			SetPlayerMoveSpeedMul(0.5f);
			StartScreenShake(0.006f,1, 0.5f,0.5f);
			PlaySoundAtEntity("bodyfall", "player_bodyfall.snt", "Player", 1.0f / 0.3f, false);
		break;
		case 5:
			FadeIn(1.5f);
			MovePlayerHeadPos(0, -0.8f, 0, 0.75f, 0.5f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
			StartPlayerLookAt("Areafaintlook_2", 0.1f, 0.1f, "");
			SetPlayerMoveSpeedMul(0.3f);
		break;
		case 6:
			StopSound("faint", 0.2f);
			FadeOut(3);
			FadePlayerRollTo(-25, 3, 5);
			SetPlayerMoveSpeedMul(0.4f);
		break;
		case 7:
			StartPlayerLookAt("Areafaintlook_1", 0.2f, 0.2f, "");
			SetPlayerMoveSpeedMul(0.7f);
			FadePlayerFOVMulTo(1.0f, 0.1f);
		break;
		case 8:
			StopSound("loop", 0.2f);
			FadeIn(1.5f);
			MovePlayerHeadPos(0, 0, 0, 1, 0.5f);
			PlaySoundAtEntity("movement", "player_climb.snt", "Player", 0, false);
			FadePlayerRollTo(0, 2, 4);
			StopPlayerLookAt();
			SetPlayerMoveSpeedMul(1);
			AddTimer("lookloop", RandFloat(2.0f,6.0f), "TimerRandomLook");	//Re-activate the spinning head
			
			// Reset player input
			SetPlayerJumpDisabled(false);
			SetPlayerCrouchDisabled(false);
		break;
	}
	
	if(GetLocalVarInt(sEvent) <= iMaxEventStep) AddTimer(sEvent, fEventSpeed, sEvent);
}

//---------------------------------------------	

//////////////////////////
// RANDOM LOOK SPIN
//////////////////////////

//---------------------------------------------	

/*Player has a bit of a random head during the level
 */
bool bRoll = true;
public void TimerRandomLook(string  asTimer)
{
	int iLook = RandFloat(1,5);
	
	if(iLook > 4){
		SetPlayerRunSpeedMul(0.5f);
		SetPlayerMoveSpeedMul(1);
		FadePlayerRollTo(0, 0.5f, 1); 
		FadePlayerFOVMulTo(1, 1);
		FadeImageTrailTo(0,1.5f);
		
		AddTimer("lookloop", RandFloat(6.0f,8.0f), "TimerRandomLook");
		return;
	}
	
	if(iLook == 1 || iLook == 3)
		PlaySoundAtEntity("sigh", "react_sigh.snt", "Player", 1.0f / 0.75f, false);
	
	FadePlayerFOVMulTo(RandFloat(0.7f,1.3f), RandFloat(0.05f,0.1f));
	
	SetPlayerMoveSpeedMul(RandFloat(0.6f,0.9f));
	SetPlayerRunSpeedMul(RandFloat(0.2f,0.4f));
	
	FadeImageTrailTo(RandFloat(0.75f,1.0f),RandFloat(0.75f,1.25f));
	
	bRoll = bRoll == false ? true : false;
	
	if(bRoll)
		FadePlayerRollTo(RandFloat(4,12), RandFloat(0.075f,0.35f), RandFloat(0.55f,1.15f)); 
	else
		FadePlayerRollTo(RandFloat(-4,-12), RandFloat(0.05f,0.25f), RandFloat(0.5f,1)); 
		
	AddTimer("lookloop", RandFloat(3.0f,6.0f), "TimerRandomLook");
}
public void StopRandomLook()
{
	RemoveTimer("lookloop");
	
	SetPlayerMoveSpeedMul(1);
	FadePlayerFOVMulTo(1, 1);
	FadeImageTrailTo(0,1.0f);
	FadePlayerRollTo(0, 0.5f, 2); 
}

//---------------------------------------------	

/////////////////////////////
// INTERACT LARGE GATE
/////////////////////////////

//---------------------------------------------	

public void InteractLargeGate(string  asEntity)
{
	PlaySoundAtEntity("guardboo", "guardian_distant1", "AreaLargeGate", 3.0f, false);
	PlaySoundAtEntity("thunderboo", "general_thunder.snt", "AreaLargeGate", 0.0f, false);
	
	PlayGuiSound("close_gate.ogg", 0.4f);
	
	AddTimer("1", 0.4f, "TimerLargeGate");
	AddTimer("2", 0.1f, "TimerLargeGate");
	AddTimer("3", 0.6f, "TimerLargeGate");
	AddTimer("4", 2.0f, "TimerLargeGate");
}
public void TimerLargeGate(string  asTimer)
{
	if(asTimer == "1"){
		PlayMusic("01_event_critters", false, 0.7f, 1.0f, 10, false);
		StartScreenShake(0.005f, 2.0f, 1.0f, 1.0f);
		GiveSanityDamage(10.0f, true);
	}
	else if(asTimer == "2"){
		
	}
	else if(asTimer == "3"){
		PlayGuiSound("react_scare", 0.5f);
	}
	else if(asTimer == "4"){
		StopMusic(6.0f, 10);
		StopSound("guardboo", 2.0f);
		StopSound("thunderboo", 4.0f);
		
		SetEntityPlayerInteractCallback("LargeGate", "InteractLargeGate02", true);
	}
}
public void InteractLargeGate02(string  asEntity)
{
	
}

//---------------------------------------------	

/////////////////////////////
// MISC
/////////////////////////////

//---------------------------------------------	


public void LookAtQuest(string  asEntity, int alState)
{
	AddQuest("00Trail","00Trail");
}
public void CollideAreaQuest(string  asParent, string  asChild, int alState)
{
	AddQuest("00Trail","00Trail");
}

//////////////////////
////////////////////////////
// Run first time starting map
public override void OnStart()
{
	SetMapDisplayNameEntry("RainyHall");
	
	//----COLLIDE CALLBACKS----//
	AddEntityCollideCallback("Player", "areanarrowhalls", "CollideNarrowHalls", false, 0);		//Begin/End narrowhall effects
	AddEntityCollideCallback("Player", "areaendthunder", "CollideAreaThunder", true, 1);		//Begin/End thunder loop
	AddEntityCollideCallback("Player", "narrowpassout", "CollideAreaNarrowPassOut", true, 1);	//Player passes out in hallway
	AddEntityCollideCallback("Player", "gallerydoor", "CollideAreaGalleryDoor", true, 1);	//Gallery door creaks open
	AddEntityCollideCallback("Player", "gallery", "CollideAreaGallery", true, 1);			//Gallery door slam behind player
	AddEntityCollideCallback("Player", "areagustdoor", "CollideAreaGustDoor", true, 1);		//Gust swings door open in hall 3
	AddEntityCollideCallback("Player", "AreaDust1", "CollideAreaGust", true, -1);		//Area that starts a gust blow in hallX
	AddEntityCollideCallback("Player", "AreaDustEnd", "CollideAreaGust", true, -1);		//Area that starts a gust just at the end
	AddEntityCollideCallback("Player", "AreaFaint", "CollideAreaFaint", true, 1);		//Player almost faints in hall 2
	AddEntityCollideCallback("Player", "AreaQuest", "CollideAreaQuest", true, -1);		//Player almost faints in hall 2

	//---- VARIABLES ----//

	
	//---- INTRO SETTINGS ----//

	
	//FadePlayerFOVMulTo(2, 1);	//2 == max? > 1 �r fisheye.	fov,speed
	//FadePlayerAspectMulTo(2, 0.1f); // > 1 �r compressed vy. aspect, speed
	if(ScriptDebugOn() == false)
	{
		FadeGlobalSoundVolume(0, 0.01f);
		FadeGlobalSoundSpeed(0.7f, 0.01f);
		SetPlayerActive(false);
		ShowPlayerCrossHairIcons(false);
		SetInventoryDisabled(true);
		SetPlayerJumpDisabled(true);
		SetPlayerCrouchDisabled(true);
		FadeOut(0);
		
		//disable some event areas
		SetEntityActive("narrowpassout", false);
		SetEntityActive("gallerydoor", false);
		SetEntityActive("areanarrowhalls", false);
		SetEntityActive("areaendthunder", false);
		SetEntityActive("areabeginthunder", false);
		SetEntityActive("areagustdoor", false);
		SetEntityActive("AreaDust3", false);
		SetEntityActive("AreaFaint", false);
		
		//Teleport player to first start pos
		TeleportPlayer("IntroStart_1");
	
		AddTimer("StartGame", 1, "TimerStartGame");
	}
		
	//----QUEST INIT----//
	SetPlayerLampOil(0);	//This is so that the player has 0 lampoil when find the lantern in level 1.
	SetNumberOfQuestsInMap(0);	
	
	BlockHint("SanityAdd"); BlockHint("LanternNoItem"); BlockHint("LanternNoOil"); BlockHint("LeanHint");
	BlockHint("PushHint"); BlockHint("ThrowHint"); BlockHint("EntityGrab02"); BlockHint("EntityPush");
	BlockHint("EntitySlide"); BlockHint("EntityLever"); BlockHint("DarknessDecrease"); BlockHint("SanityHit");

	//SetPlayerSanity(70);
	
	//----INSANITY----//
	SetInsanitySetEnabled("Ch02", false); 
	SetInsanitySetEnabled("Ch03", false);

	
	//----DEBUG----//
	if(ScriptDebugOn())
	{
		AddDebugMessage("Intro disabled as debug is active.", false);
		
		//GiveItemFromFile("lantern", "lantern.ent");
		
		//LEVEL TESTING
		AddTimer("lookloop", RandFloat(2.0f,6.0f), "TimerRandomLook");
		AddTimer("thunder", 1, "TimerThunder");
		SetPlayerRunSpeedMul(0);
	}
}

////////////////////////////
// Run when entering map
public override void OnEnter()
{
    Finder.Bufferize("0", "", "Flashbacks", "IntroSequence", "TimerIntroSequence", "IntroSequenceState", "CH01L00_DanielsMind01_0", "CH01L00_DanielsMind01_1", "CH01L00_DanielsMind01_2", "CH01L00_DanielsMind01_3", "CH01L00_DanielsMind01_4", "CH01L00_DanielsMind01_5", "CH01L00_DanielsMind01_6", "CH01L00_DanielsMind01_7", "CH01L00_DanielsMind01_8", "CH01L00_DanielsMind01_9", "CH01L00_DanielsMind01_10", "CH01L00_DanielsMind01_11", "CH01L00_DanielsMind01_12", "CH01L00_DanielsMind01_13", "CH01L00_DanielsMind01_14", "CH01L00_DanielsMind01_15", "CH01L00_DanielsMind01_16", "CH01L00_DanielsMind01_17", "CH01L00_DanielsMind01_18", "CH01L00_DanielsMind01_19", "CH01L00_DanielsMind01_20", "CH01L00_DanielsMind01_21", "CH01L00_DanielsMind01_22", "CH01L00_DanielsMind01_23", "CH01L00_DanielsMind01_24", "CH01L00_DanielsMind01_25", "CH01L00_DanielsMind01_26", "CH01L00_DanielsMind01_27", "CH01L00_DanielsMind01_28", "CH01L00_DanielsMind01_29", "CH01L00_DanielsMind01_30", "CH01L00_DanielsMind01_31", "CH01L00_DanielsMind01_32", "CH01L00_DanielsMind01_33", "CH01L00_DanielsMind01_34", "CH01L00_DanielsMind01_35", "CH01L00_DanielsMind01_36", "CH01L00_DanielsMind01_37", "CH01L00_DanielsMind01_38", "CH01L00_DanielsMind01_39", "CH01L00_DanielsMind01_40", "CH01L00_DanielsMind01_41", "CH01L00_DanielsMind01_42", "CH01L00_DanielsMind01_43", "CH01L00_DanielsMind01_44", "CH01L00_DanielsMind01_45", "CH01L00_DanielsMind01_46", "CH01L00_DanielsMind01_47", "CH01L00_DanielsMind01_48", "CH01L00_DanielsMind01_49", "CH01L00_DanielsMind01_50", "CH01L00_DanielsMind01_51", "CH01L00_DanielsMind01_52", "CH01L00_DanielsMind01_53", "CH01L00_DanielsMind01_54", "CH01L00_DanielsMind01_55", "CH01L00_DanielsMind01_56", "CH01L00_DanielsMind01_57", "CH01L00_DanielsMind01_58", "CH01L00_DanielsMind01_59", "CH01L00_DanielsMind01_60", "CH01L00_DanielsMind01_61", "CH01L00_DanielsMind01_62", "CH01L00_DanielsMind01_63", "CH01L00_DanielsMind01_64", "CH01L00_DanielsMind01_65", "CH01L00_DanielsMind01_66", "CH01L00_DanielsMind01_67", "CH01L00_DanielsMind01_68", "CH01L00_DanielsMind01_69", "CH01L00_DanielsMind01_70", "CH01L00_DanielsMind01_71", "CH01L00_DanielsMind01_72", "CH01L00_DanielsMind01_73", "CH01L00_DanielsMind01_74", "CH01L00_DanielsMind01_75", "CH01L00_DanielsMind01_76", "CH01L00_DanielsMind01_77", "CH01L00_DanielsMind01_78", "CH01L00_DanielsMind01_79", "CH01L00_DanielsMind01_80", "CH01L00_DanielsMind01_81", "CH01L00_DanielsMind01_82", "CH01L00_DanielsMind01_83", "CH01L00_DanielsMind01_84", "CH01L00_DanielsMind01_85", "CH01L00_DanielsMind01_86", "CH01L00_DanielsMind01_87", "CH01L00_DanielsMind01_88", "CH01L00_DanielsMind01_89", "CH01L00_DanielsMind01_90", "CH01L00_DanielsMind01_91", "CH01L00_DanielsMind01_92", "CH01L00_DanielsMind01_93", "CH01L00_DanielsMind01_94", "CH01L00_DanielsMind01_95", "CH01L00_DanielsMind01_96", "CH01L00_DanielsMind01_97", "CH01L00_DanielsMind01_98", "CH01L00_DanielsMind01_99", "IntroSequenceVoiceOver", "CH01L00_DanielsMind02_0", "CH01L00_DanielsMind02_1", "CH01L00_DanielsMind02_2", "CH01L00_DanielsMind02_3", "CH01L00_DanielsMind02_4", "CH01L00_DanielsMind02_5", "CH01L00_DanielsMind02_6", "CH01L00_DanielsMind02_7", "CH01L00_DanielsMind02_8", "CH01L00_DanielsMind02_9", "CH01L00_DanielsMind02_10", "CH01L00_DanielsMind02_11", "CH01L00_DanielsMind02_12", "CH01L00_DanielsMind02_13", "CH01L00_DanielsMind02_14", "CH01L00_DanielsMind02_15", "CH01L00_DanielsMind02_16", "CH01L00_DanielsMind02_17", "CH01L00_DanielsMind02_18", "CH01L00_DanielsMind02_19", "CH01L00_DanielsMind02_20", "CH01L00_DanielsMind02_21", "CH01L00_DanielsMind02_22", "CH01L00_DanielsMind02_23", "CH01L00_DanielsMind02_24", "CH01L00_DanielsMind02_25", "CH01L00_DanielsMind02_26", "CH01L00_DanielsMind02_27", "CH01L00_DanielsMind02_28", "CH01L00_DanielsMind02_29", "CH01L00_DanielsMind02_30", "CH01L00_DanielsMind02_31", "CH01L00_DanielsMind02_32", "CH01L00_DanielsMind02_33", "CH01L00_DanielsMind02_34", "CH01L00_DanielsMind02_35", "CH01L00_DanielsMind02_36", "CH01L00_DanielsMind02_37", "CH01L00_DanielsMind02_38", "CH01L00_DanielsMind02_39", "CH01L00_DanielsMind02_40", "CH01L00_DanielsMind02_41", "CH01L00_DanielsMind02_42", "CH01L00_DanielsMind02_43", "CH01L00_DanielsMind02_44", "CH01L00_DanielsMind02_45", "CH01L00_DanielsMind02_46", "CH01L00_DanielsMind02_47", "CH01L00_DanielsMind02_48", "CH01L00_DanielsMind02_49", "CH01L00_DanielsMind02_50", "CH01L00_DanielsMind02_51", "CH01L00_DanielsMind02_52", "CH01L00_DanielsMind02_53", "CH01L00_DanielsMind02_54", "CH01L00_DanielsMind02_55", "CH01L00_DanielsMind02_56", "CH01L00_DanielsMind02_57", "CH01L00_DanielsMind02_58", "CH01L00_DanielsMind02_59", "CH01L00_DanielsMind02_60", "CH01L00_DanielsMind02_61", "CH01L00_DanielsMind02_62", "CH01L00_DanielsMind02_63", "CH01L00_DanielsMind02_64", "CH01L00_DanielsMind02_65", "CH01L00_DanielsMind02_66", "CH01L00_DanielsMind02_67", "CH01L00_DanielsMind02_68", "CH01L00_DanielsMind02_69", "CH01L00_DanielsMind02_70", "CH01L00_DanielsMind02_71", "CH01L00_DanielsMind02_72", "CH01L00_DanielsMind02_73", "CH01L00_DanielsMind02_74", "CH01L00_DanielsMind02_75", "CH01L00_DanielsMind02_76", "CH01L00_DanielsMind02_77", "CH01L00_DanielsMind02_78", "CH01L00_DanielsMind02_79", "CH01L00_DanielsMind02_80", "CH01L00_DanielsMind02_81", "CH01L00_DanielsMind02_82", "CH01L00_DanielsMind02_83", "CH01L00_DanielsMind02_84", "CH01L00_DanielsMind02_85", "CH01L00_DanielsMind02_86", "CH01L00_DanielsMind02_87", "CH01L00_DanielsMind02_88", "CH01L00_DanielsMind02_89", "CH01L00_DanielsMind02_90", "CH01L00_DanielsMind02_91", "CH01L00_DanielsMind02_92", "CH01L00_DanielsMind02_93", "CH01L00_DanielsMind02_94", "CH01L00_DanielsMind02_95", "CH01L00_DanielsMind02_96", "CH01L00_DanielsMind02_97", "CH01L00_DanielsMind02_98", "CH01L00_DanielsMind02_99", "CH01L00_DanielsMind03_0", "CH01L00_DanielsMind03_1", "CH01L00_DanielsMind03_2", "CH01L00_DanielsMind03_3", "CH01L00_DanielsMind03_4", "CH01L00_DanielsMind03_5", "CH01L00_DanielsMind03_6", "CH01L00_DanielsMind03_7", "CH01L00_DanielsMind03_8", "CH01L00_DanielsMind03_9", "CH01L00_DanielsMind03_10", "CH01L00_DanielsMind03_11", "CH01L00_DanielsMind03_12", "CH01L00_DanielsMind03_13", "CH01L00_DanielsMind03_14", "CH01L00_DanielsMind03_15", "CH01L00_DanielsMind03_16", "CH01L00_DanielsMind03_17", "CH01L00_DanielsMind03_18", "CH01L00_DanielsMind03_19", "CH01L00_DanielsMind03_20", "CH01L00_DanielsMind03_21", "CH01L00_DanielsMind03_22", "CH01L00_DanielsMind03_23", "CH01L00_DanielsMind03_24", "CH01L00_DanielsMind03_25", "CH01L00_DanielsMind03_26", "CH01L00_DanielsMind03_27", "CH01L00_DanielsMind03_28", "CH01L00_DanielsMind03_29", "CH01L00_DanielsMind03_30", "CH01L00_DanielsMind03_31", "CH01L00_DanielsMind03_32", "CH01L00_DanielsMind03_33", "CH01L00_DanielsMind03_34", "CH01L00_DanielsMind03_35", "CH01L00_DanielsMind03_36", "CH01L00_DanielsMind03_37", "CH01L00_DanielsMind03_38", "CH01L00_DanielsMind03_39", "CH01L00_DanielsMind03_40", "CH01L00_DanielsMind03_41", "CH01L00_DanielsMind03_42", "CH01L00_DanielsMind03_43", "CH01L00_DanielsMind03_44", "CH01L00_DanielsMind03_45", "CH01L00_DanielsMind03_46", "CH01L00_DanielsMind03_47", "CH01L00_DanielsMind03_48", "CH01L00_DanielsMind03_49", "CH01L00_DanielsMind03_50", "CH01L00_DanielsMind03_51", "CH01L00_DanielsMind03_52", "CH01L00_DanielsMind03_53", "CH01L00_DanielsMind03_54", "CH01L00_DanielsMind03_55", "CH01L00_DanielsMind03_56", "CH01L00_DanielsMind03_57", "CH01L00_DanielsMind03_58", "CH01L00_DanielsMind03_59", "CH01L00_DanielsMind03_60", "CH01L00_DanielsMind03_61", "CH01L00_DanielsMind03_62", "CH01L00_DanielsMind03_63", "CH01L00_DanielsMind03_64", "CH01L00_DanielsMind03_65", "CH01L00_DanielsMind03_66", "CH01L00_DanielsMind03_67", "CH01L00_DanielsMind03_68", "CH01L00_DanielsMind03_69", "CH01L00_DanielsMind03_70", "CH01L00_DanielsMind03_71", "CH01L00_DanielsMind03_72", "CH01L00_DanielsMind03_73", "CH01L00_DanielsMind03_74", "CH01L00_DanielsMind03_75", "CH01L00_DanielsMind03_76", "CH01L00_DanielsMind03_77", "CH01L00_DanielsMind03_78", "CH01L00_DanielsMind03_79", "CH01L00_DanielsMind03_80", "CH01L00_DanielsMind03_81", "CH01L00_DanielsMind03_82", "CH01L00_DanielsMind03_83", "CH01L00_DanielsMind03_84", "CH01L00_DanielsMind03_85", "CH01L00_DanielsMind03_86", "CH01L00_DanielsMind03_87", "CH01L00_DanielsMind03_88", "CH01L00_DanielsMind03_89", "CH01L00_DanielsMind03_90", "CH01L00_DanielsMind03_91", "CH01L00_DanielsMind03_92", "CH01L00_DanielsMind03_93", "CH01L00_DanielsMind03_94", "CH01L00_DanielsMind03_95", "CH01L00_DanielsMind03_96", "CH01L00_DanielsMind03_97", "CH01L00_DanielsMind03_98", "CH01L00_DanielsMind03_99", "CH01L00_DanielsMind04_0", "CH01L00_DanielsMind04_1", "CH01L00_DanielsMind04_2", "CH01L00_DanielsMind04_3", "CH01L00_DanielsMind04_4", "CH01L00_DanielsMind04_5", "CH01L00_DanielsMind04_6", "CH01L00_DanielsMind04_7", "CH01L00_DanielsMind04_8", "CH01L00_DanielsMind04_9", "CH01L00_DanielsMind04_10", "CH01L00_DanielsMind04_11", "CH01L00_DanielsMind04_12", "CH01L00_DanielsMind04_13", "CH01L00_DanielsMind04_14", "CH01L00_DanielsMind04_15", "CH01L00_DanielsMind04_16", "CH01L00_DanielsMind04_17", "CH01L00_DanielsMind04_18", "CH01L00_DanielsMind04_19", "CH01L00_DanielsMind04_20", "CH01L00_DanielsMind04_21", "CH01L00_DanielsMind04_22", "CH01L00_DanielsMind04_23", "CH01L00_DanielsMind04_24", "CH01L00_DanielsMind04_25", "CH01L00_DanielsMind04_26", "CH01L00_DanielsMind04_27", "CH01L00_DanielsMind04_28", "CH01L00_DanielsMind04_29", "CH01L00_DanielsMind04_30", "CH01L00_DanielsMind04_31", "CH01L00_DanielsMind04_32", "CH01L00_DanielsMind04_33", "CH01L00_DanielsMind04_34", "CH01L00_DanielsMind04_35", "CH01L00_DanielsMind04_36", "CH01L00_DanielsMind04_37", "CH01L00_DanielsMind04_38", "CH01L00_DanielsMind04_39", "CH01L00_DanielsMind04_40", "CH01L00_DanielsMind04_41", "CH01L00_DanielsMind04_42", "CH01L00_DanielsMind04_43", "CH01L00_DanielsMind04_44", "CH01L00_DanielsMind04_45", "CH01L00_DanielsMind04_46", "CH01L00_DanielsMind04_47", "CH01L00_DanielsMind04_48", "CH01L00_DanielsMind04_49", "CH01L00_DanielsMind04_50", "CH01L00_DanielsMind04_51", "CH01L00_DanielsMind04_52", "CH01L00_DanielsMind04_53", "CH01L00_DanielsMind04_54", "CH01L00_DanielsMind04_55", "CH01L00_DanielsMind04_56", "CH01L00_DanielsMind04_57", "CH01L00_DanielsMind04_58", "CH01L00_DanielsMind04_59", "CH01L00_DanielsMind04_60", "CH01L00_DanielsMind04_61", "CH01L00_DanielsMind04_62", "CH01L00_DanielsMind04_63", "CH01L00_DanielsMind04_64", "CH01L00_DanielsMind04_65", "CH01L00_DanielsMind04_66", "CH01L00_DanielsMind04_67", "CH01L00_DanielsMind04_68", "CH01L00_DanielsMind04_69", "CH01L00_DanielsMind04_70", "CH01L00_DanielsMind04_71", "CH01L00_DanielsMind04_72", "CH01L00_DanielsMind04_73", "CH01L00_DanielsMind04_74", "CH01L00_DanielsMind04_75", "CH01L00_DanielsMind04_76", "CH01L00_DanielsMind04_77", "CH01L00_DanielsMind04_78", "CH01L00_DanielsMind04_79", "CH01L00_DanielsMind04_80", "CH01L00_DanielsMind04_81", "CH01L00_DanielsMind04_82", "CH01L00_DanielsMind04_83", "CH01L00_DanielsMind04_84", "CH01L00_DanielsMind04_85", "CH01L00_DanielsMind04_86", "CH01L00_DanielsMind04_87", "CH01L00_DanielsMind04_88", "CH01L00_DanielsMind04_89", "CH01L00_DanielsMind04_90", "CH01L00_DanielsMind04_91", "CH01L00_DanielsMind04_92", "CH01L00_DanielsMind04_93", "CH01L00_DanielsMind04_94", "CH01L00_DanielsMind04_95", "CH01L00_DanielsMind04_96", "CH01L00_DanielsMind04_97", "CH01L00_DanielsMind04_98", "CH01L00_DanielsMind04_99", "IntroStart_2", "IntroStart_3", "IntroStart_4", "WakeUpPlayer", "TimerWakeUpPlayer", "activefix", "TimerActiveFix", "Event:", " Time:", "PlayerStartArea_1", "narrowpassout", "gallerydoor", "areanarrowhalls", "areaendthunder", "areabeginthunder", "areagustdoor", "AreaDust3", "AreaFaint", "Arealook1", "blackout", "TimerBlackOut", "forhelbeep", "TimerBeginSoundsIntro", "CH01L00_DanielsMind01_01", "CH01L00_DanielsMind02_01", "CH01L00_DanielsMind03_01", "CH01L00_DanielsMind04_01", "VoiceOverIntro", "Rain", "AreaThunder", "Eerie", "Hollow", "Flow", "AreaFlowWater", "BlackoutStep", "Arealook2", "rose", "TimerRose", "Arealook3", "thunder", "TimerThunder", "sigh", "Player", "Arealook4", "movement", "lookloop", "TimerRandomLook", "AreaQuest", "00Trail", "AreaRose", "ThunderStep", "Thunder", "back to normal", "spotthunder_0", "spotthunder_1", "spotthunder_2", "spotthunder_3", "spotthunder_4", "spotthunder_5", "spotthunder_6", "spotthunder_7", "spotthunder_8", "spotthunder_9", "spotthunder_10", "spotthunder_11", "spotthunder_12", "spotthunder_13", "spotthunder_14", "spotthunder_15", "spotthunder_16", "spotthunder_17", "spotthunder_18", "spotthunder_19", "spotthunder_20", "spotthunder_21", "spotthunder_22", "spotthunder_23", "spotthunder_24", "spotthunder_25", "spotthunder_26", "spotthunder_27", "spotthunder_28", "spotthunder_29", "spotthunder_30", "spotthunder_31", "spotthunder_32", "spotthunder_33", "spotthunder_34", "spotthunder_35", "spotthunder_36", "spotthunder_37", "spotthunder_38", "spotthunder_39", "spotthunder_40", "spotthunder_41", "spotthunder_42", "spotthunder_43", "spotthunder_44", "spotthunder_45", "spotthunder_46", "spotthunder_47", "spotthunder_48", "spotthunder_49", "spotthunder_50", "spotthunder_51", "spotthunder_52", "spotthunder_53", "spotthunder_54", "spotthunder_55", "spotthunder_56", "spotthunder_57", "spotthunder_58", "spotthunder_59", "spotthunder_60", "spotthunder_61", "spotthunder_62", "spotthunder_63", "spotthunder_64", "spotthunder_65", "spotthunder_66", "spotthunder_67", "spotthunder_68", "spotthunder_69", "spotthunder_70", "spotthunder_71", "spotthunder_72", "spotthunder_73", "spotthunder_74", "spotthunder_75", "spotthunder_76", "spotthunder_77", "spotthunder_78", "spotthunder_79", "spotthunder_80", "spotthunder_81", "spotthunder_82", "spotthunder_83", "spotthunder_84", "spotthunder_85", "spotthunder_86", "spotthunder_87", "spotthunder_88", "spotthunder_89", "spotthunder_90", "spotthunder_91", "spotthunder_92", "spotthunder_93", "spotthunder_94", "spotthunder_95", "spotthunder_96", "spotthunder_97", "spotthunder_98", "spotthunder_99", "pointthunder_0", "pointthunder_1", "pointthunder_2", "pointthunder_3", "pointthunder_4", "pointthunder_5", "pointthunder_6", "pointthunder_7", "pointthunder_8", "pointthunder_9", "pointthunder_10", "pointthunder_11", "pointthunder_12", "pointthunder_13", "pointthunder_14", "pointthunder_15", "pointthunder_16", "pointthunder_17", "pointthunder_18", "pointthunder_19", "pointthunder_20", "pointthunder_21", "pointthunder_22", "pointthunder_23", "pointthunder_24", "pointthunder_25", "pointthunder_26", "pointthunder_27", "pointthunder_28", "pointthunder_29", "pointthunder_30", "pointthunder_31", "pointthunder_32", "pointthunder_33", "pointthunder_34", "pointthunder_35", "pointthunder_36", "pointthunder_37", "pointthunder_38", "pointthunder_39", "pointthunder_40", "pointthunder_41", "pointthunder_42", "pointthunder_43", "pointthunder_44", "pointthunder_45", "pointthunder_46", "pointthunder_47", "pointthunder_48", "pointthunder_49", "pointthunder_50", "pointthunder_51", "pointthunder_52", "pointthunder_53", "pointthunder_54", "pointthunder_55", "pointthunder_56", "pointthunder_57", "pointthunder_58", "pointthunder_59", "pointthunder_60", "pointthunder_61", "pointthunder_62", "pointthunder_63", "pointthunder_64", "pointthunder_65", "pointthunder_66", "pointthunder_67", "pointthunder_68", "pointthunder_69", "pointthunder_70", "pointthunder_71", "pointthunder_72", "pointthunder_73", "pointthunder_74", "pointthunder_75", "pointthunder_76", "pointthunder_77", "pointthunder_78", "pointthunder_79", "pointthunder_80", "pointthunder_81", "pointthunder_82", "pointthunder_83", "pointthunder_84", "pointthunder_85", "pointthunder_86", "pointthunder_87", "pointthunder_88", "pointthunder_89", "pointthunder_90", "pointthunder_91", "pointthunder_92", "pointthunder_93", "pointthunder_94", "pointthunder_95", "pointthunder_96", "pointthunder_97", "pointthunder_98", "pointthunder_99", "ambthunder_0", "ambthunder_1", "ambthunder_2", "ambthunder_3", "ambthunder_4", "ambthunder_5", "ambthunder_6", "ambthunder_7", "ambthunder_8", "ambthunder_9", "ambthunder_10", "ambthunder_11", "ambthunder_12", "ambthunder_13", "ambthunder_14", "ambthunder_15", "ambthunder_16", "ambthunder_17", "ambthunder_18", "ambthunder_19", "ambthunder_20", "ambthunder_21", "ambthunder_22", "ambthunder_23", "ambthunder_24", "ambthunder_25", "ambthunder_26", "ambthunder_27", "ambthunder_28", "ambthunder_29", "ambthunder_30", "ambthunder_31", "ambthunder_32", "ambthunder_33", "ambthunder_34", "ambthunder_35", "ambthunder_36", "ambthunder_37", "ambthunder_38", "ambthunder_39", "ambthunder_40", "ambthunder_41", "ambthunder_42", "ambthunder_43", "ambthunder_44", "ambthunder_45", "ambthunder_46", "ambthunder_47", "ambthunder_48", "ambthunder_49", "ambthunder_50", "ambthunder_51", "ambthunder_52", "ambthunder_53", "ambthunder_54", "ambthunder_55", "ambthunder_56", "ambthunder_57", "ambthunder_58", "ambthunder_59", "ambthunder_60", "ambthunder_61", "ambthunder_62", "ambthunder_63", "ambthunder_64", "ambthunder_65", "ambthunder_66", "ambthunder_67", "ambthunder_68", "ambthunder_69", "ambthunder_70", "ambthunder_71", "ambthunder_72", "ambthunder_73", "ambthunder_74", "ambthunder_75", "ambthunder_76", "ambthunder_77", "ambthunder_78", "ambthunder_79", "ambthunder_80", "ambthunder_81", "ambthunder_82", "ambthunder_83", "ambthunder_84", "ambthunder_85", "ambthunder_86", "ambthunder_87", "ambthunder_88", "ambthunder_89", "ambthunder_90", "ambthunder_91", "ambthunder_92", "ambthunder_93", "ambthunder_94", "ambthunder_95", "ambthunder_96", "ambthunder_97", "ambthunder_98", "ambthunder_99", "CollideAreaThunder", "creak", "AreaHallPS_0", "AreaHallPS_1", "AreaHallPS_2", "AreaHallPS_3", "AreaHallPS_4", "AreaHallPS_5", "AreaHallPS_6", "AreaHallPS_7", "AreaHallPS_8", "AreaHallPS_9", "AreaHallPS_10", "AreaHallPS_11", "AreaHallPS_12", "AreaHallPS_13", "AreaHallPS_14", "AreaHallPS_15", "AreaHallPS_16", "AreaHallPS_17", "AreaHallPS_18", "AreaHallPS_19", "AreaHallPS_20", "AreaHallPS_21", "AreaHallPS_22", "AreaHallPS_23", "AreaHallPS_24", "AreaHallPS_25", "AreaHallPS_26", "AreaHallPS_27", "AreaHallPS_28", "AreaHallPS_29", "AreaHallPS_30", "AreaHallPS_31", "AreaHallPS_32", "AreaHallPS_33", "AreaHallPS_34", "AreaHallPS_35", "AreaHallPS_36", "AreaHallPS_37", "AreaHallPS_38", "AreaHallPS_39", "AreaHallPS_40", "AreaHallPS_41", "AreaHallPS_42", "AreaHallPS_43", "AreaHallPS_44", "AreaHallPS_45", "AreaHallPS_46", "AreaHallPS_47", "AreaHallPS_48", "AreaHallPS_49", "AreaHallPS_50", "AreaHallPS_51", "AreaHallPS_52", "AreaHallPS_53", "AreaHallPS_54", "AreaHallPS_55", "AreaHallPS_56", "AreaHallPS_57", "AreaHallPS_58", "AreaHallPS_59", "AreaHallPS_60", "AreaHallPS_61", "AreaHallPS_62", "AreaHallPS_63", "AreaHallPS_64", "AreaHallPS_65", "AreaHallPS_66", "AreaHallPS_67", "AreaHallPS_68", "AreaHallPS_69", "AreaHallPS_70", "AreaHallPS_71", "AreaHallPS_72", "AreaHallPS_73", "AreaHallPS_74", "AreaHallPS_75", "AreaHallPS_76", "AreaHallPS_77", "AreaHallPS_78", "AreaHallPS_79", "AreaHallPS_80", "AreaHallPS_81", "AreaHallPS_82", "AreaHallPS_83", "AreaHallPS_84", "AreaHallPS_85", "AreaHallPS_86", "AreaHallPS_87", "AreaHallPS_88", "AreaHallPS_89", "AreaHallPS_90", "AreaHallPS_91", "AreaHallPS_92", "AreaHallPS_93", "AreaHallPS_94", "AreaHallPS_95", "AreaHallPS_96", "AreaHallPS_97", "AreaHallPS_98", "AreaHallPS_99", "ps_dust_falling_narrow", "hallway", "HallwayEvents", "HallwaySteps", "steps", "HallwayStep", "InitUniqueRandom", "step", "Arealookhall3", "Step!", "chand1_Body_3", "World", "chand2_Body_3", "chandelier_simple_3_Body_3", "breath", "scare", "cuts", "Arealookhall3_1", "narrowpass", "NarrowPassOutEvents", "HallPassOutStep", "00_event_hallway", "Arealookhall1", "loop", "Arealookhall2", "bodyfall", "faint", "inactive", "TimerPlayerInactive", "narrowpass2", "NarrowPassOutEvents02", "Pant!", "door_gallery", "creaking_door", "TimerSwingDoor", "stopcreak", "TimerStopCreak", "SwingDoor", "Swing: ", "scare2", "laugh", "breath1", "breath2", "breath3", "lightsout", "chandelier_nice_1", "candlestick_floor_2", "PointLight_25", "sanity", "gallery_2", "slam_door", "easter?", "DarknessDecrease", "00_event_gallery", "windgal", "gallery_1", "psgal", "CollideAreaGallery", "gust_door", "door_gust", "gust", "ps_dust_push", "AreaDust_4", "gusttimer", "TimerGustDoor", "Uhm, the door gust opened?", "AreaDust1", "AreaDust2", "CollideAreaGust", "AreaDustEnd", "ps_dust_whirl", "_ps", "s", "TimerFaintEvents", "Areafaintlook_1", "Areafaintlook_2", "guardboo", "guardian_distant1", "AreaLargeGate", "thunderboo", "1", "TimerLargeGate", "2", "3", "4", "01_event_critters", "react_scare", "LargeGate", "InteractLargeGate02", "RainyHall", "CollideNarrowHalls", "CollideAreaNarrowPassOut", "CollideAreaGalleryDoor", "gallery", "CollideAreaGustDoor", "CollideAreaFaint", "CollideAreaQuest", "IntroStart_1", "StartGame", "TimerStartGame", "SanityAdd", "LanternNoItem", "LanternNoOil", "LeanHint", "PushHint", "ThrowHint", "EntityGrab02", "EntityPush", "EntitySlide", "EntityLever", "SanityHit", "Ch02", "Ch03", "lantern", "react_sigh", "react_breath", "player_climb", "general_thunder", "00_creak", "scare_walk_hallway", "00_cuts", "00_loop", "player_bodyfall", "00_faint", "scare_slam_door", "ambience_wind_eerie_no3d", "ambience_hollow_tinker", "react_pant", "joint_door_move_special", "general_wind_whirl", "00_laugh", "general_rain_m", "LoadingText", "Ch01_Beginning");
    FakeDatabase.FindMusic("01_event_critters");
    FakeDatabase.FindMusic("00_event_gallery");
    FakeDatabase.FindMusic("00_event_hallway");
    FakeDatabase.FindMusic("23_amb.ogg");	
	//----PRELOADING----//
	PreloadSound("react_sigh"); PreloadSound("react_breath"); PreloadSound("player_climb"); PreloadSound("general_thunder");
	PreloadSound("00_creak"); PreloadSound("scare_walk_hallway"); PreloadSound("00_cuts"); PreloadSound("react_scare");
	PreloadSound("00_loop"); PreloadSound("player_bodyfall"); PreloadSound("00_faint"); PreloadSound("scare_slam_door");
	PreloadSound("ambience_wind_eerie_no3d"); PreloadSound("ambience_hollow_tinker"); PreloadSound("react_pant"); PreloadSound("joint_door_move_special");
	PreloadSound("general_wind_whirl"); PreloadSound("00_laugh"); PreloadSound("general_rain_m");
	  
	PreloadParticleSystem("ps_dust_falling_narrow"); PreloadParticleSystem("ps_dust_push"); PreloadParticleSystem("ps_dust_whirl");
}

////////////////////////////
// Run when leaving map
public override void OnLeave()
{
	//////////////////////
	//Load Screen Setup
	SetupLoadScreen("LoadingText", "Ch01_Beginning", 5, "game_loading_rose.jpg");
}
}
