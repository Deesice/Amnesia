using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matter : MonoBehaviour
{
    public enum Material { Default,
        Dirt,
        Dust,
        Dyn_Book,
        Dyn_Paper,
        Generic_Hard,
        Generic_Soft,
        Glass,
        Metal,
        Metal_Chain,
        Metal_Roll,
        Organic,
        Rock,
        Rock_Reverb,
        Rock_Roll,
        Rock_Water,
        Silent,
        Water,
        Wood,
        Wood_Heavy,
        Wood_Rool,
        Wood_Squeaky}
    public Material material;
    string materialName;
    float minSpeedLow = 2;
    float minSpeedMed = 3.4f;
    float minSpeedHigh = 5.8f;
    public void SetByString(string input)
    {
        switch (input)
        {
            case "Dirt":
                material = Material.Dirt;
                break;
            case "Dust":
                material = Material.Dust;
                break;
            case "Dyn_Book":
                material = Material.Dyn_Book;
                break;
            case "Dyn_Paper":
                material = Material.Dyn_Paper;
                break;
            case "Generic_Hard":
                material = Material.Generic_Hard;
                break;
            case "Generic_Soft":
                material = Material.Generic_Soft;
                break;
            case "Glass":
                material = Material.Glass;
                break;
            case "Metal":
                material = Material.Metal;
                break;
            case "Metal_Chain":
                material = Material.Metal_Chain;
                break;
            case "Metal_Roll":
                material = Material.Metal_Roll;
                break;
            case "Organic":
                material = Material.Organic;
                break;
            case "Rock":
                material = Material.Rock;
                break;
            case "Rock_Reverb":
                material = Material.Rock_Reverb;
                break;
            case "Rock_Roll":
                material = Material.Rock_Roll;
                break;
            case "Rock_Water":
                material = Material.Rock_Water;
                break;
            case "Silent":
                material = Material.Silent;
                break;
            case "Water":
                material = Material.Water;
                break;
            case "Wood":
                material = Material.Wood;
                break;
            case "Wood_Heavy":
                material = Material.Wood_Heavy;
                break;
            case "Wood_Rool":
                material = Material.Wood_Rool;
                break;
            case "Wood_Squeaky":
                material = Material.Wood_Squeaky;
                break;
            case "Default":
                material = Material.Default;
                break;
            default:
                material = Material.Default;
                Debug.Log("Matter named as " + input + " does not exist");
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 || string.IsNullOrEmpty(materialName))
            return;
        float impactSpeed = collision.relativeVelocity.magnitude;
        var pos = collision.GetContact(0).point;
        if (impactSpeed >= minSpeedHigh)
        {
            SoundManager.PlayImpactSound("impact_" + materialName + "_high", pos);
            return;
        }
        if (impactSpeed >= minSpeedMed)
        {
            SoundManager.PlayImpactSound("impact_" + materialName + "_med", pos);
            return;
        }
        if (impactSpeed >= minSpeedLow)
            SoundManager.PlayImpactSound("impact_" + materialName + "_low", pos);        
    }
    private void Start()
    {
        switch (material)
        {
            case Material.Generic_Soft:
                materialName = "generic_soft";
                break;
            case Material.Dirt:
                materialName = "dirt";
                break;
            case Material.Glass:
                materialName = "glass";
                break;
            case Material.Metal:
                materialName = "metal";
                break;
            case Material.Metal_Chain:
                materialName = "metal_chain";
                break;
            case Material.Metal_Roll:
                materialName = "metal";
                break;
            case Material.Organic:
                materialName = "organic";
                break;
            case Material.Rock:
                materialName = "rock";
                break;
            case Material.Rock_Reverb:
                materialName = "rock";
                break;
            case Material.Rock_Water:
                materialName = "rock";
                break;
            case Material.Rock_Roll:
                materialName = "rock";
                break;
            case Material.Silent:
                materialName = "";
                break;
            case Material.Water:
                materialName = "water";
                minSpeedLow = 1.1f;
                break;
            case Material.Wood:
                materialName = "wood";
                break;
            case Material.Wood_Heavy:
                materialName = "wood_heavy";
                break;
            case Material.Wood_Rool:
                materialName = "wood";
                break;
            case Material.Wood_Squeaky:
                materialName = "wood_heavy";
                break;
            case Material.Dyn_Book:
                materialName = "book";
                break;
            case Material.Dyn_Paper:
                materialName = "paper";
                break;
            case Material.Dust:
                minSpeedMed = 5.8f;
                minSpeedLow = 3.4f;
                minSpeedHigh = float.MaxValue;
                materialName = "generic_hard";
                break;
            default:
                materialName = "generic_hard";
                break;
        }
        if (!string.IsNullOrEmpty(materialName))
        {
            FakeDatabase.FindProperty("impact_" + materialName + "_high");
            FakeDatabase.FindProperty("impact_" + materialName + "_med");
            FakeDatabase.FindProperty("impact_" + materialName + "_low");
        }
        SetFriction();
    }
    void SetFriction()
    {
        if (GetComponent<Rigidbody>())
            foreach (var i in GetComponentsInChildren<Collider>())
                SetFriction(i);
        else
        {
            var collider = GetComponent<Collider>();
            if (collider != null)
                SetFriction(collider);
            else
                foreach (var i in GetComponentsInChildren<Collider>())
                    SetFriction(i);
        }
    }
    void SetFriction(Collider collider)
    {
        var mat = new PhysicMaterial();
        switch (material)
        {
            case Material.Default:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Generic_Hard:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Generic_Soft:
                mat.bounciness = 0.3f;
                mat.dynamicFriction = 0.45f;
                mat.staticFriction = 0.45f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Dirt:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.6f;
                mat.staticFriction = 0.6f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Dust:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.6f;
                mat.staticFriction = 0.6f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Glass:
                mat.bounciness = 0.05f;
                mat.dynamicFriction = 0.3f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Metal:
                mat.bounciness = 0.15f;
                mat.dynamicFriction = 0.3f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Metal_Chain:
                mat.bounciness = 0.15f;
                mat.dynamicFriction = 0.3f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Metal_Roll:
                mat.bounciness = 0.15f;
                mat.dynamicFriction = 0.3f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Organic:
                mat.bounciness = 0.3f;
                mat.dynamicFriction = 0.45f;
                mat.staticFriction = 0.45f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Rock:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Rock_Reverb:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Rock_Water:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.3f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Rock_Roll:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Silent:
                mat.bounciness = 0.1f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Water:
                mat.bounciness = 0.5f;
                mat.dynamicFriction = 0.3f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Wood:
                mat.bounciness = 0.2f;
                mat.dynamicFriction = 0.45f;
                mat.staticFriction = 0.45f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Wood_Heavy:
                mat.bounciness = 0.2f;
                mat.dynamicFriction = 0.45f;
                mat.staticFriction = 0.45f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Wood_Rool:
                mat.bounciness = 0.2f;
                mat.dynamicFriction = 0.45f;
                mat.staticFriction = 0.45f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Wood_Squeaky:
                mat.bounciness = 0.2f;
                mat.dynamicFriction = 0.45f;
                mat.staticFriction = 0.45f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Dyn_Book:
                mat.bounciness = 0.3f;
                mat.dynamicFriction = 0.35f;
                mat.staticFriction = 0.35f;
                mat.bounceCombine = PhysicMaterialCombine.Average;
                mat.frictionCombine = PhysicMaterialCombine.Average;
                break;
            case Material.Dyn_Paper:
                mat.bounciness = 0.25f;
                mat.dynamicFriction = 0.25f;
                mat.staticFriction = 0.3f;
                mat.bounceCombine = PhysicMaterialCombine.Minimum;
                mat.frictionCombine = PhysicMaterialCombine.Minimum;
                break;
        }
        collider.material = mat;
    }
}
