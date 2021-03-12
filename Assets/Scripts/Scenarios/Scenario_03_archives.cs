using UnityEngine;
using System.Collections;

public class Scenario_03_archives : Scenario {
private void Start() {}
/////////////////////////
//BEGIN DIARY FLASHBACK//
/*Generates random numbers that are only used 1 time
 */
int iLength;		//Length of array
int iEnd;			//Stop the random number search, incase time loop is ran 1 time too many
int iRand;			//Temporary random number
int[] aRand;		//Array to store the requested length of numbers
int iUniqueRand;	//The unique random int returned
int UniqueRandom(int iMin, int iMax, bool bRepeat)
{
	if(GetLocalVarInt("InitUniqueRandom") == 0){
		iLength = iMax-iMin+1;
		iEnd = iLength;
		
		aRand = new int[iLength];
		
		for(int i=0;i<iLength;i++) {aRand[i] = iMin; iMin++;}
		
		SetLocalVarInt("InitUniqueRandom", 1);
	}	
	
	if(iEnd == 0){ 
		if(bRepeat){  
			SetLocalVarInt("InitUniqueRandom", 0);
			return RandFloat(iMin,iMax);
			
		} else return 0;	
	}
	
	for(;;){
		iRand = RandFloat(0, iLength);

		if(aRand[iRand] != -1){
			iUniqueRand = aRand[iRand];
			
			aRand[iRand] = -1;

			iEnd--;
			break;
		}
	}
	
	return iUniqueRand;
}


/*Picking the third diary activates the vision
 */
public void MapFlashbackOver()
{
	//If the diary flashback is running, then need to set back some stuff when flashback is over.
	if(GetLocalVarInt("DiaryFlashbackActive")==1)
	{
		SetLocalVarInt("DiaryFlashbackActive",0);		
		
		FadeImageTrailTo(1.2f, 2);
		FadeSepiaColorTo(0.65f, 0.5f);
		FadePlayerFOVMulTo(2, 0.04f);
	}	
}

/*Picking the third diary activates the vision
 */
public void PickDiary(string  asItem, int alEntryIdx)
{	
	AddLocalVarInt("diary", 1);
	
	if(GetLocalVarInt("diary") == 1)
		PlayMusic("03_paper_daniel01.ogg", false, 0.4f, 0, 10, false);
	
	else if(GetLocalVarInt("diary") == 2)
		PlayMusic("03_paper_daniel02.ogg", false, 0.4f, 0, 10, false);
	
	else if(GetLocalVarInt("diary") == 3){	
		ReturnOpenJournal(false);
		
		SetLocalVarString("startposition", "Start_"+asItem);
		
		PlayMusic("03_event_tomb.ogg", false, 0.4f, 0, 10, false);
		
		AddTimer("TimerFlashBack", 0.01f, "TimerFlashBack");
	}	
	
	/*DEBUG
	 */
	AddDebugMessage("Got Diary and Int is: "+GetLocalVarInt("diary"), true);
}
/*Player touches orb and end of vision is triggered
 */
public void InteractOrb(string  entity)
{	
	RemoveTimer("TimerFlashBack");
	
	SetLocalVarInt("TimerFlashBack", 9);

	AddTimer("TimerFlashBack", 0.1f, "TimerFlashBack");
	
	AddEffectVoice("CH01L03_DanielDiary03_05", "", "Flashbacks", "CH01L03_DanielDiary03_05", false, "", 0, 0);
	
	if(GetLanternActive()){
		SetLanternActive(false, true);
		SetLanternDisabled(true);
	} else SetLanternDisabled(true);
			
	/*DEBUG
	 */
	AddDebugMessage("Interacted with ORB", true);
}
public void TimerFlashBack(string  asTimer)
{	
	/*Configurables
	 */
	int iMaxEventStep = 18;		//How many steps there are in the switch event
	float fEventSpeed = 0.5f;	//The default time between steps in an event
	
	/*Helpers - Do not edit
	 */
	string sEvent = asTimer;	//Using first timer name for variable, timer name & callback for the timer that loops
	AddLocalVarInt(sEvent, 1);	//What step to play in the event
	
	if(GetPlayerLampOil() <= 10) AddPlayerLampOil(5); //Make sure player always has oil for lantern during flashback
	
	switch(GetLocalVarInt(sEvent)){
		case 1:
			SetPlayerLookSpeedMul(0.8f);
			
			SetPlayerCrouchDisabled(true);
			SetInventoryDisabled(true);
			SetSanityDrainDisabled(true);
			
			if(GetLanternActive())
			{
				SetLocalVarInt("LanternActive", 1);
				SetLanternActive(false, true);
				SetLanternDisabled(true);
			}
			else 
			{
				SetLanternDisabled(true);
			}
			
			StartEffectFlash(0.1f, 0.1f, 0.25f);
			SetPlayerRunSpeedMul(0.6f);
			SetPlayerMoveSpeedMul(0.75f);
			PlaySoundAtEntity("SoundFlash1", "scare_thump_flash.snt", "Player", 0.0f, false);
			PlaySoundAtEntity("creak", "03_creak.snt", "Player", 16.0f, false);
			PlaySoundAtEntity("Breath1", "react_breath.snt", "Player", 1.0f, false);
			fEventSpeed = 0.3f;
		break;
		case 2:
			SetLocalVarInt("DiaryFlashbackActive",1);
			
			SetPlayerLookSpeedMul(0.6f);
			//AddEffectVoice("CH01L03_DanielDiary03_01", "", "Flashbacks", "CH01L03_DanielDiary03_01", false, "", 0, 0);
			//SetEffectVoiceOverCallback("DiaryPartsOver");
			StartScreenShake(0.01f, 0.75f, 0.25f,1.5f);
			FadePlayerRollTo(60, 5, 5);
			FadeImageTrailTo(1.2f, 2);
			FadeSepiaColorTo(0.65f, 0.5f);
			FadePlayerFOVMulTo(4, 0.04f);
			MovePlayerHeadPos(0, -1.2f, 0, 0.3f, 0.25f);
			//AddTimer("extrap", 0.5f, "TimerExtraVoiceEffects");
			//AddTimer("extrap2", 1.0f, "TimerExtraVoiceEffects");
			//AddTimer("extrap3", 1.75f, "TimerExtraVoiceEffects");
			//AddTimer("extrap4", 2.75f, "TimerExtraVoiceEffects");
			//AddTimer("extrap5", 3.5f, "TimerExtraVoiceEffects");
			fEventSpeed = 2.0f;
		break;
		case 3:
			SetPlayerLookSpeedMul(0.4f);
			for(int i=1;i<=3;i++)
			{
				StopSound("ambs"+i, 8);
			}
			FadePlayerRollTo(-60, 10, 10);
			MovePlayerHeadPos(0, 0, 0, 0.2f, 0.25f);
			SetPlayerRunSpeedMul(0.3f);
			SetPlayerMoveSpeedMul(0.5f);
			PlaySoundAtEntity("Breath1", "react_breath.snt", "Player", 1.5f, false);
			fEventSpeed = 1.75f;
		break;
		case 4:
			SetPlayerLookSpeedMul(0.2f);
			//FadeOut(6);
			FadeOut(4);
			FadePlayerRollTo(0, 10, 10);
			MovePlayerHeadPos(0, -1.2f, 0, 1.0f, 0.25f);
			StartPlayerLookAt("AreaLookUp", 0.5f, 0.5f, "");
			SetPlayerRunSpeedMul(0);
			SetPlayerMoveSpeedMul(0.3f);
			PlaySoundAtEntity("rock_amb", "03_rock_amb.snt", "Player", 6, false);
			PlaySoundAtEntity("Breath1", "react_breath.snt", "Player", 1.25f, false);
			//fEventSpeed = 1.5f;
			fEventSpeed = 4.0f;
		break;
		case 5:
			SetPlayerLookSpeedMul(0.1f);
			FadePlayerFOVMulTo(2, 0.1f);
			AddEffectVoice("CH01L03_DanielDiary03_01", "", "Flashbacks", "CH01L03_DanielDiary03_01", false, "", 0, 0);
			SetEffectVoiceOverCallback("DiaryPartsOver");
			AddTimer("extrap", 4.5f, "TimerExtraVoiceEffects");
			AddTimer("extrap2", 5.0f, "TimerExtraVoiceEffects");
			AddTimer("extrap3", 5.75f, "TimerExtraVoiceEffects");
			AddTimer("extrap4", 6.75f, "TimerExtraVoiceEffects");
			AddTimer("extrap5", 7.5f, "TimerExtraVoiceEffects");
			SetSkyBoxActive(false);
			SetPlayerActive(false);
			MovePlayerHeadPos(0, 0, 0, 0.5f, 0.25f);
			PlaySoundAtEntity("Breath1", "react_breath.snt", "Player", 1.5f, false);
			SetLocalVarInt("VoiceResume", 5);
			RemoveTimer(sEvent);
			return;
		break;
		case 6:
			StartPlayerLookAt("AreaRock_4", 5, 5, "");
			AddEffectVoice("CH01L03_DanielDiary03_02", "", "Flashbacks", "CH01L03_DanielDiary03_02", false, "", 0, 0);
			SetEffectVoiceOverCallback("DiaryPartsOver");
			SetLocalVarInt("VoiceResume", 7);
			MovePlayerHeadPos(0, -1.2f, 0, 3, 0.25f);
			PlaySoundAtEntity("loop", "03_loop.snt", "Player", 2, false);
			StopSound("creak", 8);
			AddTimer("randrock", 10, "TimerRandRock");
			AddTimer("extra1", 0.5f, "TimerExtraVoiceEffects");
			AddTimer("extra2", 2, "TimerExtraVoiceEffects");
			AddTimer("extra3", 3.5f, "TimerExtraVoiceEffects");
			AddTimer("extra3.1f", 4.5f, "TimerExtraVoiceEffects");
			fEventSpeed = 6;
		break;
		case 7:
			FadeIn(5);
			FadeImageTrailTo(0, 2);
			FadePlayerFOVMulTo(1, 0.1f);
			TeleportPlayer("PlayerStartArea_Flash");
			StartPlayerLookAt("orb_1", 0.1f, 0.1f, "");
			MovePlayerHeadPos(0, 0, 0, 0.25f, 0.25f);
			PlaySoundAtEntity("orb_loop", "03_orb_loop.snt", "orb_1", 6, false);
			StopSound("loop", 4);
			SetLocalVarInt(sEvent, 7);
			AddTimer("lookat", 8, "TimerExtraVoiceEffects");
			SetPlayerActive(true);
			RemoveTimer(sEvent);
			return;
		break;
		case 8:
			//SetLanternDisabled(false);
			//SetLanternActive(true, false);
			SetPlayerLookSpeedMul(0.3f);
			AddEffectVoice("CH01L03_DanielDiary03_03", "", "Flashbacks", "CH01L03_DanielDiary03_03", false, "", 0, 0);
			AddTimer("extra4", 0.1f, "TimerExtraVoiceEffects");
			AddTimer("extra5", 2, "TimerExtraVoiceEffects");
			AddTimer("extra6", 3.5f, "TimerExtraVoiceEffects");
		break;
		case 9:
			SetPlayerLookSpeedMul(0.5f);
			SetLocalVarInt(sEvent, 8);
			AddTimer(sEvent, fEventSpeed, sEvent);
			return;
		break;
		case 10:
			SetPlayerMoveSpeedMul(0.5f);
			SetPlayerLookSpeedMul(0.3f);
			StartScreenShake(0.025f, 2,4,4);
			FadeLightTo("OrbLight",1,1,1,1,0,4);
			FadePlayerFOVMulTo(0.2f, 0.2f);
			SetPropActiveAndFade("orb_1", false, 10);
			FadePlayerRollTo(60, 1, 1);
			AddEffectVoice("CH01L03_DanielDiary03_06", "", "Flashbacks", "CH01L03_DanielDiary03_06", false, "", 0, 0);
			fEventSpeed = 4.0f;
		break;
		case 11: 
			SetPlayerMoveSpeedMul(0.3f);
			SetPlayerLookSpeedMul(0.2f);
			SetPlayerActive(false);
			FadePlayerFOVMulTo(3, 0.25f);
			StartPlayerLookAt("orb_1", 0.5f, 0.5f, "");
			FadeLightTo("OrbLight",1,1,1,1,20,8);
			FadePlayerRollTo(-60, 2, 2);
			MovePlayerHeadPos(0, -0.5f, 0, 0.2f, 0.2f);
			SetEffectVoiceOverCallback("DiaryPartsLast");
			StopSound("orb_loop", 4);
			PlaySoundAtEntity("orb_loop2", "03_orb_loop_loud.snt", "orb_1", 6, false);
			fEventSpeed = 2.0f;
		break;
		case 12:
			SetPlayerMoveSpeedMul(0.1f);
			SetPlayerLookSpeedMul(0.1f);
			StartEffectFlash(4, 2, 4);
			FadePlayerRollTo(0, 1, 1);
			fEventSpeed = 5.0f;
		break;
		case 13:
			FadeOut(1);
			MovePlayerHeadPos(0, -1.3f, 0, 5, 0.25f);
			PlaySoundAtEntity("player_fall", "player_bodyfall.snt", "Player", 0.5f, false);
			fEventSpeed = 0.35f;
		break;
		case 14:
			StartPlayerLookAt("AreaLookUp", 0.5f, 0.5f, "");
			PlaySoundAtEntity("BreakOrb", "03_orb.snt", "Player", 0.0f, false);
			PlaySoundAtEntity("creak", "03_creak.snt", "Player", 8.0f, false);
			StopSound("orb_loop2", 1);
			RemoveTimer("randrock");
			fEventSpeed = 1.0f;
		break;
		case 15:
			StopSound("rock_amb", 8);
			fEventSpeed = 4.0f;
		break;
		case 16:
			FadePlayerFOVMulTo(1, 1);
			StartPlayerLookAt("candlestick_tri_5", 5, 5, "");
			SetPlayerLookSpeedMul(0.1f);
			PlaySoundAtEntity("ambs1", "03_amb.snt", "AmbSounds", 4.0f, true);
			PlaySoundAtEntity("ambs2", "03_amb_library.snt", "AmbSounds", 4.0f, true);
			PlaySoundAtEntity("ambs3", "03_insects.snt", "AmbSounds", 4.0f, true);
			StopSound("creak", 6);
			AddTimer("music", 8, "TimerExtraVoiceEffects");
			fEventSpeed = 8.0f;
		break;
		case 17:
			SetLocalVarInt("DiaryFlashbackActive",0);
			SetPlayerLookSpeedMul(0.5f);
			FadeSepiaColorTo(0, 0.05f);
			MovePlayerHeadPos(0, 0, 0, 0.5f, 0.25f);
			
			SetPlayerCrouchDisabled(false);
			SetInventoryDisabled(false);
			SetSanityDrainDisabled(false);
			
			SetPlayerActive(true);
			
			//SetSkyBoxActive(true);
			if(GetLocalVarInt("LanternActive") == 1)
			{
				SetLocalVarInt("LanternActive", 0);
				SetLanternDisabled(false);
				SetLanternActive(true, true);
			}
			else
			{
				SetLanternDisabled(false);
			}
			
			StopPlayerLookAt();
			TeleportPlayer(GetLocalVarString("startposition"));
			PlaySoundAtEntity("Breath3", "react_breath_slow.snt", "Player", 0.8f, false);
			CreateParticleSystemAtEntity("pearls", "ps_orb_on_player.ps", "Player", false);
			FadeIn(7);
			fEventSpeed = 4.0f;
		break;
		case 18:
			SetPlayerLookSpeedMul(1.0f);
			AddTimer("lookloop", 0.1f, "TimerRandomLook");
		break;
	}
	
	if(GetLocalVarInt(sEvent) <= iMaxEventStep) AddTimer(sEvent, fEventSpeed, sEvent);
}
public void TimerExtraVoiceEffects(string  asTimer)
{
	if(asTimer == "extrap") PlaySoundAtEntity("pound1", "03_pound.snt", "Player", 0.01f, false);	
	if(asTimer == "extrap2") PlaySoundAtEntity("pound2", "03_pound.snt", "Player", 0.02f, false);	
	if(asTimer == "extrap3") PlaySoundAtEntity("pound3", "03_pound.snt", "Player", 0.03f, false);
	if(asTimer == "extrap4") PlaySoundAtEntity("pound3", "03_pound.snt", "Player", 0.04f, false);
	if(asTimer == "extrap5") PlaySoundAtEntity("pound3", "03_pound.snt", "Player", 0.05f, false);
	if(asTimer == "extra1") PlaySoundAtEntity("player_fall", "player_bodyfall.snt", "Player", 0.2f, false);
	if(asTimer == "extra2") PlaySoundAtEntity("gasp1", "react_pant.snt", "Player", 0.75f, false);
	if(asTimer == "extra3") PlaySoundAtEntity("gasp1", "react_pant.snt", "Player", 0.8f, false);
	if(asTimer == "extra3.1f") PlaySoundAtEntity("gasp1", "react_pant.snt", "Player", 0.85f, false);
	if(asTimer == "extra4") PlaySoundAtEntity("player_push", "player_climb.snt", "Player", 0.5f, false);
	if(asTimer == "extra5") PlaySoundAtEntity("player_push", "player_climb.snt", "Player", 0.25f, false);
	if(asTimer == "extra6") PlaySoundAtEntity("player_push", "player_climb.snt", "Player", 0.0f, false);
	if(asTimer == "extra7"){ 
		AddEffectVoice("CH01L03_DanielDiary03_07", "", "Flashbacks", "CH01L03_DanielDiary03_07", false, "", 0, 0);
		PlaySoundAtEntity("rock", "03_lift_rock.snt", "Player", 3, false);
	}
	if(asTimer == "extra7.1f") PlaySoundAtEntity("drag1", "03_drag_dirt.snt", "Player", 0, false);
		
	if(asTimer == "extra7.2f") PlaySoundAtEntity("drag2", "03_drag_dirt.snt", "Player", 0.25f, false);
	if(asTimer == "extra7.3f") PlaySoundAtEntity("drag3", "03_drag_dirt.snt", "Player", 0.5f, false);	
	if(asTimer == "extra8"){ 
		StopSound("rock", 3);
		PlaySoundAtEntity("crew", "03_crew_talk.snt", "Player", 0, false);
	}
	
	if(asTimer == "music"){
		PlaySoundAtEntity("pianoonplayer", "03_waking_up.snt", "Player", 1, false);
		SetPlayerLookSpeedMul(0.3f);
	}
	if(asTimer == "lookat"){ 
		StartPlayerLookAt("orb_1", 0.6f, 0.6f, "");
		SetPlayerLookSpeedMul(0.1f);
	}
}
public void DiaryPartsOver()
{
	SetLocalVarInt("TimerFlashBack", GetLocalVarInt("VoiceResume"));
	
	AddTimer("TimerFlashBack", 0.01f, "TimerFlashBack");
	
	/*DEBUG
	 */	
	AddDebugMessage("Resume with CH01L03_DanielDiary03_0", true);
}
public void DiaryPartsLast()
{
	AddTimer("extra7", 0.01f, "TimerExtraVoiceEffects");
	AddTimer("extra8", 3, "TimerExtraVoiceEffects");
	AddTimer("extra7.1f", 5, "TimerExtraVoiceEffects");
	AddTimer("extra7.2f", 6.5f, "TimerExtraVoiceEffects");
	AddTimer("extra7.3f", 8, "TimerExtraVoiceEffects");
}
public void CollideOrb(string  asParent, string  asChild, int alState)
{
	if(asChild == "AreaHalfToOrb"){
		AddEffectVoice("CH01L03_DanielDiary03_04", "", "Flashbacks", "CH01L03_DanielDiary03_04", false, "", 0, 0);
	}
	if(asChild == "AreaCloseToOrb"){
		SetEntityPlayerLookAtCallback("orb_1", "LookAtOrb", true);
	}
}
public void LookAtOrb(string  entity, int alState)
{
	if(alState == 1){
		SetEntityPlayerInteractCallback("orb_1", "InteractOrb", true);
		//AddEntityCollideCallback("Player", "AreaOrbX", "CollideActualOrb", false, 0);	
	}
}
/*public void CollideActualOrb(string  asParent, string  asChild, int alState)
{
	InteractOrb("hitpa");
	//SetEntityPlayerInteractCallback("orb_1", "", true);
}*/
/*Player movement gets slower and slower the farther away from the path he goes
 */
public void CollideSlowArea(string  asParent, string  asChild, int alState)
{
	if(alState == 1){
		if(asChild == "SlowArea_1") SetPlayerMoveSpeedMul(0.75f);
		else if(asChild == "SlowArea_2") SetPlayerMoveSpeedMul(0.6f);
		else if(asChild == "SlowArea_3") SetPlayerMoveSpeedMul(0.5f);
		else SetPlayerMoveSpeedMul(0.35f);

		/*DEBUG
		 */	
		AddDebugMessage("Player enter "+asChild, true);
	} 
	else {
		if(asChild == "SlowArea_1"){
			SetPlayerMoveSpeedMul(0.6f);
			
			if(GetLocalVarInt("FirstEnterSlowArea") == 0){
				AddEntityCollideCallback("Player", "SlowArea_2", "CollideSlowArea", false, 0);	
				
				SetLocalVarInt("FirstEnterSlowArea", 1);
			}
		}
		else if(asChild == "SlowArea_2"){
			SetPlayerMoveSpeedMul(0.5f);
			
			if(GetLocalVarInt("FirstEnterSlowArea") == 1){
				AddEntityCollideCallback("Player", "SlowArea_3", "CollideSlowArea", false, 0);	
				
				SetLocalVarInt("FirstEnterSlowArea", 2);
			}
		}
		else if(asChild == "SlowArea_3"){
			SetPlayerMoveSpeedMul(0.35f);
			
			if(GetLocalVarInt("FirstEnterSlowArea") == 2){
				AddEntityCollideCallback("Player", "SlowArea_4", "CollideSlowArea", false, 0);	
				
				SetLocalVarInt("FirstEnterSlowArea", 3);
			}
		} 
		else SetPlayerMoveSpeedMul(0.2f);

		/*DEBUG
		 */	
		AddDebugMessage("Player leave "+asChild, true);
	}
}
/*Some random rock falling sounds while walking towards orb
 */
public void TimerRandRock(string  asTimer)
{
	int iRand = UniqueRandom(1, 6, true);
	float fLoop = RandFloat(5,10);

	PlaySoundAtEntity("RockSound"+iRand, "03_rock_move.snt", "AreaRock_"+iRand, 0.4f, false);
	
	AddTimer("randrock", fLoop, "TimerRandRock");
	
	/*DEBUG
	 */	
	AddDebugMessage("Sound in AreaRock_"+iRand, true);
}
//END DIARY FLASHBACK//
///////////////////////


//////////////////////////
//BEGIN RANDOM LOOK SPIN//
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
	
