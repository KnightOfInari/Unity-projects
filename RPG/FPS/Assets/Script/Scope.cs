using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour {

    public Animator animator;

    private bool isScoped = false;
    public GameObject  scopeOverlay;

    public GameObject WeaponCamera;
    public Camera mainCamera;

    public float scopedFOV = 15f;
    private float normalFOV;

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && gameObject.GetComponent<WeaponSwitching>().getIsSniper() == true)
        {
            isScoped = !isScoped;
            animator.SetBool("Scoped", isScoped);

            if (isScoped)
                StartCoroutine(OnScoped());
            else
                OnUnscoped();
        }
    }

    void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        WeaponCamera.SetActive(true);
        mainCamera.fieldOfView = normalFOV;
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(.15f);
        scopeOverlay.SetActive(true);
        WeaponCamera.SetActive(false);
        normalFOV = mainCamera.fieldOfView;
        mainCamera.fieldOfView = scopedFOV;
    }
}
