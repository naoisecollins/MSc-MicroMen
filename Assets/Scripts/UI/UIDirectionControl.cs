using UnityEngine;

public class UIDirectionControl : MonoBehaviour // this script is to keep the health rotation static
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;//finds rotation of the canvas
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;// every update sets its current rotation to the local rotation
    }
}