	bRoll= bRoll == false ? true : false;
	
	if(bRoll)
		FadePlayerRollTo(RandFloat(2,10), RandFloat(0.075f,0.35f), RandFloat(0.55f,1.15f)); 
	else
		FadePlayerRollTo(RandFloat(-2,-10), RandFloat(0.05f,0.25f), RandFloat(0.5f,1)); 
	
	AddLocalVarInt("Dizzy",1);
	
	if(GetLocalVarInt("Dizzy") <= 4) AddTimer("lookloop", RandFloat(3.0f,6.0f), "TimerRandomLook");
	else {
		SetPlayerRunSpeedMul(1);
		SetPlayerMoveSpeedMul(1);
		FadePlayerRollTo(0, 0.5f, 1); 
		FadePlayerFOVMulTo(1, 1);
		FadeImageTrailTo(0,1.5f);
	}
}
public void StopRandomLook()
{
	RemoveTimer("lookloop");
	
	SetPlayerMoveSpeedMul(1);
	FadePlayerFOVMulTo(1, 1);
	FadeImageTrailTo(0,1.0f);
	FadePlayerRollTo(0, 0.5f, 2); 
}
//END RANDOM LOOK SPING//
/////////////////////////


//////////////////////////
//BEGIN MANY BOOKS SCARE//
//Skipped for now.
/*public void CollideBeginManyBook(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("SoundScratchx_1", "scare_scratch", "throwbook_13", 0.5f, false);
	
	PlayMusic("03_event_books.ogg", false, 0.7f, 0, 10, false);
	
	AddTimer("manybooks", 3, "TimerManyBooks");
	
	//DEBUG
	AddDebugMessage("Begin Many Books Scare ", true);
}
public void TimerManyBooks(string  asTimer)
{
	AddLocalVarInt("VarMany", 1);	//What step to play in the event
	float fSpeedMany = 0.5f;	//The default time between steps in an event
	
	switch(GetLocalVarInt("VarMany")) {
		case 1:
			CreateParticleSystemAtEntity("PSGhostBang", "ps_break_wood.ps", "throwbook_14", false);
			PlaySoundAtEntity("SoundBang", "break_wood.snt", "throwbook_13", 0.0f, false);
			PlaySoundAtEntity("SoundFearx2", "scare_male_terrified.snt", "Player", 0.5f, false);
			fSpeedMany = 0.2f;
		break;
		case 2:
			GiveSanityDamage(10, true);
			for(int i=1;i<=5;i++) AddPropForce("throwbook_"+i, -100, 50, 0, "world");
			fSpeedMany = 0.2f;
		break;
		case 3:
			for(int i=6;i<=10;i++) AddPropForce("throwbook_"+i, -200, 120, -50, "world");
			fSpeedMany = 0.1f;
		break;
		case 4:
			for(int i=11;i<=15;i++) AddPropForce("throwbook_"+i, -300, 200, 50, "world");
			FadeLightTo("BoxLight_4",1,0,0.2f,1,-1,0.1f);
			fSpeedMany = 0.2f;
		break;
		case 5:
			StartEffectFlash(0.05f, 0.15f, 0.05f);
			PlaySoundAtEntity("SoundFlash1", "scare_thump_flash.snt", "Player", 0, false);
			fSpeedMany = 0.8f;
		break;
		case 6:
			FadeLightTo("BoxLight_4",0,0,0,0,-1,0.1f);
			StopSound("SoundFearx2", 2);
			StopSound("SoundScratchx_1", 2);
			SetPropActiveAndFade("throwbook_*", false, 2);
		break;
	}

	if(GetLocalVarInt("VarMany") < 6) AddTimer("manybooks", fSpeedMany, "TimerManyBooks");
}*/
//END MANY BOOKS SCARE//
////////////////////////


