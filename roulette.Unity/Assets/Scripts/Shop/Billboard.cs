using UnityEngine;
using Zenject;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    [Inject]
    public void Constuct(Camera camera)
    {
        cam = camera;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
