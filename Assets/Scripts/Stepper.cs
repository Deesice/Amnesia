using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Stepper : MonoBehaviour
{
    public SoundLibrary[] dirtSteps;
    public SoundLibrary[] fabricSteps;
    public SoundLibrary[] hardWaterSteps;
    public SoundLibrary[] metalSteps;
    public SoundLibrary[] organicSteps;
    public SoundLibrary[] rockSteps;
    public SoundLibrary[] rockReverbSteps;
    public SoundLibrary[] waterSteps;
    public SoundLibrary[] woodSteps;    
    public SoundLibrary[] woodSqueakySteps;

    CameraAnim cameraAnim;
    [SerializeField] Matter floorMatter;
    
    void Start()
    {
        cameraAnim = GetComponent<CameraAnim>();
        cameraAnim.OnStepped += () =>
        {
            Scan();
            SoundManager.PlaySound(SelectLibrary());
        };
    }
    SoundLibrary SelectLibrary()
    {
        if (floorMatter == null)
            return null;
        SoundLibrary[] lib;
        switch (floorMatter.material)
        {
            case Matter.Material.Default:
                lib = rockSteps;
                break;
            case Matter.Material.Dirt:
                lib = dirtSteps;
                break;
            case Matter.Material.Dust:
                lib = rockSteps;
                break;
            case Matter.Material.Dyn_Book:
                lib = rockSteps;
                break;
            case Matter.Material.Dyn_Paper:
                lib = rockSteps;
                break;
            case Matter.Material.Generic_Hard:
                lib = rockSteps;
                break;
            case Matter.Material.Generic_Soft:
                lib = fabricSteps;
                break;
            case Matter.Material.Glass:
                lib = rockSteps;
                break;
            case Matter.Material.Metal:
                lib = metalSteps;
                break;
            case Matter.Material.Metal_Chain:
                lib = metalSteps;
                break;
            case Matter.Material.Metal_Roll:
                lib = metalSteps;
                break;
            case Matter.Material.Organic:
                lib = organicSteps;
                break;
            case Matter.Material.Rock:
                lib = rockSteps;
                break;
            case Matter.Material.Rock_Reverb:
                lib = rockReverbSteps;
                break;
            case Matter.Material.Rock_Roll:
                lib = rockSteps;
                break;
            case Matter.Material.Rock_Water:
                lib = hardWaterSteps;
                break;
            case Matter.Material.Silent:
                return null;
            case Matter.Material.Water:
                lib = waterSteps;
                break;
            case Matter.Material.Wood:
                lib = woodSteps;
                break;
            case Matter.Material.Wood_Heavy:
                lib = woodSteps;
                break;
            case Matter.Material.Wood_Rool:
                lib = woodSteps;
                break;
            case Matter.Material.Wood_Squeaky:
                lib = woodSqueakySteps;
                break;
            default:
                return null;
        }
        if (cameraAnim.IsRun)
            return lib[2];
        else
            return lib[1];
    }
    void Scan()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.down * 1.5f, Vector3.down, out hit, 0.5f))
        {
            floorMatter = hit.collider.gameObject.GetComponent<Matter>();
            if (floorMatter == null)
                floorMatter = hit.collider.gameObject.GetComponentInParent<Matter>();
        }
    }
}