/////////////////////
//BEGIN PIANO MUSIC//
public void CollideBeginPiano(string  asParent, string  asChild, int alState)
{
	if(asChild == "AreaSecondPiano"){
		SetLeverStuckState("piano_2", 0, false);
		AddBodyImpulse("piano_2_BodyLid", 0,100, 0, "world");
		
		PlaySoundAtEntity("SoundPiano2", "general_piano02.snt", "AreaPlayPiano", 0.0f, false);
		//PlaySoundAtEntity("SoundWind", "general_wind_whirl.snt", "AreaPlayPiano", 0.5f, false);
		PlaySoundAtEntity("SoundWind", "scare_wall_scratch_single", "AreaPlayPiano", 0.5f, false);
		
		SetEntityActive("chair_nice01_12",	false);
		SetEntityActive("chair_ghost", true);
		
		AddTimer("fallbook", 1.5f, "FallBook");
		return;
	}
	
	PlaySoundAtEntity("SoundPiano", "general_piano01.snt", "piano_1", 0.0f, false);
	//PlaySoundAtEntity("SoundWind", "general_wind_whirl.snt", "tome01_8", 0.0f, false);
	PlaySoundAtEntity("SoundWind", "03_wall_scratch.snt", "tome01_8", 0.5f, false);
	
	//SetLeverStuckState("piano_1", 0, false);
	//AddBodyImpulse("piano_1_BodyLid", 0,100, 0, "world");
		
	AddTimer("fallbook", 1.5f, "FallBook");
	
	AddTimer("fallbook1", 2.2f, "FallBook");
	
	AddTimer("fallbook2", 2.5f, "FallBook");
}
public void CollideCloseLid(string  asParent, string  asChild, int alState)
{
	SetLeverStuckState("piano_1", 0, false);
	AddPropImpulse("piano_1", 0,0,10, "world");
	
	StopSound("SoundPiano", 1.0f);
	
	GiveSanityDamage(10, true);
	
	CreateParticleSystemAtEntity("PSdust", "ps_dust_piano.ps", "AreaPianoDust", false);
	
	AddTimer("liddust", 0.4f, "FallBook");
}

public void FallBook(string  asTimer)
{
	if(asTimer == "liddust"){
		PlaySoundAtEntity("LidClose", "level_wood_min_max01.snt", "piano_1", 0.0f, false);
		PlaySoundAtEntity("enemy", "enemy/grunt/amb_idle",  "AreaDustScrape_6", 0, false);
		return;
	}
	else if(asTimer == "fallbook1"){
		CreateParticleSystemAtEntity("PSdustbook", "ps_dust_impact.ps", "tome01_8", false);
		StartScreenShake(0.005f, 0.5f, 0.5f, 0.5f);
		return;
	}
	else if(asTimer == "fallbook2"){
		GiveSanityDamage(10, true);
		return;
	}	
	
	StopSound("SoundWind", 2);
	
	AddPropForce("tome01_8", 0, 300, -700, "world");
}
//END PIANO MUSIC//
///////////////////


///////////////////////////
//BEGIN SECRET SHELF DOOR//
public void InteractLibDoor(string  asEntity)
{
	AddTimer("doormess", 0.5f, "TimerLockedDoorMess");
}
public void TimerLockedDoorMess(string  asTimer)
{
	SetMessage("Ch01Level03", "DoorLocked", 0);
	AddQuest("03LockedLibrary", "03LockedLibrary");
}

public void EntityCallBreakWall(string  asEntity, string  type)
{
	SetEntityActive("PassageInteractArea", false);
	//SetEntityPlayerInteractCallback("mansionbase_secret_passage_1", "", true);
	
	GiveSanityBoostSmall();
	
	CompleteQuest("03LockedLibrary", "03LockedLibrary");
}

public void CollideSecretBook(string  asParent, string  asChild, int alState)
{
	if(alState == 1) {
		SetLocalVarInt("Var"+asParent, 1);
		
		AddTimer(asParent, 20, "PushBackBook");
		
		SetPropObjectStuckState(asParent, 1);
		
		PlaySoundAtEntity("Sound"+asParent, "gameplay_tick", asParent, 0.0f, false);
		
		StartScreenShake(0.001f, 0.5f, 0.5f, 0.5f);
		
		PlayGuiSound("16_lever_stuck", 0.3f);
		
		/*DEBUG
		 */
		AddDebugMessage("Book in area: "+asParent, true);
	} else {
		SetLocalVarInt("Var"+asParent, 0);
		
		RemoveTimer(asParent);
		
		PlaySoundAtEntity("Sound2"+asParent, "lock_door", asParent, 1.5f, false);
		PlayGuiSound("16_lever_stuck", 0.2f);
		
		StopSound("Sound"+asParent, 1.0f);
		
		/*DEBUG
		 */
		AddDebugMessage("Book out of area: "+asParent, true);
	}
	
	/*All books are pulled before time is out and the secret room is revealed.
	 */
	if(GetLocalVarInt("VarSecretBook_1") == 1 && GetLocalVarInt("VarSecretBook_2") == 1 && GetLocalVarInt("VarSecretBook_3") == 1) {
		
		FadeLightTo("PointLight_30", 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 3.0f);
		
		SetMoveObjectState("shelf_secret_door_1", 1);
		
		SetPropObjectStuckState("SecretBook_*", -1);
		
		SetEntityInteractionDisabled("SecretBook_*", true);
		
		PlaySoundAtEntity("BooksDone", "lock_door", "Player", 0, false);
		
		CreateParticleSystemAtEntity("dust", "ps_dust_falling_door_quick", "AreaDoorParticle", false);
		
		for(int i=1;i<=3;i++){ RemoveTimer("SecretBook_"+i); StopSound("SoundSecretBook_"+i, 0.0f); }
		
		GiveSanityBoostSmall();
		
		PlayMusic("03_puzzle_secret.ogg", false, 0.7f, 0, 10, false);
		
		CompleteQuest("03Books", "03Books");
		
		/*DEBUG
		 */
		AddDebugMessage("All books in position, move shelf!", true);
	}

}
public void PushBackBook(string  asTimer)
{
	SetPropObjectStuckState(asTimer, -1);
	
	AddTimer("2"+asTimer, 0.25f, "PushBackBook02");
	
	/*DEBUG
	 */
	AddDebugMessage("Push back book: "+asTimer, true);
}
public void PushBackBook02(string  asTimer)
{
	if(asTimer == "2SecretBook_1") SetPropObjectStuckState("SecretBook_1", 0);
	else if(asTimer == "2SecretBook_2") SetPropObjectStuckState("SecretBook_2", 0);
	else SetPropObjectStuckState("SecretBook_3", 0);
}

/*Give quest after the visions about the books
 */
public void VisionOverBooks()
{
	AddQuest("03Books", "03Books");
	
	SetEntityPlayerInteractCallback("shelf_secret_door_1", "InteractMoveShelf", true);
	
	AddTimer("enemys", 1.0f, "TimerPlayEnemy");
	
	//If the diary flashback is running, then need to set back some stuff when flashback is over.
	if(GetLocalVarInt("DiaryFlashbackActive")==1)
	{
		SetLocalVarInt("DiaryFlashbackActive",0);		
		
		FadeImageTrailTo(1.2f, 2);
		FadeSepiaColorTo(0.65f, 0.5f);
		FadePlayerFOVMulTo(2, 0.04f);
	}	
}
public void TimerPlayEnemy(string  asTimer)
{
	PlaySoundAtEntity("enemy", "enemy/grunt/amb_idle",  "AreaDustScrape_6", 0, false);
}

public void InteractMoveShelf(string  asEntity)
{
	SetMessage("Ch01Level03", "MoveShelf", 0);
}

/*Find the key for the wine cellar
 */
public void PickKey(string  asEntity, string  asType)
{	
	//GiveSanityBoostSmall();
	
	SetGlobalVarString(asEntity, asEntity);
	
	AddEntityCollideCallback("Player", "AreaActivateLastPiano", "CollideLastPiano", true, 1);
	
	AddTimer("TimerEGrunt", 0.2f, "TimerEGrunt");
}

public void TimerEGrunt(string  asTimer)
{
	int iMaxEventStep = 8;
	float fEventSpeed = 0.5f;
	string sEvent = asTimer;

	AddLocalVarInt(sEvent, 1);

	switch(GetLocalVarInt(sEvent)){
		case 1:
			SetPlayerMoveSpeedMul(0.8f);
			SetPlayerRunSpeedMul(0.8f);
			PlaySoundAtEntity("growl", "03_amb_idle", "AreaGrunt_1", 0.3f, false);
			fEventSpeed = 0.6f;
		break;
		case 2:
			SetPlayerMoveSpeedMul(0.6f);
			SetPlayerRunSpeedMul(0.6f);
			PlaySoundAtEntity("growl", "03_attack_claw", "AreaGrunt_1", 0.0f, false);
			fEventSpeed = 0.3f;
		break;
		case 3:
			SetPlayerMoveSpeedMul(0.4f);
			SetPlayerRunSpeedMul(0.4f);
			
			SetEntityActive("mansion_1", false);
			SetEntityActive("mansion_8", true);
			
			StartScreenShake(0.01f, 0.3f, 0.2f, 0.5f);
			
			PlaySoundAtEntity("hitdoor", "break_wood", "AreaGrunt_1", 0.0f, false);
			GiveSanityDamage(10.0f, true);
			
			CreateParticleSystemAtEntity("breakdoor", "ps_break_wood.ps", "mansion_8", false);
			
			FadeImageTrailTo(2.0f, 2.0f);
		break;
		case 4:
			PlayMusic("15_event_prisoner", false, 1.0f, 1.0f, 10, false);
			SetPlayerMoveSpeedMul(0.5f);
			SetPlayerRunSpeedMul(0.5f);
			PlayGuiSound("react_scare", 0.7f);
			fEventSpeed = 2.0f;
		break;
		case 5:
			StopMusic(4.0f, 10);
			SetPlayerMoveSpeedMul(0.7f);
			SetPlayerRunSpeedMul(0.7f);
			PlayGuiSound("react_breath", 0.8f);
			fEventSpeed = 2.0f;
		break;
		case 6:
			PlaySoundAtEntity("growl", "03_amb_idle", "AreaGrunt_2", 0.3f, false);
			SetPlayerMoveSpeedMul(0.8f);
			SetPlayerRunSpeedMul(0.8f);
			PlayGuiSound("react_breath", 0.6f);
			fEventSpeed = 2.0f;
		break;
		case 7:
			SetPlayerMoveSpeedMul(0.9f);
			SetPlayerRunSpeedMul(0.9f);
			PlayGuiSound("react_breath", 0.4f);
			fEventSpeed = 1.0f;
		break;
		case 8:
			SetPlayerMoveSpeedMul(1.0f);
			SetPlayerRunSpeedMul(1.0f);
			PlayGuiSound("react_breath", 0.2f);
			FadeImageTrailTo(0.0f, 2.0f);
		break;
	}

	if(GetLocalVarInt(sEvent) <= iMaxEventStep) AddTimer(sEvent, fEventSpeed, sEvent);
}
//END SECRET SHELF DOOR//
/////////////////////////


///////////////
//BEGIN CLOUD//
public void CloudEffects(string  asWhere, bool bLook)
{
	if(bLook) StartPlayerLookAt(asWhere, 0.75f, 0.75f, "");
	
	CreateParticleSystemAtEntity(asWhere, "ps_cloud_thing01.ps", asWhere, false);
	FadeSepiaColorTo(0.5f, 0.025f);
	FadeRadialBlurTo(0.1f, 0.025f);
	SetRadialBlurStartDist(0.2f);
	SetPlayerMoveSpeedMul(0.25f);
	SetPlayerRunSpeedMul(0);
	StartScreenShake(0.01f, 4, 2,2);
	
	AddTimer(asWhere, 0.05f, "TimerThing");
	AddTimer("react1", 0.25f, "TimerThing");
	AddTimer("thing1", 1.5f, "TimerThing");
	AddTimer("thing2", 3.0f, "TimerThing");
	AddTimer("thing3", 4.6f, "TimerThing");
	AddTimer("thing4", 6.0f, "TimerThing");
}
public void CollideCloudThing(string  asParent, string  asChild, int alState)
{
	if(asChild == "AreaCloudActivate_2")
		CloudEffects("AreaCloudThing_2", true);
	else if(asChild == "AreaCloudActivate_3")
		CloudEffects("AreaDustScrape_3", false);
	else if(asChild == "AreaCloudActivate_4")
		CloudEffects("AreaDustScrape_8", false);
	else if(asChild == "AreaCloudActivate_5")
		CloudEffects("AreaCloudThing_5", true);
		
	/*DEBUG
	 */
	AddDebugMessage("Do cloud " + asChild, true);
}
public void TimerThing(string  asTimer)
{	
	PlaySoundAtEntity("clouds1", "03_cloud_swarm.snt", asTimer, 1, false);
	SetPlayerMoveSpeedMul(0.9f);
	SetPlayerRunSpeedMul(0.9f);
		
	if(asTimer == "thing1"){
		SetPlayerMoveSpeedMul(0.7f);
		SetPlayerRunSpeedMul(0.7f);
	}
	if(asTimer == "thing2"){
		SetPlayerMoveSpeedMul(0.6f);
		SetPlayerRunSpeedMul(0.6f);
		PlaySoundAtEntity(asTimer, "react_breath.snt", "Player", 0.75f, false);
		StopSound("clouds1", 3.5f);
	}
	if(asTimer == "thing3"){
		SetPlayerMoveSpeedMul(0.8f);
		SetPlayerRunSpeedMul(0.8f);
		FadeSepiaColorTo(0, 0.1f);
		FadeRadialBlurTo(0, 0.1f);
		StopPlayerLookAt();
	}
	if(asTimer == "thing4"){
		SetPlayerMoveSpeedMul(1);
		SetPlayerRunSpeedMul(1);
		PlaySoundAtEntity(asTimer, "react_breath.snt", "Player", 1.25f, false);
	}
	
	if(asTimer == "react1"){
		GiveSanityDamage(10, false);
		PlaySoundAtEntity(asTimer, "react_scare.snt", "Player", 0.5f, false);
		SetPlayerMoveSpeedMul(0.8f);
		SetPlayerRunSpeedMul(0.8f);
	}
}
//END CLOUD//
/////////////


/////////////////////
//MISC MINOR EVENTS//
/*If player moves an urn from a table it will break and release a dust cloud
 */
