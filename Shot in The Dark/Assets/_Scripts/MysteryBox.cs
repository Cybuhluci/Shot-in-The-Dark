using Luci.Interactions;
using System.Collections;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public PointsScript pointsScript;
    public MapBasedGunScript mapBasedGunScript;
    public Transform boxWeaponTransform;
    public AnimationCurve boxLidAnim;
    public Transform boxLidTransform;

    private GameObject currentWeaponModel;
    private Vector3 initialWeaponLocalPos;
    private Vector3 raisedWeaponLocalPos;
    private bool boxClosed = false;
    private MysteryBoxInteract _activeBoxInteract;
    private bool _waitingForPickup = false;

    public void buyBox(int cost, Transform lid, Transform weapon, MysteryBoxInteract boxinteract)
    {
        if (pointsScript.points >= cost)
        {
            pointsScript.points -= cost;
            Debug.Log("Mystery Box purchased! Random weapon granted.");
            boxWeaponTransform = weapon;
            boxLidTransform = lid;
            initialWeaponLocalPos = boxWeaponTransform.localPosition;
            raisedWeaponLocalPos = initialWeaponLocalPos + new Vector3(0, 1f, 0); // Raise by 1 unit (adjust as needed)
            // find a way to disable the interact script while the box is spinning, also find a way to make no interact text show up
            boxinteract.ToggleInteract(false);
            StartCoroutine(SpinBoxCoroutine());
        }
        else
        {
            Debug.Log("Not enough points to purchase the Mystery Box.");
        }
    }

    IEnumerator AnimateLidOpen(float duration, float openAngle = 95f)
    {
        float elapsed = 0f;
        Quaternion startRot = boxLidTransform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, openAngle); // Z axis
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            boxLidTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boxLidTransform.localRotation = endRot;
    }

    IEnumerator AnimateLidOpenAndRaiseWeapon(float duration, float openAngle = 95f)
    {
        float elapsed = 0f;
        Quaternion startRot = boxLidTransform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, openAngle); // Z axis
        Vector3 startPos = initialWeaponLocalPos;
        Vector3 endPos = raisedWeaponLocalPos;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            boxLidTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            if (boxWeaponTransform != null)
                boxWeaponTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boxLidTransform.localRotation = endRot;
        if (boxWeaponTransform != null)
            boxWeaponTransform.localPosition = endPos;
    }

    IEnumerator HandleWeaponDescentAndClose(float descendDuration, float lidCloseDuration, MysteryBoxInteract boxinteract)
    {
        bool lidClosingStarted = false;
        float elapsed = 0f;
        Vector3 startPos = raisedWeaponLocalPos;
        Vector3 endPos = initialWeaponLocalPos;

        while (elapsed < descendDuration)
        {
            // If weapon is taken, start lid closing animation immediately
            if (currentWeaponModel == null && !lidClosingStarted)
            {
                lidClosingStarted = true;
                yield return StartCoroutine(AnimateLidClose(lidCloseDuration, 0f));
                boxinteract.ToggleInteract(true);
                yield break;
            }
            float t = elapsed / descendDuration;
            if (boxWeaponTransform != null)
                boxWeaponTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // After descent, if weapon is still present, destroy it and close the lid
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
            currentWeaponModel = null;
        }
        if (!lidClosingStarted)
        {
            yield return StartCoroutine(AnimateLidClose(lidCloseDuration, 0f));
            boxinteract.ToggleInteract(true);
        }
    }

    IEnumerator AnimateLidClose(float duration, float closeAngle = 0f)
    {
        float elapsed = 0f;
        Quaternion startRot = boxLidTransform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, closeAngle); // Z axis
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            boxLidTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boxLidTransform.localRotation = endRot;
        boxWeaponTransform.localPosition = initialWeaponLocalPos;
    }

    IEnumerator SpinBoxCoroutine() //4-5 sec spin,10s of weapons shown then box closes.
    {
        boxClosed = false;
        float spinDuration = 4.5f; // total time in seconds
        float lidOpenDuration = 1.0f;
        StartCoroutine(AnimateLidOpenAndRaiseWeapon(lidOpenDuration));
        float minDelay = 0.05f;
        float maxDelay = 0.25f;
        var guns = mapBasedGunScript.Guns;
        int gunCount = guns.Length;

        int chosenIndex = Random.Range(0, gunCount);

        float elapsed = 0f;
        int lastIndex = -1;

        while (elapsed < spinDuration)
        {
            int index = Random.Range(0, gunCount);
            // Avoid showing the same weapon twice in a row
            if (index == lastIndex)
                index = (index + 1) % gunCount;
            lastIndex = index;

            ShowWeaponModel(guns[index].Gun, false);

            float t = Mathf.Clamp01(elapsed / spinDuration);
            float delay = Mathf.Lerp(minDelay, maxDelay, t * t); // slows down at the end
            yield return new WaitForSeconds(delay);
            elapsed += delay;
        }

        // Show the chosen weapon
        ShowWeaponModel(guns[chosenIndex].Gun, true);

        // Start descent and closing logic
        yield return StartCoroutine(HandleWeaponDescentAndClose(10f, 1.0f, GetComponent<MysteryBoxInteract>()));
    }

    void ShowWeaponModel(PrimaryWeapon weapon, bool pickable)
    {
        if (boxWeaponTransform == null)
        {
            Debug.LogError("boxWeaponTransform is not set!");
            return;
        }
        if (currentWeaponModel != null)
            Destroy(currentWeaponModel);

        if (weapon != null && weapon.model != null && pickable == false)
        {
            currentWeaponModel = Instantiate(weapon.modelEmtpy, boxWeaponTransform.position, boxWeaponTransform.rotation, boxWeaponTransform);
        }
        else if (weapon != null && weapon.model != null && pickable == true)
        {
            currentWeaponModel = Instantiate(weapon.modelBoxPickup, boxWeaponTransform.position, boxWeaponTransform.rotation, boxWeaponTransform);
        }
    }
}