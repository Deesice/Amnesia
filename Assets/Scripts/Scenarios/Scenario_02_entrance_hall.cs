using UnityEngine;
using System.Collections;

public class Scenario_02_entrance_hall : Scenario {
private void Start() {}
////////////////////
//BEGIN LARGE ROOM//
/*Bird wings when entering room first time
 */
public void CollideEnterLargeRoom(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("BirdSound", "general_birds_flee", "BirdSound", 0, false);
	
	AddTimer("whirl", 7, "TimerDustWhirl");

	/*DEBUG
	 */	
	AddDebugMessage("Entered big room, birds flee!", true);
}
/*Random whirls of dust
 */
public void TimerDustWhirl(string  asTimer)
{
	int iWhirl= RandFloat(1, 4);
	float fWhirl= RandFloat(0.5f,15.5f);
	
	CreateParticleSystemAtEntity("whirl"+iWhirl, "ps_dust_whirl.ps", "Whirl0"+iWhirl, false);
	
	PlaySoundAtEntity("whirl"+iWhirl, "general_wind_whirl.snt", "Whirl0"+iWhirl, 0, false);
	
	AddTimer("whirl", 14.5f+fWhirl, "TimerDustWhirl");
	
	/*DEBUG
	 */
	AddDebugMessage("Where Whirl Now: "+iWhirl+" Next Whirl In: "+(14.5f+fWhirl), true);
}
//END LARGE ROOM//
//////////////////


//////////////////////
//BEGIN BLOCKED DOOR//
/*Break wood planks to unlock door
 */
public void FuncUnlockDoor(string  asEntity, string  asType)
{
	SetSwingDoorLocked("castle_arched01_1", false, true);
	
	/*DEBUG
	 */
	AddDebugMessage("UnlockDoor!", true);
}
//END BLOCKED DOOR//
////////////////////


/////////////////////////
//BEGIN GUARDIAN EVENTS//
string[] aSlime;	//Holds the entities to SetPropActiveAndFade during the guardian events
/*Guardian setup and effects specific for each event
 */
public void TimerGuardSlime(string  asTimer)
{
	if(asTimer == "fade1"){
		//FadePlayerFOVMulTo(1.5f, 0.01f);
		
		/* if(GetLanternActive()){
			SetLocalVarInt("LanternActive", 1);
			SetLanternActive(false, true);
			SetLanternDisabled(true);
		} else SetLanternDisabled(true); */
		
		PlayerReactions(1.0f);
		SetPlayerMoveSpeedMul(0.8f);
		SetPlayerRunSpeedMul(0.5f);
		return;
	} 
	else if(asTimer == "fade2"){
		FadeImageTrailTo(0,2);
		//FadePlayerFOVMulTo(1, 1.0f);
		SetPlayerMoveSpeedMul(1.0f);
		SetPlayerRunSpeedMul(1.0f);
		AddTimer("lantern", 1.0f, "TimerGuardSlime");
		return;
	}
	else if(asTimer == "lantern"){
		/* if(GetLocalVarInt("LanternActive") == 1){
			SetLocalVarInt("LanternActive", 0);
			SetLanternDisabled(false);
			SetLanternActive(true, true);
		} else SetLanternDisabled(false);	 */
		return;
	}
	
	if(asTimer == "guard1"){
		GuardianEffects(0.02f,4.5f,1.0f,0.8f,1);

		FadeLightTo("BoxLight_2", 0.15f, 0.07f, 0.1f, 0.0f, -1, 5.0f);
		
		PlaySoundAtEntity("slimeloop1", "slime_loop", "slime_pile_3", 3, true);
		PlaySoundAtEntity("slimeloop2", "slime_loop", "slime_anim_ceiling_1", 4, true);
		PlaySoundAtEntity("slimeloop3", "slime_loop", "slime_pile_large_2", 6, true);
		PlaySoundAtEntity("amb_guard", "amb_guardian", "chandelier_simple_4", 5, true);
		
		SetEntityActive("SlimeDamageArea_1", true);
		AddTimer("SlimeDamageArea_2", 4, "TimerActivateSlimeEntity");		

		string[] aSlime = { "slime_pile_1","slime_pile_2","slime_pile_3","slime_pile_4","slime_pile_5",
							"slime_egg_1","slime_egg_2","slime_egg_3","slime_egg_4","slime_egg_5",
							"slime_pile_large_1","slime_pile_large_2","slime_pile_large_3","slime_pile_large_4","slime_pile_large_5",
							"slime_6way_1","slime_6way_2","slime_6way_3","slime_6way_4",
							"slime_3way_2","slime_3way_3","slime_3way_4","slime_3way_5",
							"slime_anim_wall_1","slime_anim_ceiling_1" };			
		this.aSlime = aSlime;
	} 
	else if(asTimer == "guard2"){
		GuardianEffects(0.035f,3.0f,1.0f,1.1f,2);
	
		FadeLightTo("BoxLight_2", 0.15f, 0.03f, 0.06f, 0.0f, -1, 5.0f);
		
		PlaySoundAtEntity("slimeloop4", "slime_loop", "slime_egg2_3", 3, true);
		PlaySoundAtEntity("slimeloop5", "slime_loop", "slime_pile_large2_4", 4, true);
		PlaySoundAtEntity("slimeloop6", "slime_loop", "slime_pile_large2_1", 6, true);
		
		SetEntityActive("SlimeDamageArea_3", true);
		AddTimer("SlimeDamageArea_5", 4, "TimerActivateSlimeEntity");
		
		string[] aSlime = { "slime_pile_large2_1","slime_pile_large2_2","slime_pile_large2_3","slime_pile_large2_4","slime_pile_large2_5",
							"slime_pile2_1","slime_pile2_2","slime_pile2_3","slime_pile2_4","slime_pile2_5",
							"slime_egg2_1","slime_egg2_2","slime_egg2_3","slime_egg2_4","slime_egg2_5",
							"slime_3way2_1","slime_3way2_2","slime_3way2_3","slime_3way2_4","slime_3way2_5",
							"slime_anim_ceiling2_1","slime_anim_ceiling2_2","slime_anim_ceiling2_3","slime_anim_wall2_1","slime_anim_wall2_2",
							"slime_6way2_1","slime_6way2_2","slime_6way2_3","slime_6way2_4","slime_6way2_5" };			
		this.aSlime = aSlime;
	} 
	else if(asTimer == "guard3") {
		GuardianEffects(0.05f,3.0f,1.0f,1.4f,3);

		FadeLightTo("BoxLight_2", 0.15f, 0.03f, 0.06f, 0.0f, -1, 5.0f);
		
		PlaySoundAtEntity("slimeloop7", "slime_loop", "slime_pile3_2", 3, true);
		PlaySoundAtEntity("slimeloop8", "slime_loop", "slime_pile_large3_5", 4, true);
		PlaySoundAtEntity("slimeloop9", "slime_loop", "slime_pile_large3_4", 6, true);
		
		SetEntityActive("SlimeDamageArea_4", true);
		AddTimer("SlimeDamageArea_6", 4, "TimerActivateSlimeEntity");
		
		string[] aSlime = { "slime_pile_large3_1","slime_pile_large3_2","slime_pile_large3_3","slime_pile_large3_4","slime_pile_large3_5",
							"slime_pile3_1","slime_pile3_2","slime_pile3_3","slime_pile3_4","slime_pile3_5",
							"slime_pile3_6","slime_pile3_7","slime_pile3_8","slime_pile3_9","slime_pile3_10",
							"slime_anim_wall3_1","slime_anim_wall3_2","slime_anim_ceiling3_1",
							"slime_6way3_1","slime_6way3_2","slime_6way3_3","slime_6way3_4","slime_6way3_5",
							"slime_egg3_1","slime_egg3_2","slime_egg3_3","slime_egg3_4","slime_egg3_5",
							"slime_3way3_1","slime_3way3_2","slime_3way3_3","slime_3way3_4"};
		this.aSlime = aSlime;
	}
	
	SetPlayerMoveSpeedMul(0.6f);
	SetPlayerRunSpeedMul(0.25f);
	
	SetLocalVarFloat("SlimeSound",0.0f);
	
	for(int iSlime=0;iSlime < aSlime.Length; iSlime++) {
		int iRand = RandFloat(5,8);
			
		SetPropActiveAndFade(aSlime[iSlime], true, iRand);
			
		for(int i=0;i<=iRand;i++)
			AddTimer(aSlime[iSlime], RandFloat(1,iRand-1), "TimerSlimeSounds");
	}
}

public void TimerActivateSlimeEntity(string  asTimer)
{
	SetEntityActive(asTimer, true);	
}


/*General re-used guardian effects
 */
public void GuardianEffects(float fSS1, float fSS2, float fSS4, float fIT1, int iSound)
{
	StartScreenShake(fSS1,fSS2, 1.0f,fSS4);
	FadeImageTrailTo(fIT1,2.0f);
	
	for(int i=1;i<=3;i++)
	{
			AddDebugMessage("Fading light PointLightx"+iSound+"_"+i, false);
			FadeLightTo("PointLightx"+iSound+"_"+i, -1, -1, -1, -1, 3, 5);
	}
	
	for(int i=1;i<=3;i++){
			CreateParticleSystemAtEntity("slime"+iSound+"ps"+i, "ps_slime_fog.ps", "AreaSlime"+iSound+"_"+i, true);
			//CreateParticleSystemAtEntity("slimewall"+iSound+"ps"+i, "ps_slime_wall_flat.ps", "AreaSlimeWall"+iSound+"_"+i, true);
	} 	
	
	CreateParticleSystemAtEntity("fogfall"+iSound+"ps", "ps_slime_fog_falling.ps", "AreaFogFall"+iSound, true);
	
	//PlaySoundAtEntity("player_guard"+iSound, "player_react_guardian"+iSound, "Player", 5);
	PlaySoundAtEntity("guard"+iSound, "guardian_distant"+iSound, "Player", 0, false);
	
	AddTimer("fade1", 0.3f, "TimerGuardSlime");
	AddTimer("fade2", 7.0f-(iSound/2), "TimerGuardSlime");
	
	GiveSanityDamage(10.0f, false);
	
	/*DEBUG
	 */
	AddDebugMessage("Guardian Effect with shake amount: "+fSS1+" With Sound: "+iSound, true);
}
/*Timer loop to play slime sounds during their SetPropActiveAndFade
 */
public void TimerSlimeSounds(string  asTimer)
{
	float fPlaySound = RandFloat(0.9f,1.5f);
	
	if(fPlaySound < 1)
		PlaySoundAtEntity(asTimer, "slime_create.snt", asTimer, 0.0f+GetLocalVarFloat("SlimeSound"), false);
		
	AddLocalVarFloat("SlimeSound",0.002f);
}
/*Player react to the guaridan effect
 */
public void PlayerReactions(float fTimer)
{
	PlaySoundAtEntity("aah", "react_scare.snt", "Player", 0.3f, false);
	
	AddTimer("react1", fTimer*0.5f, "ReactionTimer");
	AddTimer("react2", fTimer*2.5f, "ReactionTimer");
	AddTimer("react3", fTimer*4.5f, "ReactionTimer");
	AddTimer("react4", fTimer*7.0f, "ReactionTimer");
}
public void ReactionTimer(string  asTimer)
{
	if(asTimer == "react1") PlaySoundAtEntity("s"+asTimer, "react_breath.snt", "Player", 0.0f, false);
	else if(asTimer == "react2") PlaySoundAtEntity("s"+asTimer, "react_breath.snt", "Player", 0.1f, false);
	else if(asTimer == "react3") PlaySoundAtEntity("s"+asTimer, "react_breath.snt", "Player", 0.3f, false);
	else PlaySoundAtEntity("s"+asTimer, "react_breath.snt", "Player", 0.6f, false);
}
//END GUARDIAN EVENTS//
///////////////////////


/////////////////////////////////
//BEGIN LOCKED WINE CELLAR DOOR//
public void PlayerInteractDoor(string  asEntity)
{
	if(HasItem("key_study_1")) SetMessage("Ch01Level02", "InteractDoorHaveKey", 0);
	else AddQuest("02LockedDoor", "02LockedDoor");
}
public void UseKeyOnDoor(string  asItem, string  asEntity)
{
	PlaySoundAtEntity("unlocked", "unlock_door", asEntity, 0.0f, false);
	
	GiveSanityBoostSmall();
	
	SetLevelDoorLocked(asEntity, false);
	RemoveItem(asItem);
	
	CompleteQuest("02LockedDoor", "02LockedDoor");
}
//END LOCKED WINE CELLAR DOOR//
///////////////////////////////


////////////////////////
//BEGIN BLOCKING SLIME//
public void CollideGiveQuestsWeb(string  asParent, string  asChild, int alState)
{
	AddQuest("02Web", "02Web");
	SetEntityActive(asChild, false);
}
public void UseAcidOnWeb(string  asItem, string  asEntity)
{
	SetPropHealth(asEntity, 0);
	RemoveItem(asItem);
	GiveItemFromFile("empty_container", "chemical_container.ent");
	
	CompleteQuest("02Web", "02Web");
	
	GiveSanityBoost();
	
	AddTimer("music", 1, "TimerMusicDelay");
	
	FadeLightTo("PointLightAcid", -1, -1, -1, -1, 3, 1.5f);
	AddTimer("PointLightAcid", 4, "TimerFadeAcidLight");
}
public void TimerMusicDelay(string  asTimer)
{
	PlayMusic("02_puzzle", false, 1, 0.1f, 10, false);
}
public void TimerFadeAcidLight(string  asTimer)
{
	FadeLightTo(asTimer, 0, 0, 0, 0, -1, 3);
}

public void UseEmptyContainerOnWeb(string  asItem, string  asEntity)
{
	SetMessage("Ch01Level02", "UseContainerOnSlime", 0);
}

public void UseChemicalOnWeb(string  asItem, string  asEntity)
{
	SetMessage("Ch01Level02", "UseChemicalOnSlime", 0);
}

//END BLOCKING SLIME//
//////////////////////


///////////////////////////
//BEGIN FLASHBACK & TRAIL//
/*Begin timer to show trail
 */
public void CollideAreaFlashBack(string  asParent, string  asChild, int alState)
{
	AddTimer("trail", 2.0f, "TimerCreateTrail");
	StartPlayerLookAt("AreaTrail_1", 3, 3, "");
	CreateParticleSystemAtEntity("rosesonmybed", "ps_rose_petals_wind.ps", "AreaTrail_1", false);
	PlayGuiSound("general_wind_whirl6.ogg", 0.3f);
}
public void TimerCreateTrail(string  asTimer)
{
	AddLocalVarInt("Trail", 1);
	
	int iTrail = GetLocalVarInt("Trail");
	
	switch(GetLocalVarInt("Trail")) {
		case 1:
			//AddTimer("trail", 2.0f, "TimerCreateTrail");
			StopPlayerLookAt();
			MakeTrailEffect();
		break;
		case 2:
			StartPlayerLookAt("AreaTrailDoor", 1, 1, "");
			MakeTrailEffect();
		break;
		case 3:
			StopPlayerLookAt();
			MakeTrailEffect();
		break;
		default:
			MakeTrailEffect();
		break;
	}
	
	if(iTrail < 8) AddTimer("trail", 1.5f, "TimerCreateTrail");
}
bool bSwing = true;
public void MakeTrailEffect()
{
	if(GetLocalVarInt("Trail") == 8){
		FadeLightTo("PointLight_2", 0.0f, 0.0f, 0.0f, 0.0f, -1, 2.0f);
		return;
	}
	
	bSwing = bSwing == false ? true : false;
	if(bSwing) FadeLightTo("PointLight_2", 0.6f, 0.3f, 0.6f, 0.5f, -1, 1.4f);
	else FadeLightTo("PointLight_2", 0.3f, 0.1f, 0.3f, 0.25f, -1, 1.4f);
		
	AddDebugMessage("hello"+GetLocalVarInt("Trail"), false);
	//CreateParticleSystemAtEntity("trail"+GetLocalVarInt("Trail"), "ps_trail_flow.ps", "AreaTrail_"+GetLocalVarInt("Trail"), false);
}
//END FLASHBACK & TRAIL//
/////////////////////////

public void InteractSlime(string  asEntity)
{
	PlayGuiSound("slime_create3.ogg", 0.4f);
	SetMessage("Ch01Level02", "InteractSlime", 0);
	
	AddTimer("backoncellar", 1.0f, "TimerCellarSlime");
}
public void TimerCellarSlime(string  asTimer)
{
	SetEntityPlayerInteractCallback("web_1", "InteractSlime", true);
}
public void InteractLockedDoor(string  asEntity)
{
	AddTimer("lockeddoor", 0.5f, "TimerLockedDoor4evah");
	
	PlaySoundAtEntity("locked_"+asEntity, "locked_door", asEntity, 0.0f, false);
}
public void TimerLockedDoor4evah(string  asTimer)
{
	SetMessage("LevelDoors", "LockedForever", 0);
}

public void TimerRunHint(string  asTimer)
{
	GiveHint("run", "Hints", "RunHint", 0);
}


/////////////////////////////
//BEGIN INTERACT LARGE GATE//
public void InteractLargeGate(string  asEntity)
{
	PlayGuiSound("close_gate.ogg", 0.5f);
	PlaySoundAtEntity("guardboo", "guardian_distant1", "LargeGate", 2.0f, false);
	FadeLightTo("PointLight_15", 0.2f, 0.0f, 0.05f, 1.0f, -1, 4.0f);
	
	AddTimer("1", 0.5f, "TimerLargeGate");
	AddTimer("2", 1.2f, "TimerLargeGate");
	AddTimer("3", 1.7f, "TimerLargeGate");
	AddTimer("4", 2.0f, "TimerLargeGate");
}
public void TimerLargeGate(string  asTimer)
{
	if(asTimer == "1"){
		PlayMusic("01_event_critters", false, 0.7f, 1.0f, 10, false);
		StartScreenShake(0.005f, 2.0f, 1.0f, 1.0f);
		SetPropActiveAndFade("slime_static_slime_6way_1", true, 2.5f);
		SetPropActiveAndFade("slime_static_slime_6way_2", true, 1.5f);
		PlaySoundAtEntity("slimecreate", "slime_loop", "LargeGate", 4.0f, true);
		PlaySoundAtEntity("slimecreate1", "slime_create", "LargeGate", 0.0f, true);
		GiveSanityDamage(10.0f, true);
	}
	else if(asTimer == "2"){
		PlaySoundAtEntity("slimecreate2", "slime_create", "LargeGate", 0.1f, true);
	}
	else if(asTimer == "3"){
		PlaySoundAtEntity("slimecreate3", "slime_create", "LargeGate", 0.15f, true);
	}
	else if(asTimer == "4"){
		StopMusic(4.0f, 10);
		StopSound("guardboo", 2.0f);
		PlaySoundAtEntity("slimecreate4", "slime_create", "LargeGate", 0.2f, true);
		SetEntityPlayerInteractCallback("LargeGate", "InteractLargeGate02", true);
		PlayMusic("02_amb_strange", true, 1.0f, 2.0f, 0, true);
	}
}
public void InteractLargeGate02(string  asEntity)
{
	PlayGuiSound("slime_create3.ogg", 0.5f);
	
	AddTimer("intslimebackon", 1.0f, "TimerSlimeGateOn");
}
public void TimerSlimeGateOn(string  asTimer)
{
	SetEntityPlayerInteractCallback("LargeGate", "InteractLargeGate02", true);
}
//END INTERACT LARGE GATE//
///////////////////////////


/*Scream from 03 if player has been in Lab first, Scream from 04 if player goes towards 03 first
 */
public void CollideDirectionHint(string  asParent, string  asChild, int alState)
{
	if(asChild == "LabHint"){
		PlaySoundAtEntity("scream", "03_no.snt", "ArchiveHint", 0.0f, false);

		AddTimer("ArchiveHint", 0.2f, "TimerScream");
		AddTimer("scream2", 0.8f, "TimerScream");
		
		RemoveEntityCollideCallback("Player", "ArchiveHint");
		RemoveEntityCollideCallback("Player", "LabHint");
	}
	else{
		if(GetGlobalVarInt("BeenToLab") == 0) return;
		
		PlaySoundAtEntity("scream", "03_no.snt", "LabHint", 0.0f, false);
		
		AddTimer("LabHint", 0.2f, "TimerScream");
		AddTimer("scream2", 0.8f, "TimerScream");
		
		RemoveEntityCollideCallback("Player", "LabHint");
		RemoveEntityCollideCallback("Player", "ArchiveHint");
	}
}
public void TimerScream(string  asTimer)
{
	if(asTimer == "scream2"){
		StopPlayerLookAt();
	}
	else if(asTimer == "stopmusic"){
		StopMusic(3.0f, 10);
	}
	else {
		PlayGuiSound("react_scare", 0.7f);
		StartPlayerLookAt(asTimer, 4.0f, 4.0f, "");
		PlayMusic("11_event_tree", false, 1.0f, 0.5f, 10, false);
		AddTimer("stopmusic", 3.0f, "TimerScream");
	}
}

////////////////////////////
// Run first time starting map
public override void OnStart()
{
	////////////////////
	// Hub start sanity boost
	GiveSanityBoost();
	
	
	//----COLLIDE CALLBACKS----//
	AddEntityCollideCallback("Player", "ScareBirds", "CollideEnterLargeRoom", true, 1);
	AddEntityCollideCallback("Player", "AreaGiveQuestWeb", "CollideGiveQuestsWeb", false, 1);
	AddEntityCollideCallback("Player", "AreaCollideFlash", "CollideAreaFlashBack", true, 1);
	AddEntityCollideCallback("Player", "LabHint", "CollideDirectionHint", false, 1);
	AddEntityCollideCallback("Player", "ArchiveHint", "CollideDirectionHint", false, 1);
	
	//---- INTERACT INIT ----//
	AddUseItemCallback("UseKeyOnDoor", "key_study_1", "level_wood_2", "UseKeyOnDoor", true);
	AddUseItemCallback("UseAcidOnWeb", "chemical_container_2", "web_1", "UseAcidOnWeb", true);
	AddUseItemCallback("UseEmptyContainerOnWeb", "chemical_container_1", "web_1", "UseEmptyContainerOnWeb", false);
	AddUseItemCallback("UseChemicalOnWeb", "Chemical_1", "web_1", "UseChemicalOnWeb", false);
	AddUseItemCallback("UseChemicalOnWeb", "Chemical_2", "web_1", "UseChemicalOnWeb", false);
	AddUseItemCallback("UseChemicalOnWeb", "Chemical_3", "web_1", "UseChemicalOnWeb", false);
	AddUseItemCallback("UseChemicalOnWeb", "Chemical_4", "web_1", "UseChemicalOnWeb", false);

	SetEntityPlayerInteractCallback("castle_1", "PlayerInteractDoor", true);
	SetEntityCustomFocusCrossHair("LargeGate", "LevelDoor");
	
	//---- ENITTY INIT----//
	SetNumberOfQuestsInMap(2);
	
	SetLocalVarInt("ScaryMusic", 0);
	
	UnBlockHint("QuestAdded");
	UnBlockHint("EntityGrab02"); UnBlockHint("EntityPush");
	
	AddTimer("runhint", 3, "TimerRunHint");
	
	//----DEBUG----//
	if(ScriptDebugOn())
	{
		GiveItemFromFile("lantern", "lantern.ent");
		GiveItemFromFile("chemical_container_1", "chemical_container.ent");
		GiveItemFromFile("chemical_container_2", "chemical_container_full.ent");
		GiveItemFromFile("Chemical_1", "flask01_aqua_regia.ent");
		GiveItemFromFile("Chemical_2", "flask01_cuprite.ent");
		GiveItemFromFile("Chemical_3", "flask01_calamine.ent");
		GiveItemFromFile("Chemical_4", "flask01_orpiment.ent");
		
		GiveItemFromFile("key_study_1", "key_study.ent");
		
		//SetGlobalVarString("key_study_1","key_study_1");	//PlayerStartArea_3
		//SetGlobalVarInt("ChemJars",4);	//PlayerStartArea_2
		//SetGlobalVarString("chemical_container_2","chemical_container_2"); //PlayerStartArea_5
	}
	
	ClearSavedMaps();
}


////////////////////////////
// Run when entering map
public override void OnEnter()
{
    Finder.Bufferize("BirdSound", "general_birds_flee", "whirl", "TimerDustWhirl", "Entered big room, birds flee!", "Whirl0", "Where Whirl Now: ", " Next Whirl In: ", "castle_arched01_1", "UnlockDoor!", "fade1", "LanternActive", "fade2", "lantern", "TimerGuardSlime", "guard1", "BoxLight_2", "slimeloop1", "slime_loop", "slime_pile_3", "slimeloop2", "slime_anim_ceiling_1", "slimeloop3", "slime_pile_large_2", "amb_guard", "amb_guardian", "chandelier_simple_4", "SlimeDamageArea_1", "SlimeDamageArea_2", "TimerActivateSlimeEntity", "slime_pile_1", "slime_pile_2", "slime_pile_4", "slime_pile_5", "slime_egg_1", "slime_egg_2", "slime_egg_3", "slime_egg_4", "slime_egg_5", "slime_pile_large_1", "slime_pile_large_3", "slime_pile_large_4", "slime_pile_large_5", "slime_6way_1", "slime_6way_2", "slime_6way_3", "slime_6way_4", "slime_3way_2", "slime_3way_3", "slime_3way_4", "slime_3way_5", "slime_anim_wall_1", "guard2", "slimeloop4", "slime_egg2_3", "slimeloop5", "slime_pile_large2_4", "slimeloop6", "slime_pile_large2_1", "SlimeDamageArea_3", "SlimeDamageArea_5", "slime_pile_large2_2", "slime_pile_large2_3", "slime_pile_large2_5", "slime_pile2_1", "slime_pile2_2", "slime_pile2_3", "slime_pile2_4", "slime_pile2_5", "slime_egg2_1", "slime_egg2_2", "slime_egg2_4", "slime_egg2_5", "slime_3way2_1", "slime_3way2_2", "slime_3way2_3", "slime_3way2_4", "slime_3way2_5", "slime_anim_ceiling2_1", "slime_anim_ceiling2_2", "slime_anim_ceiling2_3", "slime_anim_wall2_1", "slime_anim_wall2_2", "slime_6way2_1", "slime_6way2_2", "slime_6way2_3", "slime_6way2_4", "slime_6way2_5", "guard3", "slimeloop7", "slime_pile3_2", "slimeloop8", "slime_pile_large3_5", "slimeloop9", "slime_pile_large3_4", "SlimeDamageArea_4", "SlimeDamageArea_6", "slime_pile_large3_1", "slime_pile_large3_2", "slime_pile_large3_3", "slime_pile3_1", "slime_pile3_3", "slime_pile3_4", "slime_pile3_5", "slime_pile3_6", "slime_pile3_7", "slime_pile3_8", "slime_pile3_9", "slime_pile3_10", "slime_anim_wall3_1", "slime_anim_wall3_2", "slime_anim_ceiling3_1", "slime_6way3_1", "slime_6way3_2", "slime_6way3_3", "slime_6way3_4", "slime_6way3_5", "slime_egg3_1", "slime_egg3_2", "slime_egg3_3", "slime_egg3_4", "slime_egg3_5", "slime_3way3_1", "slime_3way3_2", "slime_3way3_3", "slime_3way3_4", "SlimeSound", "TimerSlimeSounds", "Fading light PointLightx", "_0", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9", "_10", "_11", "_12", "_13", "_14", "_15", "_16", "_17", "_18", "_19", "_20", "_21", "_22", "_23", "_24", "_25", "_26", "_27", "_28", "_29", "_30", "_31", "_32", "_33", "_34", "_35", "_36", "_37", "_38", "_39", "_40", "_41", "_42", "_43", "_44", "_45", "_46", "_47", "_48", "_49", "_50", "_51", "_52", "_53", "_54", "_55", "_56", "_57", "_58", "_59", "_60", "_61", "_62", "_63", "_64", "_65", "_66", "_67", "_68", "_69", "_70", "_71", "_72", "_73", "_74", "_75", "_76", "_77", "_78", "_79", "_80", "_81", "_82", "_83", "_84", "_85", "_86", "_87", "_88", "_89", "_90", "_91", "_92", "_93", "_94", "_95", "_96", "_97", "_98", "_99", "PointLightx", "slime", "ps", "AreaSlime", "slimewall", "AreaSlimeWall", "fogfall", "AreaFogFall", "player_guard", "player_react_guardian", "Player", "guard", "guardian_distant", "Guardian Effect with shake amount: ", " With Sound: ", "aah", "react1", "ReactionTimer", "react2", "react3", "react4", "s", "key_study_1", "Ch01Level02", "InteractDoorHaveKey", "02LockedDoor", "unlocked", "unlock_door", "02Web", "empty_container", "music", "TimerMusicDelay", "PointLightAcid", "TimerFadeAcidLight", "02_puzzle", "UseContainerOnSlime", "UseChemicalOnSlime", "trail", "TimerCreateTrail", "AreaTrail_1", "", "rosesonmybed", "Trail", "AreaTrailDoor", "PointLight_2", "hello", "AreaTrail_0", "AreaTrail_2", "AreaTrail_3", "AreaTrail_4", "AreaTrail_5", "AreaTrail_6", "AreaTrail_7", "AreaTrail_8", "AreaTrail_9", "AreaTrail_10", "AreaTrail_11", "AreaTrail_12", "AreaTrail_13", "AreaTrail_14", "AreaTrail_15", "AreaTrail_16", "AreaTrail_17", "AreaTrail_18", "AreaTrail_19", "AreaTrail_20", "AreaTrail_21", "AreaTrail_22", "AreaTrail_23", "AreaTrail_24", "AreaTrail_25", "AreaTrail_26", "AreaTrail_27", "AreaTrail_28", "AreaTrail_29", "AreaTrail_30", "AreaTrail_31", "AreaTrail_32", "AreaTrail_33", "AreaTrail_34", "AreaTrail_35", "AreaTrail_36", "AreaTrail_37", "AreaTrail_38", "AreaTrail_39", "AreaTrail_40", "AreaTrail_41", "AreaTrail_42", "AreaTrail_43", "AreaTrail_44", "AreaTrail_45", "AreaTrail_46", "AreaTrail_47", "AreaTrail_48", "AreaTrail_49", "AreaTrail_50", "AreaTrail_51", "AreaTrail_52", "AreaTrail_53", "AreaTrail_54", "AreaTrail_55", "AreaTrail_56", "AreaTrail_57", "AreaTrail_58", "AreaTrail_59", "AreaTrail_60", "AreaTrail_61", "AreaTrail_62", "AreaTrail_63", "AreaTrail_64", "AreaTrail_65", "AreaTrail_66", "AreaTrail_67", "AreaTrail_68", "AreaTrail_69", "AreaTrail_70", "AreaTrail_71", "AreaTrail_72", "AreaTrail_73", "AreaTrail_74", "AreaTrail_75", "AreaTrail_76", "AreaTrail_77", "AreaTrail_78", "AreaTrail_79", "AreaTrail_80", "AreaTrail_81", "AreaTrail_82", "AreaTrail_83", "AreaTrail_84", "AreaTrail_85", "AreaTrail_86", "AreaTrail_87", "AreaTrail_88", "AreaTrail_89", "AreaTrail_90", "AreaTrail_91", "AreaTrail_92", "AreaTrail_93", "AreaTrail_94", "AreaTrail_95", "AreaTrail_96", "AreaTrail_97", "AreaTrail_98", "AreaTrail_99", "InteractSlime", "backoncellar", "TimerCellarSlime", "web_1", "lockeddoor", "TimerLockedDoor4evah", "locked_0", "locked_1", "locked_2", "locked_3", "locked_4", "locked_5", "locked_6", "locked_7", "locked_8", "locked_9", "locked_10", "locked_11", "locked_12", "locked_13", "locked_14", "locked_15", "locked_16", "locked_17", "locked_18", "locked_19", "locked_20", "locked_21", "locked_22", "locked_23", "locked_24", "locked_25", "locked_26", "locked_27", "locked_28", "locked_29", "locked_30", "locked_31", "locked_32", "locked_33", "locked_34", "locked_35", "locked_36", "locked_37", "locked_38", "locked_39", "locked_40", "locked_41", "locked_42", "locked_43", "locked_44", "locked_45", "locked_46", "locked_47", "locked_48", "locked_49", "locked_50", "locked_51", "locked_52", "locked_53", "locked_54", "locked_55", "locked_56", "locked_57", "locked_58", "locked_59", "locked_60", "locked_61", "locked_62", "locked_63", "locked_64", "locked_65", "locked_66", "locked_67", "locked_68", "locked_69", "locked_70", "locked_71", "locked_72", "locked_73", "locked_74", "locked_75", "locked_76", "locked_77", "locked_78", "locked_79", "locked_80", "locked_81", "locked_82", "locked_83", "locked_84", "locked_85", "locked_86", "locked_87", "locked_88", "locked_89", "locked_90", "locked_91", "locked_92", "locked_93", "locked_94", "locked_95", "locked_96", "locked_97", "locked_98", "locked_99", "locked_door", "LevelDoors", "LockedForever", "run", "Hints", "RunHint", "guardboo", "guardian_distant1", "LargeGate", "PointLight_15", "1", "TimerLargeGate", "2", "3", "4", "01_event_critters", "slime_static_slime_6way_1", "slime_static_slime_6way_2", "slimecreate", "slimecreate1", "slime_create", "slimecreate2", "slimecreate3", "slimecreate4", "InteractLargeGate02", "02_amb_strange", "intslimebackon", "TimerSlimeGateOn", "LabHint", "scream", "ArchiveHint", "TimerScream", "scream2", "BeenToLab", "stopmusic", "react_scare", "11_event_tree", "ScareBirds", "CollideEnterLargeRoom", "AreaGiveQuestWeb", "CollideGiveQuestsWeb", "AreaCollideFlash", "CollideAreaFlashBack", "CollideDirectionHint", "UseKeyOnDoor", "level_wood_2", "UseAcidOnWeb", "chemical_container_2", "UseEmptyContainerOnWeb", "chemical_container_1", "UseChemicalOnWeb", "Chemical_1", "Chemical_2", "Chemical_3", "Chemical_4", "castle_1", "PlayerInteractDoor", "LevelDoor", "ScaryMusic", "QuestAdded", "EntityGrab02", "EntityPush", "runhint", "TimerRunHint", "ChemJars", "02_amb_safe", "EntranceHall", "general_wind_whirl", "guardian_distant2", "guardian_distant3", "ps_dust_whirl", "ps_slime_fog", "ps_slime_wall_flat", "ps_slime_fog_falling", "ps_trail_flow", "ps_trail_large");
    FakeDatabase.FindMusic("02_amb_safe");
    FakeDatabase.FindMusic("11_event_tree");
    FakeDatabase.FindMusic("02_amb_strange");
    FakeDatabase.FindMusic("01_event_critters");
    FakeDatabase.FindMusic("02_puzzle");		
	SetMapDisplayNameEntry("EntranceHall");
		
	//----PRELOADING----//
	PreloadSound("react_scare"); PreloadSound("general_birds_flee"); PreloadSound("general_wind_whirl"); PreloadSound("slime_loop"); 
	PreloadSound("amb_guardian"); PreloadSound("guardian_distant1"); PreloadSound("guardian_distant2"); PreloadSound("guardian_distant3"); 
	PreloadSound("slime_create"); 
   
	PreloadParticleSystem("ps_dust_whirl"); PreloadParticleSystem("ps_slime_fog"); //PreloadParticleSystem("ps_slime_wall_flat");
	PreloadParticleSystem("ps_slime_fog_falling"); PreloadParticleSystem("ps_trail_flow"); PreloadParticleSystem("ps_trail_large"); 
	
	     
	//----AUDIO----//
	SetFogProperties(10, 50, 1, false);
	SetFogColor(0.12f, 0.14f, 0.18f, 1);
	SetFogActive(true);
	
	//----GUARDIAN INIT----//
	float fGuardianDelay = 0.5f;
	if(GetGlobalVarString("key_study_1") == "key_study_1")
	{
		AddTimer("guard1", RandFloat(fGuardianDelay+0.25f,fGuardianDelay+0.5f), "TimerGuardSlime");
		SetLocalVarInt("ScaryMusic", 1);
		SetGlobalVarString("key_study_1", "");
	} 
	else if(GetGlobalVarInt("ChemJars") == 4)
	{
		AddTimer("guard2", RandFloat(fGuardianDelay+0.5f,fGuardianDelay+0.75f), "TimerGuardSlime");
		SetLocalVarInt("ScaryMusic", 1);
		SetGlobalVarInt("ChemJars",0);
	} 
	else if(GetGlobalVarString("chemical_container_2") == "chemical_container_2")
	{
		AddTimer("guard3", RandFloat(fGuardianDelay+0.75f,fGuardianDelay+1.0f), "TimerGuardSlime");
		SetLocalVarInt("ScaryMusic", 1);
		SetGlobalVarString("chemical_container_2", "");
	} 
	
	if(GetLocalVarInt("ScaryMusic") == 0) PlayMusic("02_amb_safe", true, 0.5f, 5, 0, true);
	else PlayMusic("02_amb_strange", true, 0.5f, 5, 0, true);
	
	//----PLAYER INIT----//
	SetPlayerRunSpeedMul(1);	//Able to run now!
	
	AutoSave();
}

////////////////////////////
// Run when leaving map
public override void OnLeave()
{	
	StopMusic(5, 0);
}
}