public void CollideJarLeaveArea(string  asParent, string  asChild, int alState)
{
	SetPropHealth(asParent, 0);
	
	PlaySoundAtEntity("ghost_released", "03_in_a_bottle", "Player", 0, false);
	
	GiveSanityDamage(10, true);
}

/*Play a last piano piece on the way out and activate a last cloud thing
 */
public void CollideLastPiano(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("SoundPiano3", "general_piano03.snt", "piano_1", 0.0f, false);
	
	PlaySoundAtEntity("growl", "03_amb_idle", "AreaGrunt_4", 0.0f, false);
	
	/*DEBUG
	 */
	AddDebugMessage("Play last piano", true);
}

/*Dripping blood at hole in the ceiling
 */
public void TimerBloodDrop(string  asTimer)
{
	if(asTimer == "bloodsound"){
		PlaySoundAtEntity("bloddrop", "general_blood_drop", "AreaBloodDrop", 0, false);
		AddTimer("blooddrop", 0.1f, "TimerBloodDrop");
	}
	else{ 
		CreateParticleSystemAtEntity("PSblood", "ps_blood_drop.ps", "AreaBloodDrop_1", false);
		AddTimer("bloodsound", 2.9f, "TimerBloodDrop");
	}
}

/*Random ceiling particle and sound
 */
public void TimerCrawl(string  asTimer)
{
	int iCrawl = RandFloat(1, 8);	
	float fCrawl = RandFloat(0.5f,6.5f);
	
	CreateParticleSystemAtEntity("crawlPS"+iCrawl, "ps_dust_falling_small.ps", "AreaDustScrape_"+iCrawl, false);
	
	PlaySoundAtEntity("crawlSound"+iCrawl, "03_wall_scratch_single.snt", "AreaDustScrape_"+iCrawl, 1.0f, false);
	
	AddTimer("crawl", 3.5f+fCrawl, "TimerCrawl");
	
	/*DEBUG
	 */
	AddDebugMessage("Now crawling in: "+iCrawl+" Next crawl in: "+(5.5f+fCrawl), true);
}

/*Music when readin villlage note
 */
public void PickVillageNote(string  entity, string  type)
{
	PlayMusic("03_paper_village.ogg", false, 0.7f, 0, 10, false);
}
/////////////////
/////////////////


/////////////////////////
//BEGIN THE BROKEN WALL//
/* public void InteractBrokenWall(string  asEntity)
{
	PlayGuiSound("impact_rock_low3.ogg", 1.0f);
	PlayGuiSound("15_rock_break", 0.7f);
	
	SetMessage("Ch01Level03", "BrokenWall", 0);
	
	AddTimer(asEntity, 2.0f, "TimerBrokenWall");
}
public void TimerBrokenWall(string  asTimer)
{
	SetEntityPlayerInteractCallback(asTimer, "InteractBrokenWall", true);
}
 */
/*Give a hint on how to throw objects you hold
 */	
public void CollideHint(string  asParent, string  asChild, int alState)
{
	GiveHint("ThrowHint", "Hints", "ThrowHint", 0);
	
	/*DEBUG
	 */
	AddDebugMessage("Collide!", true);
}


public void PlayerInteractSecretPassage(string  asEntity)
{
	AddLocalVarInt("PassageHitCount", 1);
	AddTimer("TimerDecreaseHitCount", 0.5f, "TimerDecreaseHitCount");	
	
	if(GetTimerTimeLeft("PassageBreakMessagePaused")==0)
	{
		PlayGuiSound("impact_rock_low3.ogg", 1.0f);
		PlayGuiSound("15_rock_break", 0.7f);
	
		SetMessage("Ch01Level03", "BrokenWall", 0);
		
		AddTimer("PassageBreakMessagePaused", 0.5f, "");
	}
	
	int lInteractedCount = GetLocalVarInt("PassageHitCount");
	
	int lDebrisArea = RandInt(1, 4);
	CreateParticleSystemAtEntity("PSPassageDebris"+lDebrisArea, "ps_dust_impact.ps", "PassageInteractDebris_"+lDebrisArea, false);
	
	AddDebugMessage("Adding debris PS to PassageInteractDebris_"+lDebrisArea, false);
	
	if(lInteractedCount==3)
	{
		SetPropHealth("mansionbase_secret_passage_1", 0);
	}
}

public void TimerDecreaseHitCount(string  asTimer)
{
	AddLocalVarInt("PassageHitCount", -1);
}

//END THE BROKEN WALL//
///////////////////////


////////////////////////////////
//BEGIN CAVEIN AT END OF LEVEL//
/*Make the corridor cave in behind the Player
*/
public void CollideCaveIn(string  asParent, string  asChild, int alState)
{
	SetPropActiveAndFade("cave_in_cave_in_*", true, 1.0f);
	SetEntityActive("hanging_lantern_ceiling_7", false);
	
	CreateParticleSystemAtEntity("fogarea", "ps_area_fog_large", "AreaCaveIn", true);
	CreateParticleSystemAtEntity("fogarea2", "ps_area_fog_large", "AreaCaveIn2", true);
	CreateParticleSystemAtEntity("breakcave", "ps_break_mansionbase_wall", "AreaCaveIn", false);
	
	StartScreenShake(0.02f, 0.5f, 0.8f, 2.0f);

	PlaySoundAtEntity("rumble", "general_rock_rumble_no3d.snt", "Player", 0.5f, true);
	
	AddTimer("1", 1.0f, "TimerCaveIn");
	AddTimer("2", 0.7f, "TimerCaveIn");
	AddTimer("3", 3.0f, "TimerCaveIn");
	AddTimer("4", 5.0f, "TimerCaveIn");
	
	SetEntityActive("AreaCaveInQuest", true);
	SetEntityActive("AreaQuestDone", true);
	
	SetEntityActive("AreaLookGrunt_1", true);
	
	SetEntityActive("AreaGruntSound03", true);
	
	//Lock a door so force the player path + add some confusion.
	SetSwingDoorLocked("mansion_2", true, false);
	SetEntityPlayerInteractCallback("mansion_2", "Interactmansion_2", true);
	
	//Fade out some lights to make corridor darker.
	FadeLightTo("PointLight_4", 0,0,0,0, -1, 1);
	FadeLightTo("PointLight_2", 0,0,0,0, -1, 1);
}

public void Interactmansion_2(string  asEntity)
{
	PlayGuiSound("locked_door", 0.7f);
	SetMessage("Ch02Level16", "InteractLargeDoor", 0);
	
	AddTimer(asEntity, 1.0f, "TimerGruntDoorBackon");
}
public void TimerGruntDoorBackon(string  asTimer)
{
	SetEntityPlayerInteractCallback(asTimer, "Interactmansion_2", true);
}

/*Event timer for the cavein
 */
public void TimerCaveIn(string  asTimer)
{
	if(asTimer == "1"){
		CreateParticleSystemAtEntity("cavein", "ps_break_cavein_local", "AreaCaveIn", false);
		PlaySoundAtEntity("caveins", "explosion_rock_large", "cave_in_cave_in_1", 0.0f, false);
	}
	else if(asTimer == "2"){
		PlayGuiSound("react_scare6", 1.0f);
		GiveSanityDamage(10.0f, true);
		PlayMusic("11_event_tree", false, 1.0f, 0.0f, 10, false);
	}
	else if(asTimer == "3"){
		StopSound("rumble", 3.0f);
		StopMusic(2.0f, 10);
	}
	else if(asTimer == "4"){
		PlaySoundAtEntity("caveins", "15_rock_break", "cave_in_cave_in_1", 0.0f, false);
		PlayGuiSound("react_breath.snt", 1.0f);	
	}
}

/*If going to examine the cavein, give Quest
 */
public void CollideCaveInQuest(string  asParent, string  asChild, int alState)
{
	AddQuest("03cavein", "03CaveIn");
}

/*When leaving the library, complete quest and activate some Grunt event areas
 */
public void CollideQuestDone(string  asParent, string  asChild, int alState)
{
	CompleteQuest("03cavein", "03CaveIn");
	
	PlaySoundAtEntity("growl", "03_amb_idle", "AreaGrunt_3", 0.5f, false);
	
	SetEntityActive("AreaActivateLiveGrunt_1", true);
	SetEntityActive("AreaActivateLiveGrunt_2", true);
}


//END CAVEIN AT END OF LEVEL//
//////////////////////////////


//////////////////////////////////////
//START A GRUNT WALKING IN THE LEVEL//
/*A grunt is shown going into another room when coming into the room, two different places but only 1 will be used
 */
public void CollideActivateLiveGrunt(string  asParent, string  asChild, int alState)
{
	if(asChild == "AreaActivateLiveGrunt_1"){ 
		SetEntityActive("servant_grunt_1", true);
		SetEntityActive("AreaActivateLiveGrunt_2", false);
	} else{
		SetEntityActive("servant_grunt_2", true);
		SetEntityActive("AreaActivateLiveGrunt_1", false);
	}
	
	SetPlayerRunSpeedMul(0.9f);
	SetPlayerMoveSpeedMul(0.9f);
	
	AddTimer("scare", 1.0f, "TimerGruntScare");
	AddTimer("breath", 3.0f, "TimerGruntScare");
}

/*Give sanity damage on grunt activation
 */
public void TimerGruntScare(string  asTimer)
{
	if(asTimer == "breath"){
		PlayGuiSound("react_breath", 0.6f);
		SetPlayerRunSpeedMul(1.0f);
		SetPlayerMoveSpeedMul(1.0f);
		return;
	}
	
	GiveSanityDamage(10.0f, true);
	PlayGuiSound("react_scare6", 0.7f);
	SetPlayerRunSpeedMul(0.8f);
	SetPlayerMoveSpeedMul(0.8f);
}

/*Remove the grunt as soon as it is in another room and out of sight
 */
public void CollideDisableLiveGrunt(string  asParent, string  asChild, int alState)
{
	SetEntityActive(asParent, false);
}

/*If player goes to examine cavein from the other way around, a grunt appears behind him
 */
public void CollideJumpGrunt(string  asParent, string  asChild, int alState)
{
	if(alState == 1) SetEntityActive("AreaLookGrunt", true);
	else SetEntityActive("AreaLookGrunt", false);
}

/*To make sure that grunt is only activated when looking away
 */
public void LookAtGrunt(string  asEntity, int alState)
{
	SetEntityActive("servant_grunt_3", true);
	AddTimer("scare", 0.5f, "TimerGruntScare");
}
//END A GRUNT WALKING IN THE LEVEL//
////////////////////////////////////


//////////////////////////////////////
//START GRUNT SOUNDS

public void CollideGruntSound01(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("grunt_idle","amb_idle_whimp.snt","AreaDustScrape_3",0,false);
}

public void CollideGruntSound02(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("grunt_idle","amb_idle_whimp.snt","AreaCaveIn2",0,false);
}

public void CollideGruntSound03(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("grunt_idle","amb_idle_whimp.snt","AreaGruntSoundBehind",0,false);
}

public void TimerIntroEnemy(string  asTimer)
{
	PlaySoundAtEntity("enemy", "enemy/grunt/amb_idle",  "AreaDustScrape_5", 0, false);
}

//END GRUNT SOUNDS
//////////////////////////////////////


////////////////////////////
// Run first time starting map
public override void OnStart()
{
	SetMapDisplayNameEntry("Archives");
	
	//----COLLIDE CALLBACKS----//
	for(int i=1;i<=3;i++) AddEntityCollideCallback("SecretBook_"+i, "AreaSecretBook_"+i, "CollideSecretBook", false, 0);	//Pull books to reveal room
	AddEntityCollideCallback("Player", "AreaTriggerManyBook", "CollideBeginManyBook", true, 1);	//Makes the shelf bang and books fly
	AddEntityCollideCallback("Player", "AreaTriggerPiano", "CollideBeginPiano", true, 1);	//Some distant piano is playing and a book falls to the ground
	AddEntityCollideCallback("Player", "SlowArea_1", "CollideSlowArea", false, 0);	
	AddEntityCollideCallback("Player", "AreaSecondPiano", "CollideBeginPiano", true, 1);
	AddEntityCollideCallback("Player", "AreaClosePiano", "CollideCloseLid", true, 1);
	AddEntityCollideCallback("vase02_ghost_1", "AreaJar", "CollideJarLeaveArea", true, -1);
	AddEntityCollideCallback("Player", "AreaHalfToOrb", "CollideOrb", true, 1);
	AddEntityCollideCallback("Player", "AreaCloseToOrb", "CollideOrb", true, 1);
	AddEntityCollideCallback("chair_ghost", "mansionbase_secret_passage_1", "CollideHint", true, 1);			
	for(int i=1;i<=7;i++) AddEntityCollideCallback("rock_small_"+i, "mansionbase_secret_passage_1", "CollideHint", true, 1);
	for(int i=9;i<=12;i++) AddEntityCollideCallback("chair_nice01_"+i, "mansionbase_secret_passage_1", "CollideHint", true, 1);
	
	AddEntityCollideCallback("Player", "AreaCloudActivate_5", "CollideCaveIn", true, 1);
	AddEntityCollideCallback("Player", "AreaCaveInQuest", "CollideCaveInQuest", true, 1);
	AddEntityCollideCallback("Player", "AreaQuestDone", "CollideQuestDone", true, 1);
	
	AddEntityCollideCallback("Player", "AreaActivateLiveGrunt_1", "CollideActivateLiveGrunt", true, 1);
	AddEntityCollideCallback("Player", "AreaActivateLiveGrunt_2", "CollideActivateLiveGrunt", true, 1);
	
	AddEntityCollideCallback("Player", "AreaGruntSound01", "CollideGruntSound01", true, 1);
	AddEntityCollideCallback("Player", "AreaGruntSound02", "CollideGruntSound02", true, 1);
	AddEntityCollideCallback("Player", "AreaGruntSound03", "CollideGruntSound03", true, 1);
	
	AddEntityCollideCallback("servant_grunt_1", "AreaActivateLiveGrunt_3", "CollideDisableLiveGrunt", true, 1);
	AddEntityCollideCallback("servant_grunt_2", "AreaActivateLiveGrunt_3", "CollideDisableLiveGrunt", true, 1);
	
	AddEntityCollideCallback("Player", "AreaLookGrunt_1", "CollideJumpGrunt", false, 0);
	
	//----PLAYER INTERACTION----//
	SetEntityPlayerInteractCallback("PassageInteractArea", "PlayerInteractSecretPassage", false);
	
	//----PATROL FOR MONSTERS----//
	AddEnemyPatrolNode("servant_grunt_1", "PathNodeArea_5", 0.0f, "");
	AddEnemyPatrolNode("servant_grunt_2", "PathNodeArea_10", 0.0f, "");
	AddEnemyPatrolNode("servant_grunt_3", "PathNodeArea_14", 0.0f, "");
	
	
	//NOT IN USE 
	//for(int i=2;i<=5;i++)  AddEntityCollideCallback("Player", "AreaCloudActivate_"+i, "CollideCloudThing", true, 1);
	//----INTRO----//
	//CloudEffects("AreaCloudThing_1", true);
	
	SetEntityCallbackFunc("note_paper01_2", "PickVillageNote");
	
	//SetSkyBoxActive(true);
	//SetSkyBoxColor(0.25f, 0.30f, 0.35f, 1);
	
	//----ENTITY INIT----//
	SetPropHealth("display_3", 0);
	
	PlaySoundAtEntity("ambs1", "03_amb.snt", "AmbSounds", 4.0f, true);
	PlaySoundAtEntity("ambs2", "03_amb_library.snt", "AmbSounds", 4.0f, true);
	PlaySoundAtEntity("ambs3", "03_insects.snt", "AmbSounds", 4.0f, true);
	
	//----CONNECT CALLBACKS----//
	/*Secret hole in wall
	 */
	ConnectEntities("secret_painting",		//Name of connection
		"lever_small01", 	//Parent entity (Affects)
		"painting_large03_1",	//Child entity 	(Affected) 
		false,		//Invert the state sent
		1, 		//States used (0=both), checked before invertion.
		"");	//callback

	
	//----QUEST INIT----//
	SetNumberOfQuestsInMap(1);
	
	//----TIMER INIT----//
	AddTimer("crawl", 0.5f, "TimerCrawl");
	AddTimer("blooddrop", 1, "TimerBloodDrop");
	AddTimer("enemyS", 4.0f, "TimerIntroEnemy");
	
	//----VARIABLES----//
	SetLocalVarInt("VoiceResume", 0);
	SetGlobalVarInt("PlayerBeenInLevel03",1);
	
	//----DEBUG----//
	if(ScriptDebugOn())
	{
		GiveItemFromFile("lantern", "lantern.ent");
		for(int i=0;i<10;i++) GiveItemFromFile("tinderbox_"+i, "tinderbox.ent");
		
		//CollideCaveIn("", "", 1);	//to test cavein
		
		AddLocalVarInt("diary", 2);	//Activate flashback on first diary pickup
		//AddTimer("TimerFlashBack", 0.1f, "TimerFlashBack");	//To use when tetsing with start loction _Flash
		//SetPlayerLampOil(15);	//To test always give oil during flashback
	}
}

////////////////////////////
// Run when entering map
public override void OnEnter()
{
    Finder.Bufferize("InitUniqueRandom", "DiaryFlashbackActive", "diary", "startposition", "Start_0", "Start_1", "Start_2", "Start_3", "Start_4", "Start_5", "Start_6", "Start_7", "Start_8", "Start_9", "Start_10", "Start_11", "Start_12", "Start_13", "Start_14", "Start_15", "Start_16", "Start_17", "Start_18", "Start_19", "Start_20", "Start_21", "Start_22", "Start_23", "Start_24", "Start_25", "Start_26", "Start_27", "Start_28", "Start_29", "Start_30", "Start_31", "Start_32", "Start_33", "Start_34", "Start_35", "Start_36", "Start_37", "Start_38", "Start_39", "Start_40", "Start_41", "Start_42", "Start_43", "Start_44", "Start_45", "Start_46", "Start_47", "Start_48", "Start_49", "Start_50", "Start_51", "Start_52", "Start_53", "Start_54", "Start_55", "Start_56", "Start_57", "Start_58", "Start_59", "Start_60", "Start_61", "Start_62", "Start_63", "Start_64", "Start_65", "Start_66", "Start_67", "Start_68", "Start_69", "Start_70", "Start_71", "Start_72", "Start_73", "Start_74", "Start_75", "Start_76", "Start_77", "Start_78", "Start_79", "Start_80", "Start_81", "Start_82", "Start_83", "Start_84", "Start_85", "Start_86", "Start_87", "Start_88", "Start_89", "Start_90", "Start_91", "Start_92", "Start_93", "Start_94", "Start_95", "Start_96", "Start_97", "Start_98", "Start_99", "TimerFlashBack", "Got Diary and Int is: ", "CH01L03_DanielDiary03_05", "", "Flashbacks", "Interacted with ORB", "LanternActive", "SoundFlash1", "Player", "creak", "Breath1", "CH01L03_DanielDiary03_01", "DiaryPartsOver", "extrap", "TimerExtraVoiceEffects", "extrap2", "extrap3", "extrap4", "extrap5", "ambs", "AreaLookUp", "rock_amb", "VoiceResume", "AreaRock_4", "CH01L03_DanielDiary03_02", "loop", "randrock", "TimerRandRock", "extra1", "extra2", "extra3", "PlayerStartArea_Flash", "orb_1", "orb_loop", "lookat", "CH01L03_DanielDiary03_03", "extra4", "extra5", "extra6", "OrbLight", "CH01L03_DanielDiary03_06", "DiaryPartsLast", "orb_loop2", "player_fall", "BreakOrb", "candlestick_tri_5", "ambs1", "AmbSounds", "ambs2", "ambs3", "music", "Breath3", "pearls", "lookloop", "TimerRandomLook", "pound1", "pound2", "pound3", "gasp1", "player_push", "extra7", "CH01L03_DanielDiary03_07", "rock", "drag1", "drag2", "drag3", "extra8", "crew", "pianoonplayer", "Resume with CH01L03_DanielDiary03_0", "AreaHalfToOrb", "CH01L03_DanielDiary03_04", "AreaCloseToOrb", "LookAtOrb", "InteractOrb", "AreaOrbX", "CollideActualOrb", "hitpa", "SlowArea_1", "SlowArea_2", "SlowArea_3", "Player enter ", "FirstEnterSlowArea", "CollideSlowArea", "SlowArea_4", "Player leave ", "RockSound", "AreaRock_0", "AreaRock_1", "AreaRock_2", "AreaRock_3", "AreaRock_5", "AreaRock_6", "AreaRock_7", "AreaRock_8", "AreaRock_9", "AreaRock_10", "AreaRock_11", "AreaRock_12", "AreaRock_13", "AreaRock_14", "AreaRock_15", "AreaRock_16", "AreaRock_17", "AreaRock_18", "AreaRock_19", "AreaRock_20", "AreaRock_21", "AreaRock_22", "AreaRock_23", "AreaRock_24", "AreaRock_25", "AreaRock_26", "AreaRock_27", "AreaRock_28", "AreaRock_29", "AreaRock_30", "AreaRock_31", "AreaRock_32", "AreaRock_33", "AreaRock_34", "AreaRock_35", "AreaRock_36", "AreaRock_37", "AreaRock_38", "AreaRock_39", "AreaRock_40", "AreaRock_41", "AreaRock_42", "AreaRock_43", "AreaRock_44", "AreaRock_45", "AreaRock_46", "AreaRock_47", "AreaRock_48", "AreaRock_49", "AreaRock_50", "AreaRock_51", "AreaRock_52", "AreaRock_53", "AreaRock_54", "AreaRock_55", "AreaRock_56", "AreaRock_57", "AreaRock_58", "AreaRock_59", "AreaRock_60", "AreaRock_61", "AreaRock_62", "AreaRock_63", "AreaRock_64", "AreaRock_65", "AreaRock_66", "AreaRock_67", "AreaRock_68", "AreaRock_69", "AreaRock_70", "AreaRock_71", "AreaRock_72", "AreaRock_73", "AreaRock_74", "AreaRock_75", "AreaRock_76", "AreaRock_77", "AreaRock_78", "AreaRock_79", "AreaRock_80", "AreaRock_81", "AreaRock_82", "AreaRock_83", "AreaRock_84", "AreaRock_85", "AreaRock_86", "AreaRock_87", "AreaRock_88", "AreaRock_89", "AreaRock_90", "AreaRock_91", "AreaRock_92", "AreaRock_93", "AreaRock_94", "AreaRock_95", "AreaRock_96", "AreaRock_97", "AreaRock_98", "AreaRock_99", "Sound in AreaRock_0", "Sound in AreaRock_1", "Sound in AreaRock_2", "Sound in AreaRock_3", "Sound in AreaRock_4", "Sound in AreaRock_5", "Sound in AreaRock_6", "Sound in AreaRock_7", "Sound in AreaRock_8", "Sound in AreaRock_9", "Sound in AreaRock_10", "Sound in AreaRock_11", "Sound in AreaRock_12", "Sound in AreaRock_13", "Sound in AreaRock_14", "Sound in AreaRock_15", "Sound in AreaRock_16", "Sound in AreaRock_17", "Sound in AreaRock_18", "Sound in AreaRock_19", "Sound in AreaRock_20", "Sound in AreaRock_21", "Sound in AreaRock_22", "Sound in AreaRock_23", "Sound in AreaRock_24", "Sound in AreaRock_25", "Sound in AreaRock_26", "Sound in AreaRock_27", "Sound in AreaRock_28", "Sound in AreaRock_29", "Sound in AreaRock_30", "Sound in AreaRock_31", "Sound in AreaRock_32", "Sound in AreaRock_33", "Sound in AreaRock_34", "Sound in AreaRock_35", "Sound in AreaRock_36", "Sound in AreaRock_37", "Sound in AreaRock_38", "Sound in AreaRock_39", "Sound in AreaRock_40", "Sound in AreaRock_41", "Sound in AreaRock_42", "Sound in AreaRock_43", "Sound in AreaRock_44", "Sound in AreaRock_45", "Sound in AreaRock_46", "Sound in AreaRock_47", "Sound in AreaRock_48", "Sound in AreaRock_49", "Sound in AreaRock_50", "Sound in AreaRock_51", "Sound in AreaRock_52", "Sound in AreaRock_53", "Sound in AreaRock_54", "Sound in AreaRock_55", "Sound in AreaRock_56", "Sound in AreaRock_57", "Sound in AreaRock_58", "Sound in AreaRock_59", "Sound in AreaRock_60", "Sound in AreaRock_61", "Sound in AreaRock_62", "Sound in AreaRock_63", "Sound in AreaRock_64", "Sound in AreaRock_65", "Sound in AreaRock_66", "Sound in AreaRock_67", "Sound in AreaRock_68", "Sound in AreaRock_69", "Sound in AreaRock_70", "Sound in AreaRock_71", "Sound in AreaRock_72", "Sound in AreaRock_73", "Sound in AreaRock_74", "Sound in AreaRock_75", "Sound in AreaRock_76", "Sound in AreaRock_77", "Sound in AreaRock_78", "Sound in AreaRock_79", "Sound in AreaRock_80", "Sound in AreaRock_81", "Sound in AreaRock_82", "Sound in AreaRock_83", "Sound in AreaRock_84", "Sound in AreaRock_85", "Sound in AreaRock_86", "Sound in AreaRock_87", "Sound in AreaRock_88", "Sound in AreaRock_89", "Sound in AreaRock_90", "Sound in AreaRock_91", "Sound in AreaRock_92", "Sound in AreaRock_93", "Sound in AreaRock_94", "Sound in AreaRock_95", "Sound in AreaRock_96", "Sound in AreaRock_97", "Sound in AreaRock_98", "Sound in AreaRock_99", "sigh", "Dizzy", "SoundScratchx_1", "scare_scratch", "throwbook_13", "manybooks", "TimerManyBooks", "Begin Many Books Scare ", "VarMany", "PSGhostBang", "throwbook_14", "SoundBang", "SoundFearx2", "throwbook_0", "throwbook_1", "throwbook_2", "throwbook_3", "throwbook_4", "throwbook_5", "throwbook_6", "throwbook_7", "throwbook_8", "throwbook_9", "throwbook_10", "throwbook_11", "throwbook_12", "throwbook_15", "throwbook_16", "throwbook_17", "throwbook_18", "throwbook_19", "throwbook_20", "throwbook_21", "throwbook_22", "throwbook_23", "throwbook_24", "throwbook_25", "throwbook_26", "throwbook_27", "throwbook_28", "throwbook_29", "throwbook_30", "throwbook_31", "throwbook_32", "throwbook_33", "throwbook_34", "throwbook_35", "throwbook_36", "throwbook_37", "throwbook_38", "throwbook_39", "throwbook_40", "throwbook_41", "throwbook_42", "throwbook_43", "throwbook_44", "throwbook_45", "throwbook_46", "throwbook_47", "throwbook_48", "throwbook_49", "throwbook_50", "throwbook_51", "throwbook_52", "throwbook_53", "throwbook_54", "throwbook_55", "throwbook_56", "throwbook_57", "throwbook_58", "throwbook_59", "throwbook_60", "throwbook_61", "throwbook_62", "throwbook_63", "throwbook_64", "throwbook_65", "throwbook_66", "throwbook_67", "throwbook_68", "throwbook_69", "throwbook_70", "throwbook_71", "throwbook_72", "throwbook_73", "throwbook_74", "throwbook_75", "throwbook_76", "throwbook_77", "throwbook_78", "throwbook_79", "throwbook_80", "throwbook_81", "throwbook_82", "throwbook_83", "throwbook_84", "throwbook_85", "throwbook_86", "throwbook_87", "throwbook_88", "throwbook_89", "throwbook_90", "throwbook_91", "throwbook_92", "throwbook_93", "throwbook_94", "throwbook_95", "throwbook_96", "throwbook_97", "throwbook_98", "throwbook_99", "world", "BoxLight_4", "throwbook_*", "AreaSecondPiano", "piano_2", "piano_2_BodyLid", "SoundPiano2", "AreaPlayPiano", "SoundWind", "scare_wall_scratch_single", "chair_nice01_12", "chair_ghost", "fallbook", "FallBook", "SoundPiano", "piano_1", "tome01_8", "piano_1_BodyLid", "fallbook1", "fallbook2", "PSdust", "AreaPianoDust", "liddust", "LidClose", "enemy", "enemy/grunt/amb_idle", "AreaDustScrape_6", "PSdustbook", "doormess", "TimerLockedDoorMess", "Ch01Level03", "DoorLocked", "03LockedLibrary", "PassageInteractArea", "mansionbase_secret_passage_1", "Var", "PushBackBook", "Sound", "gameplay_tick", "16_lever_stuck", "Book in area: ", "Sound2", "lock_door", "Book out of area: ", "VarSecretBook_1", "VarSecretBook_2", "VarSecretBook_3", "PointLight_30", "shelf_secret_door_1", "SecretBook_*", "BooksDone", "dust", "ps_dust_falling_door_quick", "AreaDoorParticle", "SecretBook_0", "SecretBook_1", "SecretBook_2", "SecretBook_3", "SecretBook_4", "SecretBook_5", "SecretBook_6", "SecretBook_7", "SecretBook_8", "SecretBook_9", "SecretBook_10", "SecretBook_11", "SecretBook_12", "SecretBook_13", "SecretBook_14", "SecretBook_15", "SecretBook_16", "SecretBook_17", "SecretBook_18", "SecretBook_19", "SecretBook_20", "SecretBook_21", "SecretBook_22", "SecretBook_23", "SecretBook_24", "SecretBook_25", "SecretBook_26", "SecretBook_27", "SecretBook_28", "SecretBook_29", "SecretBook_30", "SecretBook_31", "SecretBook_32", "SecretBook_33", "SecretBook_34", "SecretBook_35", "SecretBook_36", "SecretBook_37", "SecretBook_38", "SecretBook_39", "SecretBook_40", "SecretBook_41", "SecretBook_42", "SecretBook_43", "SecretBook_44", "SecretBook_45", "SecretBook_46", "SecretBook_47", "SecretBook_48", "SecretBook_49", "SecretBook_50", "SecretBook_51", "SecretBook_52", "SecretBook_53", "SecretBook_54", "SecretBook_55", "SecretBook_56", "SecretBook_57", "SecretBook_58", "SecretBook_59", "SecretBook_60", "SecretBook_61", "SecretBook_62", "SecretBook_63", "SecretBook_64", "SecretBook_65", "SecretBook_66", "SecretBook_67", "SecretBook_68", "SecretBook_69", "SecretBook_70", "SecretBook_71", "SecretBook_72", "SecretBook_73", "SecretBook_74", "SecretBook_75", "SecretBook_76", "SecretBook_77", "SecretBook_78", "SecretBook_79", "SecretBook_80", "SecretBook_81", "SecretBook_82", "SecretBook_83", "SecretBook_84", "SecretBook_85", "SecretBook_86", "SecretBook_87", "SecretBook_88", "SecretBook_89", "SecretBook_90", "SecretBook_91", "SecretBook_92", "SecretBook_93", "SecretBook_94", "SecretBook_95", "SecretBook_96", "SecretBook_97", "SecretBook_98", "SecretBook_99", "SoundSecretBook_0", "SoundSecretBook_1", "SoundSecretBook_2", "SoundSecretBook_3", "SoundSecretBook_4", "SoundSecretBook_5", "SoundSecretBook_6", "SoundSecretBook_7", "SoundSecretBook_8", "SoundSecretBook_9", "SoundSecretBook_10", "SoundSecretBook_11", "SoundSecretBook_12", "SoundSecretBook_13", "SoundSecretBook_14", "SoundSecretBook_15", "SoundSecretBook_16", "SoundSecretBook_17", "SoundSecretBook_18", "SoundSecretBook_19", "SoundSecretBook_20", "SoundSecretBook_21", "SoundSecretBook_22", "SoundSecretBook_23", "SoundSecretBook_24", "SoundSecretBook_25", "SoundSecretBook_26", "SoundSecretBook_27", "SoundSecretBook_28", "SoundSecretBook_29", "SoundSecretBook_30", "SoundSecretBook_31", "SoundSecretBook_32", "SoundSecretBook_33", "SoundSecretBook_34", "SoundSecretBook_35", "SoundSecretBook_36", "SoundSecretBook_37", "SoundSecretBook_38", "SoundSecretBook_39", "SoundSecretBook_40", "SoundSecretBook_41", "SoundSecretBook_42", "SoundSecretBook_43", "SoundSecretBook_44", "SoundSecretBook_45", "SoundSecretBook_46", "SoundSecretBook_47", "SoundSecretBook_48", "SoundSecretBook_49", "SoundSecretBook_50", "SoundSecretBook_51", "SoundSecretBook_52", "SoundSecretBook_53", "SoundSecretBook_54", "SoundSecretBook_55", "SoundSecretBook_56", "SoundSecretBook_57", "SoundSecretBook_58", "SoundSecretBook_59", "SoundSecretBook_60", "SoundSecretBook_61", "SoundSecretBook_62", "SoundSecretBook_63", "SoundSecretBook_64", "SoundSecretBook_65", "SoundSecretBook_66", "SoundSecretBook_67", "SoundSecretBook_68", "SoundSecretBook_69", "SoundSecretBook_70", "SoundSecretBook_71", "SoundSecretBook_72", "SoundSecretBook_73", "SoundSecretBook_74", "SoundSecretBook_75", "SoundSecretBook_76", "SoundSecretBook_77", "SoundSecretBook_78", "SoundSecretBook_79", "SoundSecretBook_80", "SoundSecretBook_81", "SoundSecretBook_82", "SoundSecretBook_83", "SoundSecretBook_84", "SoundSecretBook_85", "SoundSecretBook_86", "SoundSecretBook_87", "SoundSecretBook_88", "SoundSecretBook_89", "SoundSecretBook_90", "SoundSecretBook_91", "SoundSecretBook_92", "SoundSecretBook_93", "SoundSecretBook_94", "SoundSecretBook_95", "SoundSecretBook_96", "SoundSecretBook_97", "SoundSecretBook_98", "SoundSecretBook_99", "03Books", "All books in position, move shelf!", "2", "PushBackBook02", "Push back book: ", "2SecretBook_1", "2SecretBook_2", "InteractMoveShelf", "enemys", "TimerPlayEnemy", "MoveShelf", "AreaActivateLastPiano", "CollideLastPiano", "TimerEGrunt", "growl", "03_amb_idle", "AreaGrunt_1", "03_attack_claw", "mansion_1", "mansion_8", "hitdoor", "break_wood", "breakdoor", "15_event_prisoner", "react_scare", "react_breath", "AreaGrunt_2", "TimerThing", "react1", "thing1", "thing2", "thing3", "thing4", "AreaCloudActivate_2", "AreaCloudThing_2", "AreaCloudActivate_3", "AreaDustScrape_3", "AreaCloudActivate_4", "AreaDustScrape_8", "AreaCloudActivate_5", "AreaCloudThing_5", "Do cloud ", "clouds1", "ghost_released", "03_in_a_bottle", "SoundPiano3", "AreaGrunt_4", "Play last piano", "bloodsound", "bloddrop", "general_blood_drop", "AreaBloodDrop", "blooddrop", "TimerBloodDrop", "PSblood", "AreaBloodDrop_1", "crawlPS", "AreaDustScrape_0", "AreaDustScrape_1", "AreaDustScrape_2", "AreaDustScrape_4", "AreaDustScrape_5", "AreaDustScrape_7", "AreaDustScrape_9", "AreaDustScrape_10", "AreaDustScrape_11", "AreaDustScrape_12", "AreaDustScrape_13", "AreaDustScrape_14", "AreaDustScrape_15", "AreaDustScrape_16", "AreaDustScrape_17", "AreaDustScrape_18", "AreaDustScrape_19", "AreaDustScrape_20", "AreaDustScrape_21", "AreaDustScrape_22", "AreaDustScrape_23", "AreaDustScrape_24", "AreaDustScrape_25", "AreaDustScrape_26", "AreaDustScrape_27", "AreaDustScrape_28", "AreaDustScrape_29", "AreaDustScrape_30", "AreaDustScrape_31", "AreaDustScrape_32", "AreaDustScrape_33", "AreaDustScrape_34", "AreaDustScrape_35", "AreaDustScrape_36", "AreaDustScrape_37", "AreaDustScrape_38", "AreaDustScrape_39", "AreaDustScrape_40", "AreaDustScrape_41", "AreaDustScrape_42", "AreaDustScrape_43", "AreaDustScrape_44", "AreaDustScrape_45", "AreaDustScrape_46", "AreaDustScrape_47", "AreaDustScrape_48", "AreaDustScrape_49", "AreaDustScrape_50", "AreaDustScrape_51", "AreaDustScrape_52", "AreaDustScrape_53", "AreaDustScrape_54", "AreaDustScrape_55", "AreaDustScrape_56", "AreaDustScrape_57", "AreaDustScrape_58", "AreaDustScrape_59", "AreaDustScrape_60", "AreaDustScrape_61", "AreaDustScrape_62", "AreaDustScrape_63", "AreaDustScrape_64", "AreaDustScrape_65", "AreaDustScrape_66", "AreaDustScrape_67", "AreaDustScrape_68", "AreaDustScrape_69", "AreaDustScrape_70", "AreaDustScrape_71", "AreaDustScrape_72", "AreaDustScrape_73", "AreaDustScrape_74", "AreaDustScrape_75", "AreaDustScrape_76", "AreaDustScrape_77", "AreaDustScrape_78", "AreaDustScrape_79", "AreaDustScrape_80", "AreaDustScrape_81", "AreaDustScrape_82", "AreaDustScrape_83", "AreaDustScrape_84", "AreaDustScrape_85", "AreaDustScrape_86", "AreaDustScrape_87", "AreaDustScrape_88", "AreaDustScrape_89", "AreaDustScrape_90", "AreaDustScrape_91", "AreaDustScrape_92", "AreaDustScrape_93", "AreaDustScrape_94", "AreaDustScrape_95", "AreaDustScrape_96", "AreaDustScrape_97", "AreaDustScrape_98", "AreaDustScrape_99", "crawlSound", "crawl", "TimerCrawl", "Now crawling in: ", " Next crawl in: ", "15_rock_break", "BrokenWall", "TimerBrokenWall", "InteractBrokenWall", "ThrowHint", "Hints", "Collide!", "PassageHitCount", "TimerDecreaseHitCount", "PassageBreakMessagePaused", "PSPassageDebris", "PassageInteractDebris_0", "PassageInteractDebris_1", "PassageInteractDebris_2", "PassageInteractDebris_3", "PassageInteractDebris_4", "PassageInteractDebris_5", "PassageInteractDebris_6", "PassageInteractDebris_7", "PassageInteractDebris_8", "PassageInteractDebris_9", "PassageInteractDebris_10", "PassageInteractDebris_11", "PassageInteractDebris_12", "PassageInteractDebris_13", "PassageInteractDebris_14", "PassageInteractDebris_15", "PassageInteractDebris_16", "PassageInteractDebris_17", "PassageInteractDebris_18", "PassageInteractDebris_19", "PassageInteractDebris_20", "PassageInteractDebris_21", "PassageInteractDebris_22", "PassageInteractDebris_23", "PassageInteractDebris_24", "PassageInteractDebris_25", "PassageInteractDebris_26", "PassageInteractDebris_27", "PassageInteractDebris_28", "PassageInteractDebris_29", "PassageInteractDebris_30", "PassageInteractDebris_31", "PassageInteractDebris_32", "PassageInteractDebris_33", "PassageInteractDebris_34", "PassageInteractDebris_35", "PassageInteractDebris_36", "PassageInteractDebris_37", "PassageInteractDebris_38", "PassageInteractDebris_39", "PassageInteractDebris_40", "PassageInteractDebris_41", "PassageInteractDebris_42", "PassageInteractDebris_43", "PassageInteractDebris_44", "PassageInteractDebris_45", "PassageInteractDebris_46", "PassageInteractDebris_47", "PassageInteractDebris_48", "PassageInteractDebris_49", "PassageInteractDebris_50", "PassageInteractDebris_51", "PassageInteractDebris_52", "PassageInteractDebris_53", "PassageInteractDebris_54", "PassageInteractDebris_55", "PassageInteractDebris_56", "PassageInteractDebris_57", "PassageInteractDebris_58", "PassageInteractDebris_59", "PassageInteractDebris_60", "PassageInteractDebris_61", "PassageInteractDebris_62", "PassageInteractDebris_63", "PassageInteractDebris_64", "PassageInteractDebris_65", "PassageInteractDebris_66", "PassageInteractDebris_67", "PassageInteractDebris_68", "PassageInteractDebris_69", "PassageInteractDebris_70", "PassageInteractDebris_71", "PassageInteractDebris_72", "PassageInteractDebris_73", "PassageInteractDebris_74", "PassageInteractDebris_75", "PassageInteractDebris_76", "PassageInteractDebris_77", "PassageInteractDebris_78", "PassageInteractDebris_79", "PassageInteractDebris_80", "PassageInteractDebris_81", "PassageInteractDebris_82", "PassageInteractDebris_83", "PassageInteractDebris_84", "PassageInteractDebris_85", "PassageInteractDebris_86", "PassageInteractDebris_87", "PassageInteractDebris_88", "PassageInteractDebris_89", "PassageInteractDebris_90", "PassageInteractDebris_91", "PassageInteractDebris_92", "PassageInteractDebris_93", "PassageInteractDebris_94", "PassageInteractDebris_95", "PassageInteractDebris_96", "PassageInteractDebris_97", "PassageInteractDebris_98", "PassageInteractDebris_99", "Adding debris PS to PassageInteractDebris_0", "Adding debris PS to PassageInteractDebris_1", "Adding debris PS to PassageInteractDebris_2", "Adding debris PS to PassageInteractDebris_3", "Adding debris PS to PassageInteractDebris_4", "Adding debris PS to PassageInteractDebris_5", "Adding debris PS to PassageInteractDebris_6", "Adding debris PS to PassageInteractDebris_7", "Adding debris PS to PassageInteractDebris_8", "Adding debris PS to PassageInteractDebris_9", "Adding debris PS to PassageInteractDebris_10", "Adding debris PS to PassageInteractDebris_11", "Adding debris PS to PassageInteractDebris_12", "Adding debris PS to PassageInteractDebris_13", "Adding debris PS to PassageInteractDebris_14", "Adding debris PS to PassageInteractDebris_15", "Adding debris PS to PassageInteractDebris_16", "Adding debris PS to PassageInteractDebris_17", "Adding debris PS to PassageInteractDebris_18", "Adding debris PS to PassageInteractDebris_19", "Adding debris PS to PassageInteractDebris_20", "Adding debris PS to PassageInteractDebris_21", "Adding debris PS to PassageInteractDebris_22", "Adding debris PS to PassageInteractDebris_23", "Adding debris PS to PassageInteractDebris_24", "Adding debris PS to PassageInteractDebris_25", "Adding debris PS to PassageInteractDebris_26", "Adding debris PS to PassageInteractDebris_27", "Adding debris PS to PassageInteractDebris_28", "Adding debris PS to PassageInteractDebris_29", "Adding debris PS to PassageInteractDebris_30", "Adding debris PS to PassageInteractDebris_31", "Adding debris PS to PassageInteractDebris_32", "Adding debris PS to PassageInteractDebris_33", "Adding debris PS to PassageInteractDebris_34", "Adding debris PS to PassageInteractDebris_35", "Adding debris PS to PassageInteractDebris_36", "Adding debris PS to PassageInteractDebris_37", "Adding debris PS to PassageInteractDebris_38", "Adding debris PS to PassageInteractDebris_39", "Adding debris PS to PassageInteractDebris_40", "Adding debris PS to PassageInteractDebris_41", "Adding debris PS to PassageInteractDebris_42", "Adding debris PS to PassageInteractDebris_43", "Adding debris PS to PassageInteractDebris_44", "Adding debris PS to PassageInteractDebris_45", "Adding debris PS to PassageInteractDebris_46", "Adding debris PS to PassageInteractDebris_47", "Adding debris PS to PassageInteractDebris_48", "Adding debris PS to PassageInteractDebris_49", "Adding debris PS to PassageInteractDebris_50", "Adding debris PS to PassageInteractDebris_51", "Adding debris PS to PassageInteractDebris_52", "Adding debris PS to PassageInteractDebris_53", "Adding debris PS to PassageInteractDebris_54", "Adding debris PS to PassageInteractDebris_55", "Adding debris PS to PassageInteractDebris_56", "Adding debris PS to PassageInteractDebris_57", "Adding debris PS to PassageInteractDebris_58", "Adding debris PS to PassageInteractDebris_59", "Adding debris PS to PassageInteractDebris_60", "Adding debris PS to PassageInteractDebris_61", "Adding debris PS to PassageInteractDebris_62", "Adding debris PS to PassageInteractDebris_63", "Adding debris PS to PassageInteractDebris_64", "Adding debris PS to PassageInteractDebris_65", "Adding debris PS to PassageInteractDebris_66", "Adding debris PS to PassageInteractDebris_67", "Adding debris PS to PassageInteractDebris_68", "Adding debris PS to PassageInteractDebris_69", "Adding debris PS to PassageInteractDebris_70", "Adding debris PS to PassageInteractDebris_71", "Adding debris PS to PassageInteractDebris_72", "Adding debris PS to PassageInteractDebris_73", "Adding debris PS to PassageInteractDebris_74", "Adding debris PS to PassageInteractDebris_75", "Adding debris PS to PassageInteractDebris_76", "Adding debris PS to PassageInteractDebris_77", "Adding debris PS to PassageInteractDebris_78", "Adding debris PS to PassageInteractDebris_79", "Adding debris PS to PassageInteractDebris_80", "Adding debris PS to PassageInteractDebris_81", "Adding debris PS to PassageInteractDebris_82", "Adding debris PS to PassageInteractDebris_83", "Adding debris PS to PassageInteractDebris_84", "Adding debris PS to PassageInteractDebris_85", "Adding debris PS to PassageInteractDebris_86", "Adding debris PS to PassageInteractDebris_87", "Adding debris PS to PassageInteractDebris_88", "Adding debris PS to PassageInteractDebris_89", "Adding debris PS to PassageInteractDebris_90", "Adding debris PS to PassageInteractDebris_91", "Adding debris PS to PassageInteractDebris_92", "Adding debris PS to PassageInteractDebris_93", "Adding debris PS to PassageInteractDebris_94", "Adding debris PS to PassageInteractDebris_95", "Adding debris PS to PassageInteractDebris_96", "Adding debris PS to PassageInteractDebris_97", "Adding debris PS to PassageInteractDebris_98", "Adding debris PS to PassageInteractDebris_99", "cave_in_cave_in_*", "hanging_lantern_ceiling_7", "fogarea", "ps_area_fog_large", "AreaCaveIn", "fogarea2", "AreaCaveIn2", "breakcave", "ps_break_mansionbase_wall", "rumble", "1", "TimerCaveIn", "3", "4", "AreaCaveInQuest", "AreaQuestDone", "AreaLookGrunt_1", "AreaGruntSound03", "mansion_2", "Interactmansion_2", "PointLight_4", "PointLight_2", "locked_door", "Ch02Level16", "InteractLargeDoor", "TimerGruntDoorBackon", "cavein", "ps_break_cavein_local", "caveins", "explosion_rock_large", "cave_in_cave_in_1", "react_scare6", "11_event_tree", "03cavein", "03CaveIn", "AreaGrunt_3", "AreaActivateLiveGrunt_1", "AreaActivateLiveGrunt_2", "servant_grunt_1", "servant_grunt_2", "scare", "TimerGruntScare", "breath", "AreaLookGrunt", "servant_grunt_3", "grunt_idle", "AreaGruntSoundBehind", "Archives", "AreaSecretBook_0", "AreaSecretBook_1", "AreaSecretBook_2", "AreaSecretBook_3", "AreaSecretBook_4", "AreaSecretBook_5", "AreaSecretBook_6", "AreaSecretBook_7", "AreaSecretBook_8", "AreaSecretBook_9", "AreaSecretBook_10", "AreaSecretBook_11", "AreaSecretBook_12", "AreaSecretBook_13", "AreaSecretBook_14", "AreaSecretBook_15", "AreaSecretBook_16", "AreaSecretBook_17", "AreaSecretBook_18", "AreaSecretBook_19", "AreaSecretBook_20", "AreaSecretBook_21", "AreaSecretBook_22", "AreaSecretBook_23", "AreaSecretBook_24", "AreaSecretBook_25", "AreaSecretBook_26", "AreaSecretBook_27", "AreaSecretBook_28", "AreaSecretBook_29", "AreaSecretBook_30", "AreaSecretBook_31", "AreaSecretBook_32", "AreaSecretBook_33", "AreaSecretBook_34", "AreaSecretBook_35", "AreaSecretBook_36", "AreaSecretBook_37", "AreaSecretBook_38", "AreaSecretBook_39", "AreaSecretBook_40", "AreaSecretBook_41", "AreaSecretBook_42", "AreaSecretBook_43", "AreaSecretBook_44", "AreaSecretBook_45", "AreaSecretBook_46", "AreaSecretBook_47", "AreaSecretBook_48", "AreaSecretBook_49", "AreaSecretBook_50", "AreaSecretBook_51", "AreaSecretBook_52", "AreaSecretBook_53", "AreaSecretBook_54", "AreaSecretBook_55", "AreaSecretBook_56", "AreaSecretBook_57", "AreaSecretBook_58", "AreaSecretBook_59", "AreaSecretBook_60", "AreaSecretBook_61", "AreaSecretBook_62", "AreaSecretBook_63", "AreaSecretBook_64", "AreaSecretBook_65", "AreaSecretBook_66", "AreaSecretBook_67", "AreaSecretBook_68", "AreaSecretBook_69", "AreaSecretBook_70", "AreaSecretBook_71", "AreaSecretBook_72", "AreaSecretBook_73", "AreaSecretBook_74", "AreaSecretBook_75", "AreaSecretBook_76", "AreaSecretBook_77", "AreaSecretBook_78", "AreaSecretBook_79", "AreaSecretBook_80", "AreaSecretBook_81", "AreaSecretBook_82", "AreaSecretBook_83", "AreaSecretBook_84", "AreaSecretBook_85", "AreaSecretBook_86", "AreaSecretBook_87", "AreaSecretBook_88", "AreaSecretBook_89", "AreaSecretBook_90", "AreaSecretBook_91", "AreaSecretBook_92", "AreaSecretBook_93", "AreaSecretBook_94", "AreaSecretBook_95", "AreaSecretBook_96", "AreaSecretBook_97", "AreaSecretBook_98", "AreaSecretBook_99", "CollideSecretBook", "AreaTriggerManyBook", "CollideBeginManyBook", "AreaTriggerPiano", "CollideBeginPiano", "AreaClosePiano", "CollideCloseLid", "vase02_ghost_1", "AreaJar", "CollideJarLeaveArea", "CollideOrb", "CollideHint", "rock_small_0", "rock_small_1", "rock_small_2", "rock_small_3", "rock_small_4", "rock_small_5", "rock_small_6", "rock_small_7", "rock_small_8", "rock_small_9", "rock_small_10", "rock_small_11", "rock_small_12", "rock_small_13", "rock_small_14", "rock_small_15", "rock_small_16", "rock_small_17", "rock_small_18", "rock_small_19", "rock_small_20", "rock_small_21", "rock_small_22", "rock_small_23", "rock_small_24", "rock_small_25", "rock_small_26", "rock_small_27", "rock_small_28", "rock_small_29", "rock_small_30", "rock_small_31", "rock_small_32", "rock_small_33", "rock_small_34", "rock_small_35", "rock_small_36", "rock_small_37", "rock_small_38", "rock_small_39", "rock_small_40", "rock_small_41", "rock_small_42", "rock_small_43", "rock_small_44", "rock_small_45", "rock_small_46", "rock_small_47", "rock_small_48", "rock_small_49", "rock_small_50", "rock_small_51", "rock_small_52", "rock_small_53", "rock_small_54", "rock_small_55", "rock_small_56", "rock_small_57", "rock_small_58", "rock_small_59", "rock_small_60", "rock_small_61", "rock_small_62", "rock_small_63", "rock_small_64", "rock_small_65", "rock_small_66", "rock_small_67", "rock_small_68", "rock_small_69", "rock_small_70", "rock_small_71", "rock_small_72", "rock_small_73", "rock_small_74", "rock_small_75", "rock_small_76", "rock_small_77", "rock_small_78", "rock_small_79", "rock_small_80", "rock_small_81", "rock_small_82", "rock_small_83", "rock_small_84", "rock_small_85", "rock_small_86", "rock_small_87", "rock_small_88", "rock_small_89", "rock_small_90", "rock_small_91", "rock_small_92", "rock_small_93", "rock_small_94", "rock_small_95", "rock_small_96", "rock_small_97", "rock_small_98", "rock_small_99", "chair_nice01_0", "chair_nice01_1", "chair_nice01_2", "chair_nice01_3", "chair_nice01_4", "chair_nice01_5", "chair_nice01_6", "chair_nice01_7", "chair_nice01_8", "chair_nice01_9", "chair_nice01_10", "chair_nice01_11", "chair_nice01_13", "chair_nice01_14", "chair_nice01_15", "chair_nice01_16", "chair_nice01_17", "chair_nice01_18", "chair_nice01_19", "chair_nice01_20", "chair_nice01_21", "chair_nice01_22", "chair_nice01_23", "chair_nice01_24", "chair_nice01_25", "chair_nice01_26", "chair_nice01_27", "chair_nice01_28", "chair_nice01_29", "chair_nice01_30", "chair_nice01_31", "chair_nice01_32", "chair_nice01_33", "chair_nice01_34", "chair_nice01_35", "chair_nice01_36", "chair_nice01_37", "chair_nice01_38", "chair_nice01_39", "chair_nice01_40", "chair_nice01_41", "chair_nice01_42", "chair_nice01_43", "chair_nice01_44", "chair_nice01_45", "chair_nice01_46", "chair_nice01_47", "chair_nice01_48", "chair_nice01_49", "chair_nice01_50", "chair_nice01_51", "chair_nice01_52", "chair_nice01_53", "chair_nice01_54", "chair_nice01_55", "chair_nice01_56", "chair_nice01_57", "chair_nice01_58", "chair_nice01_59", "chair_nice01_60", "chair_nice01_61", "chair_nice01_62", "chair_nice01_63", "chair_nice01_64", "chair_nice01_65", "chair_nice01_66", "chair_nice01_67", "chair_nice01_68", "chair_nice01_69", "chair_nice01_70", "chair_nice01_71", "chair_nice01_72", "chair_nice01_73", "chair_nice01_74", "chair_nice01_75", "chair_nice01_76", "chair_nice01_77", "chair_nice01_78", "chair_nice01_79", "chair_nice01_80", "chair_nice01_81", "chair_nice01_82", "chair_nice01_83", "chair_nice01_84", "chair_nice01_85", "chair_nice01_86", "chair_nice01_87", "chair_nice01_88", "chair_nice01_89", "chair_nice01_90", "chair_nice01_91", "chair_nice01_92", "chair_nice01_93", "chair_nice01_94", "chair_nice01_95", "chair_nice01_96", "chair_nice01_97", "chair_nice01_98", "chair_nice01_99", "CollideCaveIn", "CollideCaveInQuest", "CollideQuestDone", "CollideActivateLiveGrunt", "AreaGruntSound01", "CollideGruntSound01", "AreaGruntSound02", "CollideGruntSound02", "CollideGruntSound03", "AreaActivateLiveGrunt_3", "CollideDisableLiveGrunt", "CollideJumpGrunt", "PlayerInteractSecretPassage", "PathNodeArea_5", "PathNodeArea_10", "PathNodeArea_14", "AreaCloudActivate_0", "AreaCloudActivate_1", "AreaCloudActivate_6", "AreaCloudActivate_7", "AreaCloudActivate_8", "AreaCloudActivate_9", "AreaCloudActivate_10", "AreaCloudActivate_11", "AreaCloudActivate_12", "AreaCloudActivate_13", "AreaCloudActivate_14", "AreaCloudActivate_15", "AreaCloudActivate_16", "AreaCloudActivate_17", "AreaCloudActivate_18", "AreaCloudActivate_19", "AreaCloudActivate_20", "AreaCloudActivate_21", "AreaCloudActivate_22", "AreaCloudActivate_23", "AreaCloudActivate_24", "AreaCloudActivate_25", "AreaCloudActivate_26", "AreaCloudActivate_27", "AreaCloudActivate_28", "AreaCloudActivate_29", "AreaCloudActivate_30", "AreaCloudActivate_31", "AreaCloudActivate_32", "AreaCloudActivate_33", "AreaCloudActivate_34", "AreaCloudActivate_35", "AreaCloudActivate_36", "AreaCloudActivate_37", "AreaCloudActivate_38", "AreaCloudActivate_39", "AreaCloudActivate_40", "AreaCloudActivate_41", "AreaCloudActivate_42", "AreaCloudActivate_43", "AreaCloudActivate_44", "AreaCloudActivate_45", "AreaCloudActivate_46", "AreaCloudActivate_47", "AreaCloudActivate_48", "AreaCloudActivate_49", "AreaCloudActivate_50", "AreaCloudActivate_51", "AreaCloudActivate_52", "AreaCloudActivate_53", "AreaCloudActivate_54", "AreaCloudActivate_55", "AreaCloudActivate_56", "AreaCloudActivate_57", "AreaCloudActivate_58", "AreaCloudActivate_59", "AreaCloudActivate_60", "AreaCloudActivate_61", "AreaCloudActivate_62", "AreaCloudActivate_63", "AreaCloudActivate_64", "AreaCloudActivate_65", "AreaCloudActivate_66", "AreaCloudActivate_67", "AreaCloudActivate_68", "AreaCloudActivate_69", "AreaCloudActivate_70", "AreaCloudActivate_71", "AreaCloudActivate_72", "AreaCloudActivate_73", "AreaCloudActivate_74", "AreaCloudActivate_75", "AreaCloudActivate_76", "AreaCloudActivate_77", "AreaCloudActivate_78", "AreaCloudActivate_79", "AreaCloudActivate_80", "AreaCloudActivate_81", "AreaCloudActivate_82", "AreaCloudActivate_83", "AreaCloudActivate_84", "AreaCloudActivate_85", "AreaCloudActivate_86", "AreaCloudActivate_87", "AreaCloudActivate_88", "AreaCloudActivate_89", "AreaCloudActivate_90", "AreaCloudActivate_91", "AreaCloudActivate_92", "AreaCloudActivate_93", "AreaCloudActivate_94", "AreaCloudActivate_95", "AreaCloudActivate_96", "AreaCloudActivate_97", "AreaCloudActivate_98", "AreaCloudActivate_99", "CollideCloudThing", "AreaCloudThing_1", "note_paper01_2", "PickVillageNote", "display_3", "secret_painting", "lever_small01", "painting_large03_1", "enemyS", "TimerIntroEnemy", "PlayerBeenInLevel03", "lantern", "tinderbox_0", "tinderbox_1", "tinderbox_2", "tinderbox_3", "tinderbox_4", "tinderbox_5", "tinderbox_6", "tinderbox_7", "tinderbox_8", "tinderbox_9", "tinderbox_10", "tinderbox_11", "tinderbox_12", "tinderbox_13", "tinderbox_14", "tinderbox_15", "tinderbox_16", "tinderbox_17", "tinderbox_18", "tinderbox_19", "tinderbox_20", "tinderbox_21", "tinderbox_22", "tinderbox_23", "tinderbox_24", "tinderbox_25", "tinderbox_26", "tinderbox_27", "tinderbox_28", "tinderbox_29", "tinderbox_30", "tinderbox_31", "tinderbox_32", "tinderbox_33", "tinderbox_34", "tinderbox_35", "tinderbox_36", "tinderbox_37", "tinderbox_38", "tinderbox_39", "tinderbox_40", "tinderbox_41", "tinderbox_42", "tinderbox_43", "tinderbox_44", "tinderbox_45", "tinderbox_46", "tinderbox_47", "tinderbox_48", "tinderbox_49", "tinderbox_50", "tinderbox_51", "tinderbox_52", "tinderbox_53", "tinderbox_54", "tinderbox_55", "tinderbox_56", "tinderbox_57", "tinderbox_58", "tinderbox_59", "tinderbox_60", "tinderbox_61", "tinderbox_62", "tinderbox_63", "tinderbox_64", "tinderbox_65", "tinderbox_66", "tinderbox_67", "tinderbox_68", "tinderbox_69", "tinderbox_70", "tinderbox_71", "tinderbox_72", "tinderbox_73", "tinderbox_74", "tinderbox_75", "tinderbox_76", "tinderbox_77", "tinderbox_78", "tinderbox_79", "tinderbox_80", "tinderbox_81", "tinderbox_82", "tinderbox_83", "tinderbox_84", "tinderbox_85", "tinderbox_86", "tinderbox_87", "tinderbox_88", "tinderbox_89", "tinderbox_90", "tinderbox_91", "tinderbox_92", "tinderbox_93", "tinderbox_94", "tinderbox_95", "tinderbox_96", "tinderbox_97", "tinderbox_98", "tinderbox_99", "amb_soft_mood", "scare_thump_flash", "03_creak", "03_rock_amb", "03_loop", "03_orb_loop", "03_orb_loop_loud", "player_bodyfall", "03_orb", "03_amb", "03_insects", "03_amb_library", "react_breath_slow", "03_pound", "react_pant", "player_climb", "03_drag_dirt", "03_lift_rock", "03_crew_talk", "03_waking_up", "03_rock_move", "react_sigh", "scare_male_terrified", "general_piano02", "general_wind_whirl", "general_piano01", "03_cloud_swarm", "general_piano03", "ps_break_wood", "ps_dust_piano", "ps_dust_impact", "ps_cloud_thing01", "ps_blood_drop", "ps_dust_falling_small", "ps_orb_on_player", "LoadingText", "Ch01_Diary01_0", "Ch01_Diary01_1", "Ch01_Diary01_2", "Ch01_Diary01_3", "Ch01_Diary01_4", "Ch01_Diary01_5", "Ch01_Diary01_6", "Ch01_Diary01_7", "Ch01_Diary01_8", "Ch01_Diary01_9", "Ch01_Diary01_10", "Ch01_Diary01_11", "Ch01_Diary01_12", "Ch01_Diary01_13", "Ch01_Diary01_14", "Ch01_Diary01_15", "Ch01_Diary01_16", "Ch01_Diary01_17", "Ch01_Diary01_18", "Ch01_Diary01_19", "Ch01_Diary01_20", "Ch01_Diary01_21", "Ch01_Diary01_22", "Ch01_Diary01_23", "Ch01_Diary01_24", "Ch01_Diary01_25", "Ch01_Diary01_26", "Ch01_Diary01_27", "Ch01_Diary01_28", "Ch01_Diary01_29", "Ch01_Diary01_30", "Ch01_Diary01_31", "Ch01_Diary01_32", "Ch01_Diary01_33", "Ch01_Diary01_34", "Ch01_Diary01_35", "Ch01_Diary01_36", "Ch01_Diary01_37", "Ch01_Diary01_38", "Ch01_Diary01_39", "Ch01_Diary01_40", "Ch01_Diary01_41", "Ch01_Diary01_42", "Ch01_Diary01_43", "Ch01_Diary01_44", "Ch01_Diary01_45", "Ch01_Diary01_46", "Ch01_Diary01_47", "Ch01_Diary01_48", "Ch01_Diary01_49", "Ch01_Diary01_50", "Ch01_Diary01_51", "Ch01_Diary01_52", "Ch01_Diary01_53", "Ch01_Diary01_54", "Ch01_Diary01_55", "Ch01_Diary01_56", "Ch01_Diary01_57", "Ch01_Diary01_58", "Ch01_Diary01_59", "Ch01_Diary01_60", "Ch01_Diary01_61", "Ch01_Diary01_62", "Ch01_Diary01_63", "Ch01_Diary01_64", "Ch01_Diary01_65", "Ch01_Diary01_66", "Ch01_Diary01_67", "Ch01_Diary01_68", "Ch01_Diary01_69", "Ch01_Diary01_70", "Ch01_Diary01_71", "Ch01_Diary01_72", "Ch01_Diary01_73", "Ch01_Diary01_74", "Ch01_Diary01_75", "Ch01_Diary01_76", "Ch01_Diary01_77", "Ch01_Diary01_78", "Ch01_Diary01_79", "Ch01_Diary01_80", "Ch01_Diary01_81", "Ch01_Diary01_82", "Ch01_Diary01_83", "Ch01_Diary01_84", "Ch01_Diary01_85", "Ch01_Diary01_86", "Ch01_Diary01_87", "Ch01_Diary01_88", "Ch01_Diary01_89", "Ch01_Diary01_90", "Ch01_Diary01_91", "Ch01_Diary01_92", "Ch01_Diary01_93", "Ch01_Diary01_94", "Ch01_Diary01_95", "Ch01_Diary01_96", "Ch01_Diary01_97", "Ch01_Diary01_98", "Ch01_Diary01_99");
    FakeDatabase.FindMusic("amb_soft_mood");
    FakeDatabase.FindMusic("11_event_tree");
    FakeDatabase.FindMusic("03_paper_village.ogg");
    FakeDatabase.FindMusic("15_event_prisoner");
    FakeDatabase.FindMusic("03_puzzle_secret.ogg");
    FakeDatabase.FindMusic("03_event_books.ogg");
    FakeDatabase.FindMusic("03_event_tomb.ogg");
    FakeDatabase.FindMusic("03_paper_daniel02.ogg");
    FakeDatabase.FindMusic("03_paper_daniel01.ogg");	
	//----PRELOADING----//
	PreloadSound("scare_thump_flash"); PreloadSound("03_creak"); PreloadSound("react_breath"); PreloadSound("03_rock_amb"); 
	PreloadSound("03_loop"); PreloadSound("03_orb_loop"); PreloadSound("03_orb_loop_loud"); PreloadSound("player_bodyfall"); 
	PreloadSound("03_orb"); PreloadSound("03_amb"); PreloadSound("03_insects"); PreloadSound("03_amb_library"); 
	PreloadSound("react_breath_slow"); PreloadSound("03_pound"); PreloadSound("react_pant"); PreloadSound("player_climb"); 
	PreloadSound("03_drag_dirt"); PreloadSound("03_lift_rock"); PreloadSound("03_crew_talk"); PreloadSound("03_waking_up"); 
	PreloadSound("03_rock_move"); PreloadSound("react_sigh"); PreloadSound("scare_scratch"); PreloadSound("break_wood"); 
	PreloadSound("scare_male_terrified"); PreloadSound("scare_thump_flash"); PreloadSound("general_piano02"); PreloadSound("general_wind_whirl"); 
    PreloadSound("scare_wall_scratch_single"); PreloadSound("general_piano01"); PreloadSound("gameplay_tick"); PreloadSound("lock_door"); 
	PreloadSound("03_cloud_swarm"); PreloadSound("03_in_a_bottle"); PreloadSound("general_piano03"); PreloadSound("general_blood_drop"); 
                                      
	PreloadParticleSystem("ps_break_wood"); PreloadParticleSystem("ps_dust_piano"); PreloadParticleSystem("ps_dust_impact");
	PreloadParticleSystem("ps_cloud_thing01"); PreloadParticleSystem("ps_blood_drop"); PreloadParticleSystem("ps_dust_falling_small");
	PreloadParticleSystem("ps_orb_on_player"); 
	      
	     
	//----AUDIO----//
	StopMusic(4.0f, 0);
	//PlayMusic("amb_soft_mood", true, 1, 0.1f, 0, true);
	AutoSave();
}

////////////////////////////
// Run when leaving map
public override void OnLeave()
{
	//////////////////////
	//Load Screen Setup
	SetupLoadScreen("LoadingText", "Ch01_Diary01_", 6, "game_loading_catacombs.jpg");
}
}
